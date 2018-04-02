namespace Diffen.Models.Forum
{
	public class User
	{
		public string Id { get; set; }
		public string Avatar { get; set; }
		public string NickName { get; set; }
		public bool IsAdmin { get; set; }
		public string SecludedUntil { get; set; }
	}
}
