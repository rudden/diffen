using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using AutoMapper;

namespace Diffen.Repositories
{
	using Contracts;
	using Database.Clients.Contracts;
	using Helpers;
	using Helpers.Extensions;
	using Models;
	using Models.Forum;

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
				await ComplementPostWithPotentialUrlTipThreadsAndLineupAsync(newPost.Id, post, results);
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
				await ComplementPostWithPotentialUrlTipThreadsAndLineupAsync(updatePost.Id, post, results);
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

		public async Task<bool> UnSavePostAsync(int postId, string userId)
		{
			var savedPost = await _dbClient.GetSavedPostsOnPostAndUserIdAsync(postId, userId);
			return await _dbClient.DeleteSavedPostForUserAsync(savedPost);
		}

		public async Task<List<UrlTip>> GetLastMonthsMostClickedUrlTipsAsync()
		{
			var urlTips = await _dbClient.GetUrlTipsAsync();
			var topList = urlTips.Where(x => x.Created.Date.Day == DateTime.Now.Day)
				.Select(x => new UrlTip
				{
					Id = x.Id,
					Href = _mapper.Map<string>(x),
					Clicks = x.Clicks,
					PostId = x.PostId
				}).OrderByDescending(x => x.Clicks).Take(10).ToList();
			return topList;
		}

		public Task<bool> UpdateUrlTipClickCountAsync(string subject, int id)
		{
			return _dbClient.IncrementUrlTipClickCounterAsync(subject, id);
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

		public async Task<List<Thread>> GetThreadsAsync()
		{
			var threads = await _dbClient.GetPostThreadsAsync();
			var mappedThreads = _mapper.Map<List<Thread>>(threads);
			foreach (var thread in mappedThreads)
			{
				thread.NumberOfPosts = await _dbClient.GetNumberOfPostsOnThreadAsync(thread.Id);
			}
			return mappedThreads;
		}

		public async Task<bool> UpdateThreadsOnPostAsync(int postId, List<int> threadIds)
		{
			var result = await _dbClient.DeleteExistingThreadsOnPostAsync(postId);
			if (threadIds.Any())
			{
				result = await _dbClient.AddThreadsToPostAsync(postId, threadIds);
			}
			return result;
		}

		private async Task ComplementPostWithPotentialUrlTipThreadsAndLineupAsync(int postId, Models.Forum.CRUD.Post post, List<Result> results)
		{
			if (!string.IsNullOrEmpty(post.UrlTipHref))
			{
				if (!_dbClient.UrlTipEqualsCurrentUrlTipOnPostId(postId, post.UrlTipHref))
				{
					if (!(post.UrlTipHref.StartsWith("http://") || post.UrlTipHref.StartsWith("https://")))
					{
						post.UrlTipHref = post.UrlTipHref.Insert(0, "http://");
					}
					var urlTip = new Database.Entities.Forum.UrlTip
					{
						PostId = postId,
						Clicks = 0,
						Href = post.UrlTipHref,
						Created = DateTime.Now
					};
					results.Update(await _dbClient.CreateUrlTipAsync(urlTip), ResultMessages.CreateUrlTip);
				}
			}
			else
			{
				if (await _dbClient.PostHasAnUrlTipConnectedToItAsync(postId))
				{
					await _dbClient.DeleteUrlTipAsync(postId);
				}
			}

			await _dbClient.DeleteExistingThreadsOnPostAsync(postId);
			if (post.ThreadIds.Any())
			{
				await _dbClient.AddThreadsToPostAsync(postId, post.ThreadIds);
			}
			if (post.NewThreadNames != null && post.NewThreadNames.Any())
			{
				await _dbClient.CreatePostThreadsAndConnectToNewPostWithIdAsync(postId, post.NewThreadNames.ToList());
			}

			if (post.Lineup != null)
			{
				if (await _dbClient.PostHasALineupConnectedToItAsync(postId))
				{
					await _dbClient.DeleteLineupConnectionToPostAsync(postId);
				}
				var newLineup = _mapper.Map<Database.Entities.Squad.Lineup>(post.Lineup);
				var result = await _dbClient.CreateLineupAsync(newLineup);
				results.Update(result, ResultMessages.CreateLineup);
				if (result)
				{
					await ConnectLineupToPostAsync(postId, newLineup.Id, results);
				}
			}
			else
			{
				if (post.LineupId > 0)
				{
					await ConnectLineupToPostAsync(postId, post.LineupId, results);
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

		private async Task ConnectLineupToPostAsync(int postId, int lineupId, List<Result> results)
		{
			var postToLineup = new Database.Entities.Forum.PostToLineup
			{
				PostId = postId,
				LineupId = lineupId,
				Created = DateTime.Now
			};
			results.Update(await _dbClient.ConnectLineupToPostAsync(postToLineup), ResultMessages.CreateLineupToPost);
		}
	}
}
