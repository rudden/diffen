using System.Collections.Generic;

namespace Diffen.Models.Forum
{
	public class Post
	{
		public int Id { get; set; }
		public string Message { get; set; }

		public User User { get; set; }

		public string UrlTipHref { get; set; }
		public IEnumerable<Vote> Votes { get; set; }
		public ParentPost ParentPost { get; set; }
		public int? LineupId { get; set; }

		public string Since { get; set; }
		public string Updated { get; set; }

		public bool IsScissored { get; set; }
		public bool LoggedInUserCanVote { get; set; }

		public bool InEdit => false;
		public bool InReply => false;
		public bool InScissor => false;
		public bool Disabled => false;
	}
}
