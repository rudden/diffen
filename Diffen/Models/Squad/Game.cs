using System.Collections.Generic;

namespace Diffen.Models.Squad
{
	using Helpers.Enum;

	public class Game
	{
		public int Id { get; set; }
		public GameType Type { get; set; }
		public string PlayedOn { get; set; }
		public IEnumerable<PlayerEvent> PlayerEvents { get; set; }
	}
}
