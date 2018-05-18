namespace Diffen.Models.Squad
{
	using Helpers.Enum;

	public class Title
	{
		public int Id { get; set; }
		public TitleType Type { get; set; }
		public string Year { get; set; }
		public string Description { get; set; }
	}
}
