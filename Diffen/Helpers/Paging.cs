using System.Collections.Generic;

namespace Diffen.Helpers
{
	public class Paging<T>
	{
		public IEnumerable<T> Data { get; set; }
		public int CurrentPage { get; set; }
		public int NumberOfPages { get; set; }
		public int Total { get; set; }

		public Paging() { }
		public Paging(IEnumerable<T> data)
		{
			Data = data;
		}
	}
}
