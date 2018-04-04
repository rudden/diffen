namespace Diffen.Models.Forum.CRUD
{
	using Helpers.Enum;

	public class Vote
	{
		public VoteType Type { get; set; }
		public int PostId { get; set; }
		public string CreatedByUserId { get; set; }
	}
}
