using System;
using System.Collections.Generic;

namespace Diffen.Models.Squad.CRUD
{
	using Helpers.Enum;

	public class Game
	{
		public int Id { get; set; }
		public GameType Type { get; set; }
		public DateTime PlayedDate { get; set; }
		public IEnumerable<PlayerEvent> Events { get; set; }
	}

	public class PlayerEvent
	{
		public int PlayerId { get; set; }
		public GameEventType Type { get; set; }
	}
}
