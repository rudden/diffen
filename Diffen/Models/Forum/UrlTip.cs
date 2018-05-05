namespace Diffen.Models.Forum
{
	public class UrlTip
	{
		public int Id { get; set; }
		public string Href { get; set; }
		public int Clicks { get; set; }
		public int? PostId { get; set; }
	}
}
