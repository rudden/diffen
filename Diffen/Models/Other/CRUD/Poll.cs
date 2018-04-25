using System.Collections.Generic;

namespace Diffen.Models.Other.CRUD
{
	public class Poll
	{
		public string Name { get; set; }
		public IEnumerable<string> Selections { get; set; }
		public string CreatedByUserId { get; set; }
	}
}
