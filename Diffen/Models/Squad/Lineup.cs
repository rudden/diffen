using System.Collections.Generic;

namespace Diffen.Models.Squad
{
	public class Lineup
	{
		public int Id { get; set; }
		public string ComponentName { get; set; }
		public List<PlayerToLineup> Players { get; set; }
		public string Created { get; set; }
	}
}
