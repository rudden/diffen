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
			CreateMap<Database.Entities.Forum.UrlTip, string>().ConvertUsing<PostResolver>();
			CreateMap<Database.Entities.User.AppUser, Models.User.User>().ConvertUsing<UserResolver>();
			CreateMap<Database.Entities.User.AppUser, Models.Forum.User>().ConvertUsing<UserResolver>();
			CreateMap<Database.Entities.User.PersonalMessage, Models.User.PersonalMessage>().ConvertUsing<UserResolver>();
			CreateMap<Models.User.CRUD.PersonalMessage, Database.Entities.User.PersonalMessage>().ConvertUsing<UserResolver>();
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
			CreateMap<Database.Entities.Other.Poll, Models.Other.Poll>().ConvertUsing<PollResolver>();
			CreateMap<Database.Entities.Other.PollSelection, Models.Other.PollSelection>().ConvertUsing<PollResolver>();
			CreateMap<Models.Other.CRUD.PollVote, Database.Entities.Other.PollVote>().ConvertUsing<PollResolver>();
			CreateMap<Models.Other.CRUD.Poll, Database.Entities.Other.Poll>().ConvertUsing<PollResolver>();
			CreateMap<string, Database.Entities.Other.PollSelection>().ConvertUsing<PollResolver>();
			CreateMap<Database.Entities.Other.Chronicle, Models.Other.Chronicle>().ConvertUsing<ChronicleResolver>();
			CreateMap<Models.Other.CRUD.Chronicle, Database.Entities.Other.Chronicle>().ConvertUsing<ChronicleResolver>();
			CreateMap<Database.Entities.Other.Region, Models.Other.Region>().ConvertUsing<RegionResolver>();
			CreateMap<Models.Other.CRUD.Region, Database.Entities.Other.Region>().ConvertUsing<RegionResolver>();
		}
	}
}
