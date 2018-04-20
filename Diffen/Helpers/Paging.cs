using System;
using System.Collections.Generic;

namespace Diffen.Helpers
{
	public class Paging<T>
	{
		public IEnumerable<T> Data { get; set; }
		public int CurrentPage { get; set; }
		public int NumberOfPages { get; set; }
		public int Total { get; set; }
	}

	public static class PagingExtensions
	{
		public static Paging<T> ToPaging<T>(this IEnumerable<T> data, int total, int pageNumber, int pageSize)
		{
			return new Paging<T>
			{
				Data = data,
				NumberOfPages = Convert.ToInt32(Math.Ceiling((double)total / pageSize)),
				CurrentPage = pageNumber,
				Total = total
			};
		}
	}
}
