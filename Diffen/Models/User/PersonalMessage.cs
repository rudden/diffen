namespace Diffen.Models.User
{
	public class PersonalMessage
	{
		public int Id { get; set; }
		public From From { get; set; }
		public string Message { get; set; }
		public string Since { get; set; }
	}

	public class From
	{
		public string Avatar { get; set; }
		public string NickName { get; set; }
	}
}