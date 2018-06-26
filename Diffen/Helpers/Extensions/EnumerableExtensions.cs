using System;
using System.Linq;
using System.Collections.Generic;

namespace Diffen.Helpers.Extensions
{
	public static class EnumerableExtensions
	{
		public static T PickRandom<T>(this IEnumerable<T> source)
		{
			return source.PickRandom(1).Single();
		}

		public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
		{
			return source.Shuffle().Take(count);
		}

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
		{
			return source.OrderBy(x => Guid.NewGuid());
		}

		public static IEnumerable<T> Page<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
		{
			return source.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
		}

		public static IQueryable<T> PageAsQueryable<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
		{
			return source.Skip(pageSize * (pageNumber - 1)).Take(pageSize).AsQueryable();
		}

		public static string Current(this IEnumerable<Database.Entities.User.NickName> nickNames)
		{
			return nickNames.OrderByDescending(x => x.Created).FirstOrDefault()?.Nick;
		}

		public static Database.Entities.Forum.UrlTip Current(this IEnumerable<Database.Entities.Forum.UrlTip> urlTips)
		{
			return urlTips.OrderByDescending(x => x.Created).FirstOrDefault();
		}

		public static Database.Entities.Forum.PostToLineup Current(this IEnumerable<Database.Entities.Forum.PostToLineup> lineups)
		{
			return lineups.OrderByDescending(x => x.Created).FirstOrDefault();
		}
	}
}
