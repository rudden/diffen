namespace Diffen.Models.User
{
	public class PersonalMessage
	{
		public int Id { get; set; }
		public PmUser From { get; set; }
		public PmUser To { get; set; }
		public string Message { get; set; }
		public bool IsReadByToUser { get; set; }
		public string Since { get; set; }
	}

	public class PmUser
	{
		public string Id { get; set; }
		public string Avatar { get; set; }
		public string NickName { get; set; }
	}
}