using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Serilog;
using AutoMapper;
using Newtonsoft.Json;

namespace Diffen.Controllers.Api
{
	using Models;
	using Helpers;
	using Models.Forum;
	using Repositories.Contracts;

	[Route("api/[controller]")]
	public class PostsController : Controller
	{
		private readonly IPostRepository _postRepository;

		private readonly ILogger _logger = Log.ForContext<PostsController>();

		public PostsController(IMapper mapper, IPostRepository postRepository)
		{
			_postRepository = postRepository;
		}

		[HttpGet]
		public Task<List<Post>> Get()
		{
			_logger.Debug("Requesting all posts");
			return _postRepository.GetPostsAsync();
		}

		[HttpGet("{postId}")]
		public Task<Post> Get(int postId)
		{
			_logger.Debug("Requesting post with id {postId}", postId);
			return _postRepository.GetPostOnIdAsync(postId);
		}

		[HttpGet("page/{pageNumber}/{pageSize}")]
		public Task<Paging<Post>> GetPage(int pageNumber, int pageSize, string filter)
		{
			_logger.Debug("Requesting {pageSize} posts on page {pageNumber}", pageSize, pageNumber);
			var serializedFilter = JsonConvert.DeserializeObject<Filter>(filter);
			return serializedFilter == null
				? _postRepository.GetPagedPostsAsync(pageNumber, pageSize)
				: _postRepository.GetPagedPostsOnFilterAsync(pageNumber, pageSize, serializedFilter);
		}

		[HttpPost("{userId}/create")]
		public Task<List<Result>> CreatePost(string userId, [FromBody] Models.Forum.CRUD.Post post)
		{
			_logger.Debug("Requesting to create a new post");
			return _postRepository.CreatePostAsync(post);
		}

		[HttpPost("{userId}/update")]
		public Task<List<Result>> UpdatePost(string userId, [FromBody] Models.Forum.CRUD.Post post)
		{
			_logger.Debug("Requesting to update an existing post");
			return _postRepository.UpdatePostAsync(post);
		}

		[HttpPost("{postId}/scissor")]
		public Task<bool> Scissor(int postId)
		{
			_logger.Debug("Requesting to set a post as scissored (hidden from forum)");
			return _postRepository.ScissorPostAsync(postId);
		}

		[HttpPost("vote")]
		public Task<bool> Vote([FromBody] Models.Forum.CRUD.Vote vote)
		{
			_logger.Debug("Requesting to vote on a post with id {postId}", vote.PostId);
			return _postRepository.CreateVoteAsync(vote);
		}

		[HttpPost("{postId}/url/click")]
		public Task<bool> UrlTipClickCount(int postId)
		{
			_logger.Debug("Requesting to increment click count for urltip connected to post with id {postId}", postId);
			return _postRepository.UpdateUrlTipClickCountAsync(postId);
		}

		[HttpGet("url/toplist")]
		public Task<List<UrlTip>> UrlTipClickCountTopList()
		{
			_logger.Debug("Requesting to get last months most clicked urltips");
			return _postRepository.GetLastMonthsMostClickedUrlTipsAsync();
		}

		[HttpPost("{postId}/bookmark")]
		public Task<bool> Bookmark(int postId, string userId)
		{
			_logger.Debug("Requesting to save a post with id {postId} for user with id {userId}", postId, userId);
			return _postRepository.SavePostAsync(postId, userId);
		}
	}
}
