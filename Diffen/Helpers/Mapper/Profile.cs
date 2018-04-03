namespace Diffen.Helpers.Mapper
{
	using Resolvers;

	using DbPost = Database.Entities.Forum.Post;
	using DbVote = Database.Entities.Forum.Vote;
	using DbUser = Database.Entities.User.AppUser;
	using DbPersonalMessage = Database.Entities.User.PersonalMessage;
	using DbFilter = Database.Entities.User.Filter;
	using DbInvite = Database.Entities.User.Invite;
	using DbPlayer = Database.Entities.Squad.Player;
	using DbLineup = Database.Entities.Squad.Lineup;
	using DbPlayerToLineup = Database.Entities.Squad.PlayerToLineup;

	using ModelPost = Models.Forum.Post;
	using ModelParentPost = Models.Forum.ParentPost;
	using ModelVote = Models.Forum.Vote;
	using ModelPostUser = Models.Forum.User;
	using ModelUser = Models.User.User;
	using ModelPersonalMessage = Models.User.PersonalMessage;
	using ModelFilter = Models.User.Filter;
	using ModelInvite = Models.User.Invite;
	using ModelPlayer = Models.Squad.Player;
	using ModelLineup = Models.Squad.Lineup;
	using ModelPlayerToLineup = Models.Squad.PlayerToLineup;

	public class Profile : AutoMapper.Profile
	{
		public Profile()
		{
			CreateMap<DbPost, ModelPost>().ConvertUsing<PostResolver>();
			CreateMap<DbVote, ModelVote>().ConvertUsing<PostResolver>();
			CreateMap<DbPost, ModelParentPost>().ConvertUsing<PostResolver>();
			CreateMap<DbUser, ModelUser>().ConvertUsing<UserResolver>();
			CreateMap<DbUser, ModelPostUser>().ConvertUsing<UserResolver>();
			CreateMap<DbPersonalMessage, ModelPersonalMessage>().ConvertUsing<UserResolver>();
			CreateMap<DbFilter, ModelFilter>().ConvertUsing<UserResolver>();
			CreateMap<DbInvite, ModelInvite>().ConvertUsing<UserResolver>();
			CreateMap<DbPlayer, ModelPlayer>().ConvertUsing<SquadResolver>();
			CreateMap<DbLineup, ModelLineup>().ConvertUsing<SquadResolver>();
			CreateMap<DbPlayerToLineup, ModelPlayerToLineup>().ConvertUsing<SquadResolver>();
		}
	}
}
