using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace Diffen.Helpers.Extensions
{
	using Database.Entities.User;
	using Database.Entities.Forum;
	using Database.Entities.Squad;
	using Database.Entities.Other;

	public static class QueryableExtensions
	{
		public static IQueryable<Post> IncludeAll(this DbSet<Post> source)
		{
			return source
				.Include(x => x.User).ThenInclude(x => x.NickNames)
				.Include(x => x.User).ThenInclude(x => x.SavedPosts)
				.Include(x => x.Votes).ThenInclude(x => x.User).ThenInclude(x => x.NickNames)
				.Include(x => x.Lineups)
				.Include(x => x.UrlTips)
				.Include(x => x.ParentPost).ThenInclude(x => x.User).ThenInclude(x => x.NickNames)
				.Include(x => x.InThreads).ThenInclude(x => x.Thread)
				.AsNoTracking();
		}

		public static IQueryable<Post> IncludeAllExceptParent(this DbSet<Post> source)
		{
			return source
				.Include(x => x.User).ThenInclude(x => x.NickNames)
				.Include(x => x.User).ThenInclude(x => x.SavedPosts)
				.Include(x => x.Votes).ThenInclude(x => x.User).ThenInclude(x => x.NickNames)
				.Include(x => x.Lineups)
				.Include(x => x.UrlTips)
				.Include(x => x.InThreads).ThenInclude(x => x.Thread)
				.AsNoTracking();
		}

		public static IQueryable<SavedPost> IncludeAll(this DbSet<SavedPost> source)
		{
			return source
				.Include(x => x.Post).ThenInclude(x => x.User).ThenInclude(x => x.NickNames)
				.Include(x => x.Post).ThenInclude(x => x.Votes).ThenInclude(x => x.User).ThenInclude(x => x.NickNames)
				.Include(x => x.Post).ThenInclude(x => x.Lineups)
				.Include(x => x.Post).ThenInclude(x => x.UrlTips)
				.Include(x => x.Post).ThenInclude(x => x.ParentPost).ThenInclude(x => x.User).ThenInclude(x => x.NickNames)
				.AsNoTracking();
		}

		public static IQueryable<Post> ExceptScissored(this IQueryable<Post> source)
		{
			return source.Where(x => x.Scissored == null);
		}

		public static IOrderedQueryable<Post> OrderByCreated(this IQueryable<Post> source)
		{
			return source.OrderByDescending(x => x.Created);
		}

		public static IQueryable<Lineup> IncludeAll(this DbSet<Lineup> source)
		{
			return source
				.Include(x => x.Players).ThenInclude(x => x.Position)
				.Include(x => x.Players).ThenInclude(x => x.Player).ThenInclude(x => x.AvailablePositions).ThenInclude(x => x.Position)
				.Include(x => x.Formation)
				.AsNoTracking();
		}

		public static IQueryable<Player> IncludeAll(this DbSet<Player> source)
		{
			return source
				.Include(x => x.AvailablePositions).ThenInclude(x => x.Position)
				.Include(x => x.InLineups)
				.Include(x => x.PlayerEvents).ThenInclude(x => x.Game)
				.AsNoTracking();
		}

		public static IQueryable<AppUser> IncludeAll(this DbSet<AppUser> source)
		{
			return source
				.Include(x => x.NickNames)
				.Include(x => x.SavedPosts).ThenInclude(x => x.Post)
				.Include(x => x.FavoritePlayer).ThenInclude(x => x.Player)
				.Include(x => x.Filter)
				.Include(x => x.Votes).ThenInclude(x => x.User).ThenInclude(x => x.NickNames)
				.Include(x => x.Posts).ThenInclude(x => x.UrlTips)
				.Include(x => x.Posts).ThenInclude(x => x.Lineups)
				.Include(x => x.Posts).ThenInclude(x => x.Votes).ThenInclude(x => x.User).ThenInclude(x => x.NickNames)
				.Include(x => x.Lineups)
				.Include(x => x.Region).ThenInclude(x => x.Region)
				.Include(x => x.GameResultGuesses).ThenInclude(x => x.Game).ThenInclude(x => x.PlayerEvents).ThenInclude(x => x.Player)
				.Include(x => x.GameResultGuesses).ThenInclude(x => x.Game).ThenInclude(x => x.Lineup).ThenInclude(x => x.Players).ThenInclude(x => x.Position)
				.Include(x => x.GameResultGuesses).ThenInclude(x => x.Game).ThenInclude(x => x.Lineup).ThenInclude(x => x.Players).ThenInclude(x => x.Player).ThenInclude(x => x.AvailablePositions).ThenInclude(x => x.Position)
				.Include(x => x.GameResultGuesses).ThenInclude(x => x.Game).ThenInclude(x => x.Lineup).ThenInclude(x => x.Formation)
				.AsNoTracking();
		}

		public static IQueryable<PersonalMessage> IncludeAll(this DbSet<PersonalMessage> source)
		{
			return source
				.Include(x => x.FromUser).ThenInclude(x => x.NickNames)
				.Include(x => x.ToUser).ThenInclude(x => x.NickNames)
				.AsNoTracking();
		}

		public static IQueryable<Poll> IncludeAll(this DbSet<Poll> source)
		{
			return source
				.Include(x => x.CreatedByUser).ThenInclude(x => x.NickNames)
				.Include(x => x.Selections).ThenInclude(x => x.Poll)
				.Include(x => x.Selections).ThenInclude(x => x.Votes).ThenInclude(x => x.VotedByUser).ThenInclude(x => x.NickNames);
		}

		public static IQueryable<Chronicle> IncludeAll(this DbSet<Chronicle> source)
		{
			return source
				.Include(x => x.WrittenByUser).ThenInclude(x => x.NickNames)
				.Include(x => x.Categories).ThenInclude(x => x.Category);
		}

		public static IQueryable<Region> IncludeAll(this DbSet<Region> source)
		{
			return source.Include(x => x.UsersInRegion).ThenInclude(x => x.User).ThenInclude(x => x.NickNames);
		}

		public static IQueryable<Game> IncludeAll(this DbSet<Game> source)
		{
			return source
				.Include(x => x.PlayerEvents).ThenInclude(x => x.Player)
				.Include(x => x.Lineup).ThenInclude(x => x.Players).ThenInclude(x => x.Position)
				.Include(x => x.Lineup).ThenInclude(x => x.Players).ThenInclude(x => x.Player).ThenInclude(x => x.AvailablePositions).ThenInclude(x => x.Position)
				.Include(x => x.Lineup).ThenInclude(x => x.Formation)
				.Include(x => x.GameResultGuesses).ThenInclude(x => x.GuessedByUser).ThenInclude(x => x.NickNames);
		}

		public static IQueryable<PlayerEvent> IncludeAll(this DbSet<PlayerEvent> source)
		{
			return source.Include(x => x.Player).ThenInclude(x => x.AvailablePositions).ThenInclude(x => x.Position)
				.Include(x => x.Player).ThenInclude(x => x.InLineups);
		}

		public static IQueryable<GameResultGuess> IncludeAll(this DbSet<GameResultGuess> source)
		{
			return source
				.Include(x => x.Game).ThenInclude(x => x.PlayerEvents).ThenInclude(x => x.Player)
				.Include(x => x.GuessedByUser).ThenInclude(x => x.NickNames);
		}
	}
}
