using System.Linq;
using System.Collections.Generic;

using AutoMapper;

namespace Diffen.Helpers.Mapper.Resolvers
{
	using Extensions;
	using Models.User;

	using DbUser = Database.Entities.User.AppUser;
	using DbPersonalMessage = Database.Entities.User.PersonalMessage;
	using DbFilter = Database.Entities.User.Filter;
	using DbInvite = Database.Entities.User.Invite;

	using ModelUser = Models.User.User;
	using ModelFilter = Models.User.Filter;
	using ModelInvite = Models.User.Invite;
	using ModelPersonalMessage = Models.User.PersonalMessage;
	using ModelPlayer = Models.Squad.Player;
	using ModelPostUser = Models.Forum.User;

	public class UserResolver : 
		ITypeConverter<DbUser, ModelUser>, 
		ITypeConverter<DbPersonalMessage, ModelPersonalMessage>, 
		ITypeConverter<DbUser, ModelPostUser>, 
		ITypeConverter<DbFilter, ModelFilter>, 
		ITypeConverter<DbInvite, ModelInvite>
	{
		public ModelUser Convert(DbUser source, ModelUser destination, ResolutionContext context)
		{
			return new ModelUser
			{
				Id = source.Id,
				Bio = source.Bio,
				NickName = source.NickNames.OrderByDescending(x => x.Created).FirstOrDefault()?.Nick ?? "anonymous",
				Avatar = "",
				Filter = context.Mapper.Map<ModelFilter>(source.Filter),
				FavoritePlayer = context.Mapper.Map<ModelPlayer>(source.FavoritePlayer),
				SavedPostsIds = source.SavedPosts?.Select(p => p.Id),
				InRoles = source.Roles.Select(r => r.Name),
				SecludedUntil = source.SecludedUntil.GetSecluded(),
				HasCreatedPosts = source.Posts != null,
				HasCreatedLineups = source.Lineups != null
			};
		}

		public ModelPersonalMessage Convert(DbPersonalMessage source, ModelPersonalMessage destination, ResolutionContext context)
		{
			return new ModelPersonalMessage
			{
				Id = source.Id,
				From = new From
				{
					Avatar = "",
					NickName = source.FromUser.NickNames.OrderByDescending(x => x.Created).FirstOrDefault()?.Nick ?? "anonymous"
				},
				Message = source.Message,
				Since = source.Created.GetSinceStamp()
			};
		}

		public ModelPostUser Convert(DbUser source, ModelPostUser destination, ResolutionContext context)
		{
			return new ModelPostUser
			{
				Id = source.Id,
				NickName = source.NickNames.OrderByDescending(x => x.Created).FirstOrDefault()?.Nick ?? "anonymous",
				Avatar = "",
				IsAdmin = source.Roles.Any(r => r.Name == "Admin" || r.Name == "Sax"),
				SecludedUntil = source.SecludedUntil.GetSecluded()
			};
		}

		public ModelFilter Convert(DbFilter source, ModelFilter destination, ResolutionContext context)
		{
			return new ModelFilter
			{
				UserId = source.UserId,
				PostsPerPage = source.PostsPerPage,
				ExcludedUsers = source.ExcludedUserIds.Split(",")
					.Select(userId => new KeyValuePair<string, string>(userId, "GetNickFromRepositoryOnUserId"))
			};
		}

		public ModelInvite Convert(DbInvite source, ModelInvite destination, ResolutionContext context)
		{
			return new ModelInvite
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
	}
}
