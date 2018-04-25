using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using AutoMapper;

namespace Diffen.Repositories
{
	using Contracts;
	using Models;
	using Models.Forum;
	using Helpers;
	using Helpers.Extensions;
	using Database.Clients.Contracts;

	public class PostRepository : IPostRepository
	{
		private readonly IMapper _mapper;
		private readonly IDiffenDbClient _dbClient;

		public PostRepository(IMapper mapper, IDiffenDbClient dbClient)
		{
			_mapper = mapper;
			_dbClient = dbClient;
		}

		public async Task<List<Post>> GetPostsAsync()
		{
			var posts = await _dbClient.GetPostsAsync();
			return _mapper.Map<List<Post>>(posts);
		}

		public async Task<Paging<Post>> GetPagedPostsAsync(int pageNumber, int pageSize)
		{
			var posts = await _dbClient.GetPagedPostsAsync(pageNumber, pageSize);
			return _mapper.Map<List<Post>>(posts.Page(pageNumber, pageSize)).ToPaging(posts.Count, pageNumber, pageSize);
		}

		public async Task<Paging<Post>> GetPagedPostsOnUserIdAsync(string userId, int pageNumber, int pageSize = 5)
		{
			var posts = await _dbClient.GetPostsOnUserIdAsync(userId);
			return _mapper.Map<List<Post>>(posts.Page(pageNumber, pageSize)).ToPaging(posts.Count, pageNumber, pageSize);
		}

		public async Task<Paging<Post>> GetPagedPostsOnFilterAsync(int pageNumber, int pageSize, Filter filter)
		{
			var posts = await _dbClient.GetPostsOnFilterAsync(filter);
			return _mapper.Map<List<Post>>(posts.Page(pageNumber, pageSize)).ToPaging(posts.Count, pageNumber, pageSize);
		}

		public async Task<Paging<Post>> GetPagedSavedPostsAsync(string userId, int pageNumber, int pageSize = 5)
		{
			var posts = await _dbClient.GetSavedPostsOnUserIdAsync(userId);
			return _mapper.Map<List<Post>>(posts.Page(pageNumber, pageSize)).ToPaging(posts.Count, pageNumber, pageSize);
		}

		public async Task<Post> GetPostOnIdAsync(int id)
		{
			var post = await _dbClient.GetPostOnIdAsync(id);
			return _mapper.Map<Post>(post);
		}

		public async Task<List<Post>> GetConversationOnPostIdAsync(int id)
		{
			var conversation = await _dbClient.GetConversationOnPostIdAsync(id);
			return _mapper.Map<List<Post>>(conversation);
		}

		public Task<int> CountAllPostsAsync()
		{
			return _dbClient.CountPostsAsync();
		}

		public async Task<List<Result>> CreatePostAsync(Models.Forum.CRUD.Post post)
		{
			var newPost = _mapper.Map<Database.Entities.Forum.Post>(post);
			newPost.Created = DateTime.Now;

			var isCreated = _dbClient.CreatePostAsync(newPost);
			var results = await new List<Result>().Get(isCreated, ResultMessages.CreatePost);

			if (await isCreated)
			{
				await ComplementPostWithPotentialUrlTipAndLineupAsync(newPost.Id, post, results);
			}

			return results;
		}

		public async Task<List<Result>> UpdatePostAsync(Models.Forum.CRUD.Post post)
		{
			var updatePost = _mapper.Map<Database.Entities.Forum.Post>(post);
			updatePost.Updated = DateTime.Now;

			var isUpdated = _dbClient.UpdatePostAsync(updatePost);
			var results = await new List<Result>().Get(isUpdated, ResultMessages.UpdatePost);

			if (await isUpdated)
			{
				await ComplementPostWithPotentialUrlTipAndLineupAsync(updatePost.Id, post, results);
			}

			return results;
		}

		public Task<bool> ScissorPostAsync(int postId)
		{
			var scissored = new Database.Entities.Forum.Scissored
			{
				PostId = postId,
				Created = DateTime.Now
			};
			return _dbClient.ScissorPostAsync(scissored);
		}

		public Task<bool> SavePostAsync(int postId, string userId)
		{
			var savedPost = new Database.Entities.User.SavedPost
			{
				PostId = postId,
				SavedByUserId = userId,
				Created = DateTime.Now
			};
			return _dbClient.SavePostForUserAsync(savedPost);
		}

		public async Task<List<UrlTip>> GetLastMonthsMostClickedUrlTipsAsync()
		{
			var urlTips = await _dbClient.GetUrlTipsAsync();
			var topList = urlTips.Where(x => x.Post.Created > DateTime.Now.AddMonths(-1))
				.Select(x => new UrlTip
				{
					Href = x.Href,
					Clicks = x.Clicks,
					PostId = x.PostId
				}).OrderByDescending(x => x.Clicks).Take(10).ToList();
			return topList;
		}

		public Task<bool> UpdateUrlTipClickCountAsync(int postId)
		{
			return _dbClient.IncrementUrlTipClickCounterAsync(postId);
		}

		public async Task<List<Vote>> GetVotesOnPostIdAsync(int postId)
		{
			var votes = await _dbClient.GetVotesOnPostIdAsync(postId);
			return _mapper.Map<List<Vote>>(votes);
		}

		public async Task<bool> CreateVoteAsync(Models.Forum.CRUD.Vote vote)
		{
			if (await _dbClient.UserHasAlreadyVotedOnPostAsync(vote.PostId, vote.CreatedByUserId))
			{
				return false;
			}
			var newVote = _mapper.Map<Database.Entities.Forum.Vote>(vote);
			return await _dbClient.CreateVoteAsync(newVote);
		}

		private async Task ComplementPostWithPotentialUrlTipAndLineupAsync(int postId, Models.Forum.CRUD.Post post, List<Result> results)
		{
			if (!string.IsNullOrEmpty(post.UrlTipHref))
			{
				if (!(post.UrlTipHref.StartsWith("http://") || post.UrlTipHref.StartsWith("https://")))
				{
					post.UrlTipHref = post.UrlTipHref.Insert(0, "http://");
				}
				var urlTip = new Database.Entities.Forum.UrlTip
				{
					PostId = postId,
					Clicks = 0,
					Href = post.UrlTipHref
				};
				results.Update(await _dbClient.CreateUrlTipAsync(urlTip), ResultMessages.CreateUrlTip);
			}
			else
			{
				if (await _dbClient.PostHasAnUrlTipConnectedToItAsync(postId))
				{
					await _dbClient.DeleteUrlTipAsync(postId);
				}
			}

			if (post.LineupId > 0)
			{
				if (await _dbClient.PostHasALineupConnectedToItAsync(postId))
				{
					await _dbClient.DeleteLineupConnectionToPostAsync(postId);
				}
				var postToLineup = new Database.Entities.Forum.PostToLineup
				{
					PostId = postId,
					LineupId = post.LineupId
				};
				results.Update(await _dbClient.ConnectLineupToPostAsync(postToLineup), ResultMessages.CreateLineupToPost);
			}
			else
			{
				if (await _dbClient.PostHasALineupConnectedToItAsync(postId))
				{
					await _dbClient.DeleteLineupConnectionToPostAsync(postId);
				}
			}
		}
	}
}
