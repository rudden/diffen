using System.Collections.Generic;

namespace Diffen.Models.Squad
{
	public class PlayerToLineup
	{
		public int Id { get; set; }
		public PlayerToLineupPlayer Player { get; set; }
		public Position Position { get; set; }
	}

	public class PlayerToLineupPlayer
	{
		public int Id { get; set; }
		public string FullName { get; set; }
		public string ShortName { get; set; }
		public PlayerAttributes Attributes { get; set; }
		public IEnumerable<Position> AvailablePositions { get; set; }
	}

	public class PlayerAttributes
	{
		public bool IsCaptain { get; set; }
		public bool IsViceCaptain { get; set; }
		public bool IsOutOnLoan { get; set; }
		public bool IsHereOnLoan { get; set; }
		public bool IsSold { get; set; }
	}
}