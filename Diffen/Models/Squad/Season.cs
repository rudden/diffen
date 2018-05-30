using System.Collections.Generic;

namespace Diffen.Models.Squad
{
	public class Season
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public bool IsActive { get; set; }
		public IEnumerable<Game> Games { get; set; }
	}
}
