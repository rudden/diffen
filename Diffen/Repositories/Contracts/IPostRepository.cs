using System.Threading.Tasks;
using System.Collections.Generic;

namespace Diffen.Repositories.Contracts
{
	using Database.Entities.User;
	using Database.Entities.Forum;

	using Filter = Models.Forum.Filter;

	public interface IPostRepository
	{
		Task<IEnumerable<Post>> GetPostsAsync();
		Task<IEnumerable<Post>> GetPagedPostsAsync(int pageNumber, int pageSize);
		Task<int> CountAllPostsAsync();
		Task<IEnumerable<Post>> GetPostsOnUserIdAsync(string userId);
		Task<Post> GetPostOnIdAsync(int id);
		Task<IEnumerable<Post>> GetPostsOnFilterAsync(Filter filter);
		Task<IEnumerable<Post>> GetSavedPosts(string userId);
		Task<bool> AddPostAsync(Post post);
		Task<bool> UpdatePostAsync(Post post);
		Task<bool> ScissorPostAsync(Scissored scissoredPost);
		Task<bool> SavePostAsync(SavedPost savedPost);
		Task<bool> AddLineupToPostAsync(PostToLineup postToLineup);
		Task<bool> RemovePostToLineupAsync(int postId);
		Task<bool> PostToLineupExistsAsync(int postId);
		Task<IEnumerable<UrlTip>> GetUrlTipsAsync();
		Task<UrlTip> GetUrlOnPostIdAsync(int postId);
		Task<bool> PostToUrlExistsAsync(int postId);
		Task<bool> AddUrlToPostAsync(UrlTip tip);
		Task<bool> UpdateUrlToPostAsync(UrlTip tip);
		Task<bool> RemoveUrlToPostAsync(int postId);
		Task<bool> UpdateUrlTipClickCountAsync(int postId);
		Task<IEnumerable<Vote>> GetVotesAsync();
		Task<IEnumerable<Vote>> GetVotesOnUserIdAsync(string id);
		Task<IEnumerable<Vote>> GetVotesOnPostIdAsync(int id);
		Task<bool> AddVoteAsync(Vote vote);
		Task<bool> UserHasAlreadyVotedAsync(int postId, string userId);
	}
}
