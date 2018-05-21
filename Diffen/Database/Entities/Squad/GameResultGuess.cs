using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Squad
{
	using User;

	public class GameResultGuess
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[ForeignKey("GuessedByUserId")]
		public AppUser GuessedByUser { get; set; }
		public string GuessedByUserId { get; set; }

		[ForeignKey("GameId")]
		public Game Game { get; set; }
		public int GameId { get; set; }

		public int NumberOfGoalsScoredByDif { get; set; }
		public int NumberOfGoalsScoredByOpponent { get; set; }

		public DateTime Created { get; set; }
	}
}
