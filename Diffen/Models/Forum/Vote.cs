namespace Diffen.Models.Forum
{
	using Helpers.Enum;

	public class Vote
	{
		public int Id { get; set; }
		public VoteType Type { get; set; }
		public string ByNickName { get; set; }
	}
}
