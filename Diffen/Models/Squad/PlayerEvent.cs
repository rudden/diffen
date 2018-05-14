namespace Diffen.Models.Squad
{
	using Helpers.Enum;

	public class PlayerEvent
	{
		public int Id { get; set; }
		public Player Player { get; set; }
		public GameEventType EventType { get; set; }
	}
}
