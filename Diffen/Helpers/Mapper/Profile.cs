namespace Diffen.Helpers.Mapper
{
	using Resolvers;

	public class Profile : AutoMapper.Profile
	{
		public Profile()
		{
			CreateMap<Database.Entities.Forum.Post, Models.Forum.Post>().ConvertUsing<PostResolver>();
			CreateMap<Database.Entities.Forum.Vote, Models.Forum.Vote>().ConvertUsing<PostResolver>();
			CreateMap<Database.Entities.Forum.Post, Models.Forum.ParentPost>().ConvertUsing<PostResolver>();
			CreateMap<Database.Entities.User.AppUser, Models.User.User>().ConvertUsing<UserResolver>();
			CreateMap<Database.Entities.User.AppUser, Models.Forum.User>().ConvertUsing<UserResolver>();
			CreateMap<Database.Entities.User.PersonalMessage, Models.User.PersonalMessage>().ConvertUsing<UserResolver>();
			CreateMap<Database.Entities.User.Filter, Models.User.Filter>().ConvertUsing<UserResolver>();
			CreateMap<Database.Entities.User.Invite, Models.User.Invite>().ConvertUsing<UserResolver>();
			CreateMap<Database.Entities.Squad.Player, Models.Squad.Player>().ConvertUsing<SquadResolver>();
			CreateMap<Database.Entities.Squad.Lineup, Models.Squad.Lineup>().ConvertUsing<SquadResolver>();
			CreateMap<Database.Entities.Squad.PlayerToLineup, Models.Squad.PlayerToLineup>().ConvertUsing<SquadResolver>();
			CreateMap<Database.Entities.User.FavoritePlayer, Models.Squad.Player>().ConvertUsing<SquadResolver>();
			CreateMap<Models.Forum.CRUD.Post, Database.Entities.Forum.Post>().ConvertUsing<PostResolver>();
			CreateMap<Models.Forum.CRUD.Vote, Database.Entities.Forum.Vote>().ConvertUsing<PostResolver>();
			CreateMap<Models.Squad.CRUD.Lineup, Database.Entities.Squad.Lineup>().ConvertUsing<SquadResolver>();
			CreateMap<Models.Squad.CRUD.Player, Database.Entities.Squad.Player>().ConvertUsing<SquadResolver>();
			CreateMap<Database.Entities.User.AppUser, ViewModels.LoggedInUser>().ConvertUsing<UserResolver>();
		}
	}
}
