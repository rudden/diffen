using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Serilog;
using AutoMapper;

namespace Diffen.Controllers.Api
{
	using Models;
	using Helpers;
	using Helpers.Extensions;
	using Repositories.Contracts;
	using Database.Entities.Forum;
	
	using Post = Models.Forum.Post;
	using Vote = Models.Forum.Vote;

	[Route("api/[controller]")]
	public class PostsController : Controller
	{
		private readonly IMapper _mapper;
		private readonly IPostRepository _postRepository;
		private readonly ISquadRepository _squadRepository;

		private readonly ILogger _logger = Log.ForContext<PostsController>();

		public PostsController(IMapper mapper, IPostRepository postRepository, ISquadRepository squadRepository)
		{
			_mapper = mapper;
			_postRepository = postRepository;
			_squadRepository = squadRepository;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			try
			{
				return Json(_mapper.Map<List<Post>>(await _postRepository.GetPostsAsync()));
			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "Get: An unexpected error occured when fetching posts");
				return BadRequest();
			}
		}

		[HttpGet("{postId}")]
		public async Task<IActionResult> Get(int postId)
		{
			try
			{
				return Json(_mapper.Map<List<Post>>(await _postRepository.GetPostOnIdAsync(postId)));
			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "Get: An unexpected error occured when fetching post with id {postId}", postId);
				return BadRequest();
			}
		}

		[HttpGet("page/{pageNumber}/{pageSize}")]
		public async Task<IActionResult> GetPage(int pageNumber, int pageSize)
		{
			try
			{
				var posts = _mapper.Map<List<Post>>(await _postRepository.GetPagedPostsAsync(pageNumber, pageSize));
				var numberOfPosts = await _postRepository.CountAllPostsAsync();
				return Json(new Paging<Post>
				{
					Data = posts,
					CurrentPage = pageNumber,
					NumberOfPages = Convert.ToInt32(Math.Ceiling((double)numberOfPosts / pageSize)),
					Total = numberOfPosts
				});
			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "GetPage: An unexpected error occured when fetching {pageSize} posts on page {pageNumber}", pageSize, pageNumber);
				return BadRequest();
			}
		}

		[HttpGet("user/{userId}")]
		public async Task<IActionResult> GetPostsOnUserId(string userId)
		{
			try
			{
				return Json(_mapper.Map<List<Post>>(await _postRepository.GetPostsOnUserIdAsync(userId)));
			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "GetPostsOnUserId: An unexpected error occured when fetching posts for user {userId}", userId);
				return BadRequest();
			}
		}

		[HttpPost("{userId}/create")]
		public async Task<IActionResult> CreatePost(string userId, [FromBody] Models.Forum.CRUD.Post post)
		{
			try
			{
				if (post == null)
					return BadRequest();

				var results = new List<Result>();

				var newPost = _mapper.Map<Database.Entities.Forum.Post>(post);

				await _postRepository.AddPostAsync(newPost)
					.ContinueWith(task => task.UpdateResults(ResultMessages.CreatePost, results));

				if (post.ParentId > 0)
				{
					await _postRepository.AddConversationAsync(new PostToPost
					{
						PostId = newPost.Id,
						ParentPostId = post.ParentId
					}).ContinueWith(task => task.UpdateResults(ResultMessages.CreateConversation, results));
				}

				await ComplementPostWithPotentialUrlTipAndLineupAsync(post, results);

				return Json(results);
			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "CreatePost: An unexpected error occured when adding a new post");
				return BadRequest();
			}
		}

		[HttpPost("{userId}/update")]
		public async Task<IActionResult> UpdatePost(string userId, [FromBody] Models.Forum.CRUD.Post post)
		{
			try
			{
				if (post == null)
					return BadRequest();

				var results = new List<Result>();

				var updatePost = _mapper.Map<Database.Entities.Forum.Post>(post);

				await _postRepository.UpdatePostAsync(updatePost)
					.ContinueWith(task => task.UpdateResults(ResultMessages.UpdatePost, results));

				await ComplementPostWithPotentialUrlTipAndLineupAsync(post, results);

				return Json(results);
			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "UpdatePost: An unexpected error occured when updating an existing post with id {postId}", post?.Id);
				return BadRequest();
			}
		}

		[HttpPost("{postId}/scissor")]
		public async Task<IActionResult> Scissor(int postId)
		{
			try
			{
				var post = await _postRepository.GetPostOnIdAsync(postId);
				if (post == null)
					return BadRequest("Inlägget finns inte");

				return Json(await _postRepository.ScissorPostAsync(new Scissored
				{
					PostId = postId,
					Created = DateTime.Now
				}));
			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "Scissor: An unexpected error occured when scissoring a post with id {postId}", postId);
				return BadRequest();
			}
		}

		[HttpGet("{postId}/votes")]
		public async Task<IActionResult> GetVotesOnPost(int postId)
		{
			try
			{
				return Json(_mapper.Map<List<Vote>>(await _postRepository.GetVotesOnPostIdAsync(postId)));
			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "GetVotesOnPost: An unexpected error occured when fetching votes for post {postId}", postId);
				return BadRequest();
			}
		}

		[HttpPost("{postId}/vote/{type}")]
		public async Task<IActionResult> Vote([FromBody] Models.Forum.CRUD.Vote vote)
		{
			try
			{
				if (vote == null)
					return BadRequest();

				if (await _postRepository.UserHasAlreadyVotedAsync(vote.PostId, vote.CreatedByUserId))
				{
					_logger.Information($"{User.Identity.Name} tried to vote a post for that he or she has already voted on. Aborting!");
					return Forbid();
				}

				var newVote = _mapper.Map<Database.Entities.Forum.Vote>(vote);
				return Json(await _postRepository.AddVoteAsync(newVote));
			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "Vote: An unexpected error occured trying to vote post {postId}", vote?.PostId);
				return BadRequest();
			}
		}

		[HttpPost("url/click/{tipId}")]
		public async Task<IActionResult> UrlTipClickCount(int tipId)
		{
			try
			{
				return Json(await _postRepository.UpdateUrlTipClickCountAsync(tipId));
			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "UrlTipClickCount: An unexpected error occured when updating click count for tip {tipId}", tipId);
				return BadRequest();
			}
		}

		private async Task ComplementPostWithPotentialUrlTipAndLineupAsync(Models.Forum.CRUD.Post post, ICollection<Result> results)
		{
			if (!string.IsNullOrEmpty(post.UrlTip?.Href))
			{
				if (!(post.UrlTip.Href.StartsWith("http://") || post.UrlTip.Href.StartsWith("https://")))
				{
					post.UrlTip.Href = post.UrlTip.Href.Insert(0, "http://");
				}
				await _postRepository.AddUrlToPostAsync(new UrlTip
				{
					PostId = post.Id,
					Clicks = 0,
					Href = post.UrlTip.Href,
					Created = DateTime.Now
				}).ContinueWith(task => task.UpdateResults(ResultMessages.CreateUrlTip, results));
			}

			if (post.Lineup != null)
			{
				if (post.Lineup.Id > 0)
				{
					if (await _postRepository.PostToLineupExistsAsync(post.Lineup.Id))
					{
						await _postRepository.RemovePostToLineupAsync(post.Lineup.Id);
					}
					await _postRepository.AddLineupToPostAsync(new PostToLineup
					{
						PostId = post.Id,
						LineupId = post.Lineup.Id
					}).ContinueWith(task => task.UpdateResults(ResultMessages.CreateLineupToPost, results));
				}
				else
				{
					var newLineup = _mapper.Map<Database.Entities.Squad.Lineup>(post.Lineup);

					await _squadRepository.AddLineupAsync(newLineup)
						.ContinueWith(task => task.UpdateResults(ResultMessages.CreateLineup, results));

					await _postRepository.AddLineupToPostAsync(new PostToLineup
					{
						PostId = post.Id,
						LineupId = newLineup.Id
					}).ContinueWith(task => task.UpdateResults(ResultMessages.CreateLineupToPost, results));
				}
			}
		}

	}
}
