using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Squad
{
	using Helpers.Enum;

	public class Game
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public GameType Type { get; set; }
		public DateTime OnDate { get; set; }

		// Linked Tables
		public ICollection<PlayerEvent> PlayerEvents { get; set; }
	}
}
