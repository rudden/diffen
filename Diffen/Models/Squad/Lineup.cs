using System.Collections.Generic;

namespace Diffen.Models.Squad
{
	using Helpers.Enum;

	public class Lineup
	{
		public int Id { get; set; }
		public Formation Formation { get; set; }
		public List<PlayerToLineup> Players { get; set; }
		public LineupType Type { get; set; }
		public string Created { get; set; }
	}
}
