using System;
using System.Collections.Generic;

namespace Diffen.Models.Squad.CRUD
{
	using Helpers.Enum;

	public class Lineup
	{
		public int Id { get; set; }
		public int FormationId { get; set; }
		public IEnumerable<PlayerToLineup> Players { get; set; }
		public string CreatedByUserId { get; set; }
		public LineupType Type { get; set; }
		public DateTime Created { get; set; }
	}

	public class PlayerToLineup
	{
		public int PlayerId { get; set; }
		public int PositionId { get; set; }
	}
}
