namespace Diffen.Models.Squad
{
	public class PlayerToLineup
	{
		public int Id { get; set; }
		public Player Player { get; set; }
		public Position Position { get; set; }
	}
}