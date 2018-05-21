namespace Diffen.Models.Squad
{
	using Helpers.Enum;

	public class PlayerEventOnPlayer
	{
		public int GameId { get; set; }
		public string Opponent { get; set; }
		public GameType GameType { get; set; }
		public GameEventType EventType { get; set; }
		public int InMinuteOfGame { get; set; }
		public string Date { get; set; }
	}
}
