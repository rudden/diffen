using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

namespace Diffen.Repositories
{
	using Database;
	using Contracts;
	using Helpers.Extensions;
	using Database.Entities.User;
	using Database.Entities.Forum;

	using Filter = Models.Forum.Filter;
	using StartingEleven = Models.Forum.StartingEleven;

	public class PostRepository : IPostRepository
	{
		private readonly DiffenDbContext _dbContext;

		public PostRepository(DiffenDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IEnumerable<Post>> GetPostsAsync()
		{
			return await _dbContext.Posts.IncludeAll().ExceptScissored().OrderByCreated().ToListAsync();
		}

		public async Task<IEnumerable<Post>> GetPagedPostsAsync(int pageNumber, int pageSize)
		{
			return await _dbContext.Posts.IncludeAll().ExceptScissored()
				.OrderByCreated().Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
		}

		public async Task<int> CountAllPostsAsync()
		{
			return await _dbContext.Posts.ExceptScissored().CountAsync();
		}

		public async Task<IEnumerable<Post>> GetPostsOnUserIdAsync(string userId)
		{
			return await _dbContext.Posts.IncludeAll().ExceptScissored()
				.Where(post => post.CreatedByUserId == userId).OrderByCreated().ToListAsync();
		}

		public async Task<Post> GetPostOnIdAsync(int id)
		{
			return await _dbContext.Posts.IncludeAll().FirstOrDefaultAsync(post => post.Id == id);
		}

		public async Task<IEnumerable<Post>> GetPostsOnFilterAsync(Filter filter)
		{
			var posts = _dbContext.Posts.IncludeAll().ExceptScissored();
			if (filter == null)
			{
				return await posts.ToListAsync();
			}
			if (filter.ExcludedUsers != null && filter.ExcludedUsers.Any())
			{
				posts = posts.Where(x => !filter.ExcludedUsers.Select(y => y.Key).Contains(x.CreatedByUserId));
			}
			if (filter.FromDate != null)
			{
				posts = posts.Where(p => p.Created.Date >= Convert.ToDateTime(filter.FromDate).Date);
			}
			if (filter.ToDate != null)
			{
				posts = posts.Where(p => p.Created.Date <= Convert.ToDateTime(filter.ToDate).Date);
			}
			switch (filter.StartingEleven)
			{
				case StartingEleven.With:
					posts = posts.Where(x => x.Lineup != null);
					break;
				case StartingEleven.Without:
					posts = posts.Where(x => x.Lineup == null);
					break;
				case StartingEleven.All:
					break;
			}
			if (filter.IncludedUsers != null && filter.IncludedUsers.Any())
			{
				posts = posts.Where(x => filter.IncludedUsers.Select(y => y.Key).Contains(x.CreatedByUserId));
			}
			return await posts.ToListAsync();
		}

		public async Task<IEnumerable<Post>> GetSavedPosts(string userId)
		{
			return await _dbContext.SavedPosts.Include(x => x.Post)
				.Where(post => post.SavedByUserId == userId).Select(x => x.Post).ToListAsync();
		}

		public async Task<bool> AddPostAsync(Post post)
		{
			post.Created = DateTime.Now;
			_dbContext.Posts.Add(post);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<bool> UpdatePostAsync(Post post)
		{
			post.Edited = DateTime.Now;
			_dbContext.Posts.Update(post);

			_dbContext.Entry(post).State = EntityState.Modified;
			_dbContext.Entry(post).Property(x => x.Created).IsModified = false;

			var result = await _dbContext.SaveChangesAsync();

			_dbContext.Entry(post).State = EntityState.Detached;

			return result >= 0;
		}

		public async Task<bool> ScissorPostAsync(Scissored scissoredPost)
		{
			scissoredPost.Created = DateTime.Now;
			_dbContext.ScissoredPosts.Add(scissoredPost);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<bool> SavePostAsync(SavedPost savedPost)
		{
			savedPost.Created = DateTime.Now;
			_dbContext.SavedPosts.Add(savedPost);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<bool> AddLineupToPostAsync(PostToLineup postToLineup)
		{
			_dbContext.LineupsOnPosts.Add(postToLineup);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<bool> RemovePostToLineupAsync(int postId)
		{
			var post = await _dbContext.LineupsOnPosts.FirstOrDefaultAsync(x => x.PostId == postId);
			_dbContext.LineupsOnPosts.Remove(post);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<bool> PostToLineupExistsAsync(int postId)
		{
			return await _dbContext.LineupsOnPosts.CountAsync(x => x.PostId == postId) > 0;
		}

		public async Task<IEnumerable<UrlTip>> GetUrlTipsAsync()
		{
			return await _dbContext.UrlTips.Include(x => x.Post).ToListAsync();
		}

		public async Task<UrlTip> GetUrlTipOnIdAsync(int id)
		{
			return await _dbContext.UrlTips.FindAsync(id);
		}

		public async Task<UrlTip> GetUrlOnPostIdAsync(int postId)
		{
			return await _dbContext.UrlTips.Include(x => x.Post).FirstOrDefaultAsync(tip => tip.PostId == postId);
		}

		public async Task<bool> PostToUrlExistsAsync(int postId)
		{
			return await _dbContext.UrlTips.CountAsync(x => x.PostId == postId) > 0;
		}

		public async Task<bool> AddUrlToPostAsync(UrlTip tip)
		{
			_dbContext.UrlTips.Add(tip);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<bool> UpdateUrlToPostAsync(UrlTip tip)
		{
			_dbContext.UrlTips.Update(tip);

			_dbContext.Entry(tip).State = EntityState.Modified;
			_dbContext.Entry(tip).Property(x => x.Clicks).IsModified = false;

			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<bool> RemoveUrlToPostAsync(int postId)
		{
			var tip = await _dbContext.UrlTips.FirstOrDefaultAsync(x => x.PostId == postId);
			_dbContext.UrlTips.Remove(tip);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<bool> UpdateUrlTipClickCountAsync(int postId)
		{
			var tip = await _dbContext.UrlTips.FirstOrDefaultAsync(t => t.PostId == postId);
			tip.Clicks++;
			_dbContext.UrlTips.Update(tip);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<IEnumerable<Vote>> GetVotesAsync()
		{
			return await _dbContext.Votes.OrderByDescending(x => x.Created).ToListAsync();
		}

		public async Task<IEnumerable<Vote>> GetVotesOnUserIdAsync(string id)
		{
			return await _dbContext.Votes.Where(x => x.CreatedByUserId == id).OrderByDescending(x => x.Created).ToListAsync();
		}

		public async Task<IEnumerable<Vote>> GetVotesOnPostIdAsync(int id)
		{
			return await _dbContext.Votes.Include(x => x.User).ThenInclude(x => x.NickNames)
				.Where(x => x.PostId == id).OrderByDescending(x => x.Created).ToListAsync();
		}

		public async Task<bool> AddVoteAsync(Vote vote)
		{
			_dbContext.Votes.Add(vote);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<bool> UserHasAlreadyVotedAsync(int postId, string userId)
		{
			return await _dbContext.Votes.CountAsync(x => x.PostId == postId && x.CreatedByUserId == userId) > 0;
		}
	}
}
