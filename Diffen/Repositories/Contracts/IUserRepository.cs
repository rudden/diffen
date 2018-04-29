using System.Threading.Tasks;
using System.Collections.Generic;

namespace Diffen.Repositories.Contracts
{
	using Models;
	using Models.User;
	using Models.Squad;
	using Models.Other;

	public interface IUserRepository
	{
		Task<List<KeyValuePair<string, string>>> GetUsersAsKeyValuePairAsync();
		Task<User> GetUserOnIdAsync(string userId);
		Task<User> GetUserOnEmailAsync(string email);
		Task<List<Result>> UpdateUserAsync(string userId, Models.User.CRUD.User user);
		Task<Player> GetFavoritePlayerAsync(string userId);
		Task<string> GetCurrentNickOnUserIdAsync(string userId);
		Task<bool> NickExistsAsync(string nickName);
		Task<bool> CreateNewNickNameAsync(string userId, string nickName);
		Task<bool> SetSelectedAvatarForUserAsync(string userId, string fileName);
		Task<List<Result>> UpdateUserFilterAsync(string userId, Filter filter);
		Task<bool> EmailHasInvite(string email);
		Task<Invite> GetInviteOnEmailAsync(string email);
		Task<List<Invite>> GetInvitesAsync();
		Task<List<Result>> CreateInviteAsync(Models.User.CRUD.Invite invite);
		Task<bool> SetInviteAsAccountCreatedAsync(string userEmail);
		Task<List<Result>> SecludeUserAsync(string userId, string toDate);
		Task<List<KeyValuePair<string, string>>> GetUsersInRoleAsKeyValuePairAsync(string roleName);
		Task<List<Result>> UpdateAvatarFileNameForUserWithIdAsync(string userId, string fileName);
		Task<List<Result>> ResetUsersAvatarToGenericAsync(string userId);
		Task<bool> CreateRegionToUserAsync(string userId, int regionId);
	}
}
