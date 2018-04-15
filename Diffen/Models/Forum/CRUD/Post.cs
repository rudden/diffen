namespace Diffen.Models.Forum.CRUD
{
	using Squad.CRUD;

	public class Post
	{
		public int Id { get; set; }
		public string Message { get; set; }
		public string CreatedByUserId { get; set; }
		public int? ParentPostId { get; set; }
		public string UrlTipHref { get; set; }
		public Lineup Lineup { get; set; }
	}
}