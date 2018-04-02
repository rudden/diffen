namespace Diffen.Models.Forum
{
	using Helpers.Enum;

	public class Vote
	{
		public VoteType Type { get; set; }
		public string ByNickName { get; set; }
	}
}
