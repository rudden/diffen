using System.Collections.Generic;

namespace Diffen.Models.Squad
{
	public class GameResultGuessLeagueItem
	{
		public IdAndNickNameUser User { get; set; }
		public IEnumerable<GameResultGuess> Guesses { get; set; }
	}
}
