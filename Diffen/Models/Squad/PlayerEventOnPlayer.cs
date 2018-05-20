namespace Diffen.Models.Squad
{
	using Helpers.Enum;

	public class PlayerEventOnPlayer
	{
		public int GameId { get; set; }
		public GameType GameType { get; set; }
		public GameEventType EventType { get; set; }
		public string Date { get; set; }
	}
}
