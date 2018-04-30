using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using AutoMapper;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Diffen.Repositories
{
	using Contracts;
	using Models;
	using Models.User;
	using Models.Squad;
	using Models.Other;
	using Helpers.Extensions;
	using Database.Clients.Contracts;

	using AppUser = Database.Entities.User.AppUser;

	public class UserRepository : IUserRepository
	{
		private readonly IMapper _mapper;
		private readonly IDiffenDbClient _dbClient;
		private readonly UserManager<AppUser> _userManager;
		private readonly IUploadRepository _uploadRepository;

		public UserRepository(IMapper mapper, IDiffenDbClient dbClient, UserManager<AppUser> userManager, IUploadRepository uploadRepository)
		{
			_mapper = mapper;
			_dbClient = dbClient;
			_userManager = userManager;
			_uploadRepository = uploadRepository;
		}

		public async Task<List<KeyValuePair<string, string>>> GetUsersAsKeyValuePairAsync()
		{
			var users = await _dbClient.GetUsersAsync();
			return users.Select(user =>
					new KeyValuePair<string, string>(user.Id,
						user.NickNames.Current())).ToList();
		}

		public async Task<User> GetUserOnIdAsync(string userId)
		{
			var user = await _dbClient.GetUserOnIdAsync(userId);
			return _mapper.Map<User>(user);
		}

		public async Task<User> GetUserOnEmailAsync(string email)
		{
			var user = await _dbClient.GetUserOnEmailAsync(email);
			return _mapper.Map<User>(user);
		}

		public async Task<List<Result>> UpdateUserAsync(string userId, Models.User.CRUD.User user)
		{
			// user is fetched with user manager due to issue with entity framework (entity is already being tracked...)
			var currentUser = await _userManager.Users.Include(x => x.NickNames).Include(x => x.FavoritePlayer).Include(x => x.Region).ThenInclude(x => x.Region).FirstOrDefaultAsync(x => x.Id == userId);
			var currentNick = currentUser.NickNames.Current();

			var results = new List<Result>();
			if (!string.IsNullOrEmpty(currentNick) && !currentNick.Equals(user.NickName))
			{
				if (!await _dbClient.NickNameIsAlreadyTakenByOtherUserAsync(user.NickName))
				{
					var nickName = new Database.Entities.User.NickName
					{
						UserId = userId,
						Nick = user.NickName,
						Created = DateTime.Now
					};
					results.Update(await _dbClient.CreateNewNickNameForUserAsync(nickName), ResultMessages.CreateNick);
				}
			}
			if (currentUser.Bio == null && !string.IsNullOrEmpty(user.Bio) || currentUser.Bio != null && !currentUser.Bio.Equals(user.Bio))
			{
				results.Update(await _dbClient.UpdateUserBioAsync(userId, user.Bio), ResultMessages.UpdateBio);
			}

			if (!string.IsNullOrEmpty(user.Region))
			{
				if (currentUser.Region?.Region?.Name != user.Region)
				{
					results.Update(await _dbClient.UpdateRegionForUserAsync(userId, user.Region), ResultMessages.UpdateRegion);
				}
			}
			else
			{
				if (await _dbClient.UserHasRegionSelectedAsync(userId))
				{
					results.Update(await _dbClient.DeleteRegionForUserAsync(userId), ResultMessages.UpdateRegion);
				}
			}

			if (string.IsNullOrEmpty(user.SecludeUntil) && currentUser.SecludedUntil != null)
			{
				currentUser.SecludedUntil = null;
				results.Update(await _dbClient.UpdateUserAsync(currentUser), ResultMessages.RemoveSeclude);
			}
			else
			{
				if (!string.IsNullOrEmpty(user.SecludeUntil))
				{
					if (Convert.ToDateTime(user.SecludeUntil).Date != Convert.ToDateTime(currentUser.SecludedUntil).Date)
					{
						currentUser.SecludedUntil = Convert.ToDateTime(user.SecludeUntil);
						results.Update(await _dbClient.UpdateUserAsync(currentUser), ResultMessages.CreateSeclude);
					}
				}
			}

			var currentRoles = await _userManager.GetRolesAsync(currentUser);
			if (!currentRoles.SequenceEqual(user.Roles))
			{
				var removeResult = await _userManager.RemoveFromRolesAsync(currentUser, currentRoles);
				var addResult = await _userManager.AddToRolesAsync(currentUser, user.Roles);
				results.Update(removeResult.Succeeded && addResult.Succeeded, ResultMessages.UpdateRoles);
			}

			if (currentUser.FavoritePlayer != null)
			{
				if (currentUser.FavoritePlayer.PlayerId == user.FavoritePlayerId)
				{
					return results;
				}
				if (await _dbClient.UserHasAFavoritePlayerSelectedAsync(userId))
				{
					results.Update(await _dbClient.DeleteFavoritePlayerConnectionToUserAsync(userId), ResultMessages.RemovedFavoritePlayer);
				}

				if (user.FavoritePlayerId <= 0)
				{
					return results;
				}
				var favoritePlayer = new Database.Entities.User.FavoritePlayer
				{
					PlayerId = user.FavoritePlayerId,
					UserId = userId
				};
				results.Update(await _dbClient.ConnectFavoritePlayerToUserAsync(favoritePlayer), ResultMessages.CreateFavoritePlayer);
			}
			else
			{
				if (user.FavoritePlayerId <= 0)
				{
					return results;
				}
				var favoritePlayer = new Database.Entities.User.FavoritePlayer
				{
					PlayerId = user.FavoritePlayerId,
					UserId = userId
				};
				results.Update(await _dbClient.ConnectFavoritePlayerToUserAsync(favoritePlayer), ResultMessages.CreateFavoritePlayer);
			}
			return results;
		}

		public async Task<Player> GetFavoritePlayerAsync(string userId)
		{
			var favoritePlayer = await _dbClient.GetFavoritePlayerOnUserIdAsync(userId);
			return _mapper.Map<Player>(favoritePlayer);
		}

		public Task<string> GetCurrentNickOnUserIdAsync(string userId)
		{
			return _dbClient.GetCurrentNickNameOnUserIdAsync(userId);
		}

		public Task<bool> NickExistsAsync(string nickName)
		{
			return _dbClient.NickNameIsAlreadyTakenByOtherUserAsync(nickName);
		}

		public Task<bool> CreateNewNickNameAsync(string userId, string nickName)
		{
			var newNickName = new Database.Entities.User.NickName
			{
				UserId = userId,
				Nick = nickName,
				Created = DateTime.Now
			};
			return _dbClient.CreateNewNickNameForUserAsync(newNickName);
		}

		public Task<bool> SetSelectedAvatarForUserAsync(string userId, string fileName)
		{
			return _dbClient.SetSelectedAvatarFileNameForUserAsync(userId, fileName);
		}

		public async Task<List<Result>> UpdateUserFilterAsync(Filter filter)
		{
			var currentFilter = await _dbClient.GetBaseFilterForForumOnUserIdAsync(filter.UserId);
			if (currentFilter == null)
			{
				var newFilter = new Database.Entities.User.Filter
				{
					UserId = filter.UserId,
					PostsPerPage = filter.PostsPerPage,
					ExcludedUserIds = string.Join(",", filter.ExcludedUsers.Select(x => x.Key))
				};
				return await new List<Result>().Get(_dbClient.CreateBaseFilterForForumOnUserAsync(newFilter),
					ResultMessages.ChangeFilter);
			}

			currentFilter.PostsPerPage = filter.PostsPerPage;
			currentFilter.ExcludedUserIds = string.Join(",", filter.ExcludedUsers.Select(x => x.Key));

			return await new List<Result>().Get(_dbClient.UpdateBaseFilterForForumOnUserAsync(currentFilter),
				ResultMessages.ChangeFilter);
		}

		public Task<bool> EmailHasInvite(string email)
		{
			return _dbClient.AnActiveInviteExistsOnSelectedEmailAsync(email);
		}

		public async Task<Invite> GetInviteOnEmailAsync(string email)
		{
			var invite = await _dbClient.GetInviteOnUserEmailAsync(email);
			return _mapper.Map<Invite>(invite);
		}

		public async Task<List<Invite>> GetInvitesAsync()
		{
			var invites = await _dbClient.GetInvitesAsync();
			return _mapper.Map<List<Invite>>(invites);
		}

		public async Task<List<Result>> CreateInviteAsync(Models.User.CRUD.Invite invite)
		{
			if (!new EmailAddressAttribute().IsValid(invite.Email))
			{
				return new List<Result>
				{
					new Result(false, "Emailen är inte giltig")
				};
			}
			if (await EmailHasInvite(invite.Email))
			{
				return new List<Result>
				{
					new Result(false, "Det finns redan en inbjudan på denna email")
				};
			}
			return await new List<Result>().Get(_dbClient.CreateInviteAsync(_mapper.Map<Database.Entities.User.Invite>(invite)),
				ResultMessages.CreateInvite);
		}

		public async Task<bool> SetInviteAsAccountCreatedAsync(string userEmail)
		{
			var invite = await _dbClient.GetInviteOnUserEmailAsync(userEmail);
			invite.AccountCreated = DateTime.Now;
			invite.AccountIsCreated = true;
			return await _dbClient.UpdateInviteAsync(invite);
		}

		public async Task<List<Result>> SecludeUserAsync(string userId, string toDate)
		{
			var user = await _dbClient.GetUserOnIdAsync(userId);
			user.SecludedUntil = Convert.ToDateTime(toDate);
			return await new List<Result>().Get(_dbClient.UpdateUserAsync(user), ResultMessages.CreateSeclude);
		}

		public async Task<List<KeyValuePair<string, string>>> GetUsersInRoleAsKeyValuePairAsync(string roleName)
		{
			var users = await _userManager.GetUsersInRoleAsync(roleName);
			return users.Select(user => new KeyValuePair<string, string>(user.Id, user.NickNames.Current())).ToList();
		}

		public async Task<List<Result>> UpdateAvatarFileNameForUserWithIdAsync(string userId, string fileName)
		{
			var user = await _userManager.Users.Include(x => x.NickNames).Include(x => x.FavoritePlayer).FirstOrDefaultAsync(x => x.Id == userId);
			user.AvatarFileName = fileName;
			var results = await new List<Result>().Get(_dbClient.UpdateUserAsync(user), ResultMessages.ChangeAvatar);
			if (results.Any(x => x.Type == ResultType.Success))
			{
				_uploadRepository.DeleteFilesInDirectory("avatars", x => x.Name.Contains(userId) && !x.Name.Equals(fileName));
			}
			return results;
		}

		public async Task<List<Result>> ResetUsersAvatarToGenericAsync(string userId)
		{
			var user = await _userManager.Users.Include(x => x.NickNames).Include(x => x.FavoritePlayer).FirstOrDefaultAsync(x => x.Id == userId);
			user.AvatarFileName = null;
			var results = await new List<Result>().Get(_dbClient.UpdateUserAsync(user), ResultMessages.ChangeAvatar);
			if (results.Any(x => x.Type == ResultType.Success))
			{
				_uploadRepository.DeleteFilesInDirectory("avatars", x => x.Name.Contains(userId));
			}
			return results;
		}

		public Task<bool> CreateRegionToUserAsync(string userId, int regionId)
		{
			return _dbClient.CreateRegionToUserAsync(userId, regionId);
		}
	}
}
