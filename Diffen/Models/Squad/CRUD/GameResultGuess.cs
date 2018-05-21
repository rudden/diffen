namespace Diffen.Models.Squad.CRUD
{
	public class GameResultGuess
	{
		public int GameId { get; set; }
		public int NumberOfGoalsScoredByDif { get; set; }
		public int NumberOfGoalsScoredByOpponent { get; set; }
		public string GuessedByUserId { get; set; }
	}
}
