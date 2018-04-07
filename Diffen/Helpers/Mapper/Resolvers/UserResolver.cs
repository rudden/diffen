using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Hosting;
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
		ITypeConverter<Database.Entities.User.AppUser, Models.Forum.User>,
		ITypeConverter<Database.Entities.User.Filter, Filter>,
		ITypeConverter<Database.Entities.User.Invite, Invite>, 
		ITypeConverter<Database.Entities.User.AppUser, ViewModels.LoggedInUser>
	{
		private readonly UserManager<Database.Entities.User.AppUser> _userManager;
		private readonly IUserRepository _userRepository;

		private readonly IHostingEnvironment _environment;

		private const string GenericAvatarPath = "/uploads/avatars/generic/logo.png";

		public UserResolver(UserManager<Database.Entities.User.AppUser> userManager, IUserRepository userRepository, IHostingEnvironment environment)
		{
			_userManager = userManager;
			_userRepository = userRepository;
			_environment = environment;
		}

		public User Convert(Database.Entities.User.AppUser source, User destination, ResolutionContext context)
		{
			return new User
			{
				Id = source.Id,
				Bio = source.Bio,
				NickName = source.NickNames.OrderByDescending(x => x.Created).FirstOrDefault()?.Nick ?? "anonymous",
				Avatar = GetAvatar(source),
				Karma = GetKarma(source.Posts),
				NumberOfPosts = source.Posts.Count,
				Filter = context.Mapper.Map<Filter>(source.Filter) ?? new Filter(source),
				FavoritePlayer = context.Mapper.Map<Models.Squad.Player>(source.FavoritePlayer),
				SavedPostsIds = source.SavedPosts?.Select(p => p.PostId),
				InRoles = _userManager.GetRolesAsync(source).Result,
				VoteStatistics = new VoteStatistics
				{
					UpVotes = source.Votes.Count(x => x.Type == VoteType.Up),
					DownVotes = source.Votes.Count(x => x.Type == VoteType.Down)
				},
				SecludedUntil = source.SecludedUntil.GetSecluded(),
			};
		}

		public PersonalMessage Convert(Database.Entities.User.PersonalMessage source, PersonalMessage destination, ResolutionContext context)
		{
			return new PersonalMessage
			{
				Id = source.Id,
				From = new From
				{
					Avatar = GetAvatar(source.FromUser),
					NickName = source.FromUser.NickNames.OrderByDescending(x => x.Created).FirstOrDefault()?.Nick ?? "anonymous"
				},
				Message = source.Message,
				Since = source.Created.GetSinceStamp()
			};
		}

		public Models.Forum.User Convert(Database.Entities.User.AppUser source, Models.Forum.User destination, ResolutionContext context)
		{
			return new Models.Forum.User
			{
				Id = source.Id,
				NickName = source.NickNames.OrderByDescending(x => x.Created).FirstOrDefault()?.Nick ?? "anonymous",
				Avatar = GetAvatar(source),
				IsAdmin = _userManager.GetRolesAsync(source).Result.Any(role => role.Equals("Admin") || role.Equals("Sax")),
				SecludedUntil = source.SecludedUntil.GetSecluded()
			};
		}

		public Filter Convert(Database.Entities.User.Filter source, Filter destination, ResolutionContext context)
		{
			return new Filter
			{
				UserId = source.UserId,
				PostsPerPage = source.PostsPerPage,
				ExcludedUsers = source.ExcludedUserIds?.Split(",")
					.Select(userId => new KeyValuePair<string, string>(userId, _userRepository.GetCurrentNickOnUserIdAsync(userId).Result))
			};
		}

		public Invite Convert(Database.Entities.User.Invite source, Invite destination, ResolutionContext context)
		{
			return new Invite
			{
				Email = source.Email,
				InvitedBy = new InvitedBy
				{
					Id = source.InvitedByUser.Id,
					NickName = source.InvitedByUser.NickNames.OrderByDescending(x => x.Created).FirstOrDefault()?.Nick ?? "anonymous"
				},
				AccountIsCreated = source.AccountIsCreated,
				InviteSent = source.InviteSent.ToString("yyyy-MM-dd"),
				AccountCreated = source.AccountCreated != null
					? System.Convert.ToDateTime(source.AccountCreated).ToString("yyyy-MM-dd")
					: string.Empty
			};
		}

		public ViewModels.LoggedInUser Convert(Database.Entities.User.AppUser source, ViewModels.LoggedInUser destination, ResolutionContext context)
		{
			return new ViewModels.LoggedInUser
			{
				Id = source.Id,
				Name = source.UserName,
				Nick = source.NickNames.OrderByDescending(x => x.Created).FirstOrDefault()?.Nick ?? "anonymous",
				AvatarFileName = GetAvatar(source),
				Bio = source.Bio,
				InRoles = _userManager.GetRolesAsync(source).Result,
				SecludedUntil = source.SecludedUntil.ToString("yyyy-MM-dd"),
				Filter = context.Mapper.Map<Models.User.Filter>(source.Filter) ?? new Models.User.Filter(source)
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
			if (string.IsNullOrEmpty(source.AvatarFileName))
			{
				return GenericAvatarPath;
			}
			var userPath = $"uploads\\avatars\\{source.Id}";
			var path = Path.Combine(_environment.WebRootPath, userPath);
			if (!Directory.Exists(path))
			{
				return GenericAvatarPath;
			}
			var files = Directory.GetFiles(Path.Combine(_environment.WebRootPath, userPath));
			if (files == null || !files.Any())
			{
				return GenericAvatarPath;
			}
			return $"/uploads/avatars/{source.Id}/{source.AvatarFileName}";
		}
	}
}
