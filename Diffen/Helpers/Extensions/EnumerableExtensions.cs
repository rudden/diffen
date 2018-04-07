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
	}
}
