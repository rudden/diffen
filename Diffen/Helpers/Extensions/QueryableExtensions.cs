using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace Diffen.Helpers.Extensions
{
	using Database.Entities.User;
	using Database.Entities.Forum;
	using Database.Entities.Squad;

	public static class QueryableExtensions
	{
		public static IQueryable<Post> IncludeAll(this DbSet<Post> source)
		{
			return source
				.Include(x => x.User).ThenInclude(x => x.NickNames)
				.Include(x => x.User).ThenInclude(x => x.SavedPosts)
				.Include(x => x.Votes).ThenInclude(x => x.User).ThenInclude(x => x.NickNames)
				.Include(x => x.Lineup)
				.Include(x => x.UrlTip)
				.AsNoTracking();
		}

		public static IOrderedQueryable<Post> OrderByCreated(this IQueryable<Post> source)
		{
			return source.OrderByDescending(x => x.Created);
		}

		public static IQueryable<Lineup> IncludeAll(this DbSet<Lineup> source)
		{
			return source
				.Include(x => x.Players).ThenInclude(x => x.Player)
				.Include(x => x.Players).ThenInclude(x => x.Position)
				.Include(x => x.Formation)
				.AsNoTracking();
		}

		public static IQueryable<AppUser> IncludeAll(this DbSet<AppUser> source)
		{
			return source
				.Include(x => x.NickNames)
				.Include(x => x.SavedPosts).ThenInclude(x => x.Post)
				.Include(x => x.FavoritePlayer).ThenInclude(x => x.Player)
				.Include(x => x.Filter)
				.Include(x => x.Votes)
				.Include(x => x.Posts).ThenInclude(x => x.UrlTip)
				.Include(x => x.Posts).ThenInclude(x => x.Lineup)
				.Include(x => x.Lineups)
				.AsNoTracking();
		}

		public static IQueryable<PersonalMessage> IncludeAll(this DbSet<PersonalMessage> source)
		{
			return source
				.Include(x => x.FromUser).ThenInclude(x => x.NickNames)
				.Include(x => x.ToUser).ThenInclude(x => x.NickNames);
		}
	}
}
