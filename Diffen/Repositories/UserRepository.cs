using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Diffen.Repositories
{
	using Database;
	using Contracts;
	using Helpers.Extensions;
	using Database.Entities.User;

	public class UserRepository : IUserRepository
	{
		private readonly DiffenDbContext _dbContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UserRepository(DiffenDbContext dbContext, IHttpContextAccessor httpContextAccessor)
		{
			_dbContext = dbContext;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<IEnumerable<AppUser>> GetUsersAsync()
		{
			return await _dbContext.Users
				.Include(x => x.NickNames)
				.Where(x => x.UserName != _httpContextAccessor.HttpContext.User.Identity.Name)
				.OrderByDescending(x => x.Joined)
				.ToListAsync();
		}

		public async Task<AppUser> GetUserOnIdAsync(string id)
		{
			return await _dbContext.Users.IncludeAll().FirstOrDefaultAsync(user => user.Id == id);
		}

		public async Task<AppUser> GetUserOnEmailAsync(string email)
		{
			return await _dbContext.Users.IncludeAll().FirstOrDefaultAsync(user => user.Email == email);
		}

		public async Task<bool> UpdateUserAsync(AppUser user)
		{
			_dbContext.Users.Update(user);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<FavoritePlayer> GetFavoritePlayerAsync(string userId)
		{
			return await _dbContext.FavoritePlayers.Include(x => x.Player).FirstOrDefaultAsync(x => x.UserId == userId);
		}

		public async Task<bool> FavoritePlayerExistsAsync(string userId)
		{
			return await _dbContext.FavoritePlayers.CountAsync(x => x.UserId == userId) > 0;
		}

		public async Task<bool> AddFavoritePlayerAsync(FavoritePlayer favoritePlayer)
		{
			_dbContext.FavoritePlayers.Add(favoritePlayer);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<bool> RemovePlayerToUserAsync(string userId)
		{
			var item = _dbContext.FavoritePlayers.FirstOrDefault(x => x.UserId == userId);
			_dbContext.FavoritePlayers.Remove(item);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<bool> AddNickNameAsync(NickName nickname)
		{
			_dbContext.NickNames.Add(nickname);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<string> GetCurrentNickOnUserIdAsync(string userId)
		{
			var activeNick = await _dbContext.NickNames.Where(x => x.UserId == userId).OrderByDescending(x => x.Created).FirstOrDefaultAsync();
			return activeNick.Nick;
		}

		public async Task<bool> NickExistsAsync(string nick)
		{
			var activeNicks = _dbContext.NickNames.OrderByDescending(x => x.Created).GroupBy(x => x.UserId).Select(x => x.FirstOrDefault().Nick);
			return await activeNicks.CountAsync(x => x == nick) > 0;
		}

		public async Task<Filter> GetFiltersOnUserIdAsync(string userId)
		{
			return await _dbContext.UserFilters.FirstOrDefaultAsync(u => u.UserId == userId);
		}

		public async Task<bool> UpdateUserFilterAsync(Filter filter)
		{
			_dbContext.UserFilters.Update(filter);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<bool> AddUserFilterAsync(Filter filter)
		{
			_dbContext.UserFilters.Add(filter);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<bool> EmailHasInvite(string email)
		{
			return await _dbContext.Invites.CountAsync(x => x.Email.Equals(email)) > 0;
		}

		public async Task<Invite> GetInviteOnEmailAsync(string email)
		{
			return await _dbContext.Invites.FirstOrDefaultAsync(x => x.Email.Equals(email));
		}

		public async Task<IEnumerable<Invite>> GetInvitesAsync()
		{
			return await _dbContext.Invites.Include(x => x.InvitedByUser).ThenInclude(x => x.NickNames).OrderByDescending(x => x.InviteSent).ToListAsync();
		}

		public async Task<bool> AddInviteAsync(Invite invite)
		{
			_dbContext.Invites.Add(invite);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<bool> UpdateInviteAsync(Invite invite)
		{
			_dbContext.Invites.Update(invite);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<bool> RemoveInviteAsync(Invite invite)
		{
			_dbContext.Invites.Remove(invite);
			return await _dbContext.SaveChangesAsync() >= 0;
		}
	}
}
