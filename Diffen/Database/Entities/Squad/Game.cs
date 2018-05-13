using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Squad
{
	public class Game
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[ForeignKey("HomeTeamId")]
		public Team HomeTeam { get; set; }
		public int HomeTeamId { get; set; }

		[ForeignKey("AwayTeamId")]
		public Team AwayTeam { get; set; }
		public int AwayTeamId { get; set; }

		public DateTime Played { get; set; }

		// Linked Tables
		public ICollection<GameEvent> Events { get; set; }
	}
}
