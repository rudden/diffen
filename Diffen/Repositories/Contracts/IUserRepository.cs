using System.Threading.Tasks;
using System.Collections.Generic;

namespace Diffen.Repositories.Contracts
{
	using Database.Entities.User;
	
	public interface IUserRepository
	{
		Task<IEnumerable<AppUser>> GetUsersAsync();
		Task<AppUser> GetUserOnIdAsync(string id);
		Task<AppUser> GetUserOnEmailAsync(string email);
		Task<bool> UpdateUserAsync(AppUser user);
		Task<FavoritePlayer> GetFavoritePlayerAsync(string userId);
		Task<bool> FavoritePlayerExistsAsync(string userId);
		Task<bool> AddFavoritePlayerAsync(FavoritePlayer favoritePlayer);
		Task<bool> RemovePlayerToUserAsync(string userId);
		Task<bool> AddNickNameAsync(NickName nickname);
		Task<string> GetCurrentNickOnUserIdAsync(string userId);
		Task<bool> NickExistsAsync(string nick);
		Task<Filter> GetFiltersOnUserIdAsync(string userId);
		Task<bool> UpdateUserFilterAsync(Filter filter);
		Task<bool> AddUserFilterAsync(Filter filter);
		Task<bool> EmailHasInvite(string email);
		Task<Invite> GetInviteOnEmailAsync(string email);
		Task<IEnumerable<Invite>> GetInvitesAsync();
		Task<bool> AddInviteAsync(Invite invite);
		Task<bool> UpdateInviteAsync(Invite invite);
		Task<bool> RemoveInviteAsync(Invite invite);
	}
}
