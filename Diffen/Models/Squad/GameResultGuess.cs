namespace Diffen.Models.Squad
{
	public class GameResultGuess
	{
		public Game Game { get; set; }
		public int NumberOfGoalsScoredByDif { get; set; }
		public int NumberOfGoalsScoredByOpponent { get; set; }
		public IdAndNickNameUser GuessedByUser { get; set; }
	}
}
