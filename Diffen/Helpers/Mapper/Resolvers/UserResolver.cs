using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;

using AutoMapper;

namespace Diffen.Helpers.Mapper.Resolvers
{
	using Enum;
	using Extensions;
	using Models.User;
	using Repositories.Contracts;

	public class UserResolver :
		ITypeConverter<Database.Entities.User.AppUser, User>,
		ITypeConverter<Database.Entities.User.PersonalMessage, PersonalMessage>,
		ITypeConverter<Models.User.CRUD.PersonalMessage, Database.Entities.User.PersonalMessage>,
		ITypeConverter<Database.Entities.User.AppUser, Models.Forum.User>,
		ITypeConverter<Database.Entities.User.Filter, Filter>,
		ITypeConverter<Database.Entities.User.Invite, Invite>,
		ITypeConverter<Models.User.CRUD.Invite, Database.Entities.User.Invite>
	{
		private readonly UserManager<Database.Entities.User.AppUser> _userManager;
		private readonly IUserRepository _userRepository;

		private const string BasePathForAvatars = "/uploads/avatars/";
		private readonly string _genericAvatarPath;

		public UserResolver(UserManager<Database.Entities.User.AppUser> userManager, IUserRepository userRepository)
		{
			_userManager = userManager;
			_userRepository = userRepository;
			_genericAvatarPath = string.Concat(BasePathForAvatars, "generic/logo.png");
		}

		public User Convert(Database.Entities.User.AppUser source, User destination, ResolutionContext context)
		{
			return new User
			{
				Id = source.Id,
				Bio = source.Bio,
				Email = source.Email,
				NickName = source.NickNames.Current() ?? "anonymous",
				Avatar = GetAvatar(source),
				Region = source.Region?.Region?.Name,
				Karma = source.Posts != null ? GetKarma(source.Posts) : 0,
				NumberOfPosts = source.Posts?.Count ?? 0,
				Filter = context.Mapper.Map<Filter>(source.Filter) ?? new Filter(source),
				FavoritePlayer = source.FavoritePlayer != null ? context.Mapper.Map<Models.Squad.Player>(source.FavoritePlayer) : null,
				SavedPostsIds = source.SavedPosts?.Select(p => p.PostId),
				InRoles = _userManager.GetRolesAsync(source).Result,
				VoteStatistics = source.Votes != null ? new VoteStatistics
				{
					UpVotes = source.Votes.Count(x => x.Type == VoteType.Up),
					DownVotes = source.Votes.Count(x => x.Type == VoteType.Down)
				} : null,
				Joined = source.Joined.ToString("yyyy-MM-dd"),
				SecludedUntil = source.SecludedUntil.GetSecluded(),
			};
		}

		public PersonalMessage Convert(Database.Entities.User.PersonalMessage source, PersonalMessage destination, ResolutionContext context)
		{
			return new PersonalMessage
			{
				Id = source.Id,
				From = new PmUser
				{
					Id = source.FromUserId,
					Avatar = GetAvatar(source.FromUser),
					NickName = source.FromUser.NickNames.Current() ?? "anonymous"
				},
				To = new PmUser
				{
					Id = source.ToUserId,
					Avatar = GetAvatar(source.ToUser),
					NickName = source.ToUser.NickNames.Current() ?? "anonymous"
				},
				Message = source.Message,
				Since = source.Created.GetSinceStamp()
			};
		}

		public Database.Entities.User.PersonalMessage Convert(Models.User.CRUD.PersonalMessage source, Database.Entities.User.PersonalMessage destination, ResolutionContext context)
		{
			return new Database.Entities.User.PersonalMessage
			{
				FromUserId = source.FromUserId,
				ToUserId = source.ToUserId,
				Message = source.Message,
				Created = DateTime.Now
			};
		}

		public Models.Forum.User Convert(Database.Entities.User.AppUser source, Models.Forum.User destination, ResolutionContext context)
		{
			return new Models.Forum.User
			{
				Id = source.Id,
				NickName = source.NickNames.Current() ?? "anonymous",
				Avatar = GetAvatar(source),
				IsAdmin = _userManager.GetRolesAsync(source).Result.Any(role => role.Equals("Admin") || role.Equals("Scissor")),
				SecludedUntil = source.SecludedUntil.GetSecluded()
			};
		}

		public Filter Convert(Database.Entities.User.Filter source, Filter destination, ResolutionContext context)
		{
			return new Filter
			{
				UserId = source.UserId,
				PostsPerPage = source.PostsPerPage,
				HideLeftMenu = source.HideLeftMenu,
				HideRightMenu = source.HideRightMenu,
				ExcludedUsers = !string.IsNullOrEmpty(source.ExcludedUserIds) ? source.ExcludedUserIds?.Split(",")
					.Select(userId => new KeyValuePair<string, string>(userId, _userRepository.GetCurrentNickOnUserIdAsync(userId).Result)) : new List<KeyValuePair<string, string>>()
			};
		}

		public Invite Convert(Database.Entities.User.Invite source, Invite destination, ResolutionContext context)
		{
			return new Invite
			{
				UniqueCode = source.UniqueCode,
				InvitedBy = new InvitedBy
				{
					Id = source.InvitedByUser.Id,
					NickName = source.InvitedByUser.NickNames.Current() ?? "anonymous"
				},
				InviteUsedBy = new InvitedBy
				{
					Id = source.InviteUsedByUser?.Id,
					NickName = source.InviteUsedByUser?.NickNames.Current() ?? "anonymous"
				},
				AccountIsCreated = source.AccountIsCreated,
				InviteSent = source.InviteSent.ToString("yyyy-MM-dd"),
				AccountCreated = source.AccountCreated != null
					? System.Convert.ToDateTime(source.AccountCreated).ToString("yyyy-MM-dd")
					: string.Empty
			};
		}

		public Database.Entities.User.Invite Convert(Models.User.CRUD.Invite source, Database.Entities.User.Invite destination, ResolutionContext context)
		{
			return new Database.Entities.User.Invite
			{
				UniqueCode = Guid.NewGuid().ToString(),
				InvitedByUserId = source.InvitedByUserId
			};
		}

		private static int GetKarma(IEnumerable<Database.Entities.Forum.Post> posts)
		{
			var karma = 0;
			foreach (var post in posts)
			{
				foreach (var vote in post.Votes)
				{
					switch (vote.Type)
					{
						case VoteType.Up:
							karma++;
							break;
						case VoteType.Down:
							karma--;
							break;
					}
				}
			}
			return karma;
		}

		private string GetAvatar(Database.Entities.User.AppUser source)
		{
			return string.IsNullOrEmpty(source.AvatarFileName) 
				? _genericAvatarPath 
				: string.Concat(BasePathForAvatars, source.AvatarFileName);
		}
	}
}
