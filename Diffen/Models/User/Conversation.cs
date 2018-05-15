namespace Diffen.Models.User
{
	public class Conversation
	{
		public IdAndNickNameUser User { get; set; }
		public int NumberOfUnReadMessages { get; set; }
	}
}
