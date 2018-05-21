namespace Diffen.Models.Squad
{
	using Helpers.Enum;

	public class PlayerEvent
	{
		public int Id { get; set; }
		public EventPlayer Player { get; set; }
		public GameEventType EventType { get; set; }
		public int InMinute { get; set; }
	}
}
