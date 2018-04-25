namespace Diffen.Models.Forum
{
	public class ParentPost
	{
		public int Id { get; set; }
		public string Message { get; set; }
		public User User { get; set; }
		public ParentPost Parent { get; set; }
		public string Since { get; set; }
	}
}
