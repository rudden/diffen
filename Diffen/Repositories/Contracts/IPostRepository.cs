using System.Threading.Tasks;
using System.Collections.Generic;

namespace Diffen.Repositories.Contracts
{
	using Helpers;
	using Models;
	using Models.Forum;

	public interface IPostRepository
	{
		Task<List<Post>> GetPostsAsync();
		Task<Paging<Post>> GetPagedPostsAsync(int pageNumber, int pageSize);
		Task<Paging<Post>> GetPagedPostsOnUserIdAsync(string userId, int pageNumber, int pageSize = 5);
		Task<Paging<Post>> GetPagedPostsOnFilterAsync(int pageNumber, int pageSize, Filter filter);
		Task<Paging<Post>> GetPagedSavedPostsAsync(string userId, int pageNumber, int pageSize = 5);
		Task<Post> GetPostOnIdAsync(int id);
		Task<List<Post>> GetConversationOnPostIdAsync(int id);
		Task<int> CountAllPostsAsync();
		Task<List<Result>> CreatePostAsync(Models.Forum.CRUD.Post post);
		Task<List<Result>> UpdatePostAsync(Models.Forum.CRUD.Post post);
		Task<bool> ScissorPostAsync(int postId);
		Task<bool> SavePostAsync(int postId, string userId);
		Task<bool> UnSavePostAsync(int postId, string userId);
		Task<List<UrlTip>> GetUrlTipsTopListAsync();
		Task<bool> UpdateUrlTipClickCountAsync(string subject, int id);
		Task<List<Vote>> GetVotesOnPostIdAsync(int postId);
		Task<bool> CreateVoteAsync(Models.Forum.CRUD.Vote vote);
		Task<List<Thread>> GetThreadsAsync();
		Task<bool> UpdateThreadsOnPostAsync(int postId, List<int> threadIds);
	}
}
