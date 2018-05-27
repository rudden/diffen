using System.Collections.Generic;

namespace Diffen.Models.Squad
{
	using Helpers.Enum;

	public class Game
	{
		public int Id { get; set; }
		public GameType Type { get; set; }
		public ArenaType ArenaType { get; set; }
		public string Opponent { get; set; }
		public int NumberOfGoalsScoredByOpponent { get; set; }
		public int NumberOfAddonMinutes { get; set; }
		public Lineup Lineup { get; set; }
		public IEnumerable<PlayerEvent> PlayerEvents { get; set; }
		public string PlayedOn { get; set; }
	}
}
