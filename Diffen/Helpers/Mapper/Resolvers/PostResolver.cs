using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using AutoMapper;

namespace Diffen.Helpers.Mapper.Resolvers
{
	using Extensions;

	using DbPost = Database.Entities.Forum.Post;
	using DbPostToPost = Database.Entities.Forum.PostToPost;
	using DbVote = Database.Entities.Forum.Vote;
	using DbUser = Database.Entities.User.AppUser;

	using ModelPost = Models.Forum.Post;
	using ModelVote = Models.Forum.Vote;
	using ModelUser = Models.Forum.User;
	using ModelParentPost = Models.Forum.ParentPost;

	public class PostResolver : 
		ITypeConverter<DbPost, ModelPost>,
		ITypeConverter<DbPostToPost, ModelParentPost>, 
		ITypeConverter<DbVote, ModelVote>
	{
		private readonly string _loggedInUserId;

		public PostResolver(UserManager<DbUser> userManager, IHttpContextAccessor httpContextAccessor)
		{
			_loggedInUserId = userManager.GetUserId(httpContextAccessor.HttpContext.User);
		}

		public ModelPost Convert(DbPost source, ModelPost destination, ResolutionContext context)
		{
			return new ModelPost
			{
				Id = source.Id,
				Message = source.Message,
				User = context.Mapper.Map<ModelUser>(source.User),
				UrlTipHref = source.UrlTip?.Href,
				Votes = context.Mapper.Map<IEnumerable<ModelVote>>(source.Votes),
				Since = source.Created.GetSinceStamp(),
				Edited = source.Edited.GetSinceStamp(),
				HasLineup = source.Lineup != null,
				IsPartOfConversation = source.Conversation != null,
				IsScissored = source.Scissored != null,
				LoggedInUserCanVote = source.User.Id != _loggedInUserId && source.Votes.All(v => v.CreatedByUserId != _loggedInUserId)
			};
		}

		public ModelParentPost Convert(DbPostToPost source, ModelParentPost destination, ResolutionContext context)
		{
			if (source.ParentPostId == null)
				return null;

			return new ModelParentPost
			{
				Id = (int) source.ParentPostId,
				Message = source.ParentPost.Message,
				User = context.Mapper.Map<ModelUser>(source.ParentPost.User),
				Since = source.ParentPost.Created.GetSinceStamp()
			};
		}

		public ModelVote Convert(DbVote source, ModelVote destination, ResolutionContext context)
		{
			return new ModelVote
			{
				Type = source.Type,
				ByNickName = source.User.NickNames.OrderByDescending(x => x.Created).FirstOrDefault()?.Nick ?? "anonymous"
			};
		}
	}
}
