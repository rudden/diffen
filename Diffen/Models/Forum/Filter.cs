using System;
using System.Collections.Generic;

namespace Diffen.Models.Forum
{
	public class Filter
	{
		public DateTime? FromDate { get; set; }
		public DateTime? ToDate { get; set; }
		public string MessageWildCard { get; set; }
		public StartingEleven StartingEleven { get; set; }
		public IEnumerable<KeyValuePair<string, string>> IncludedUsers { get; set; }
		public IEnumerable<KeyValuePair<string, string>> ExcludedUsers { get; set; }
		public IEnumerable<int> ThreadIds { get; set; }
	}

	public enum StartingEleven
	{
		All,
		With,
		Without
	}
}
