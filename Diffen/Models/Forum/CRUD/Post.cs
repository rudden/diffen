using System.Collections.Generic;

namespace Diffen.Models.Forum.CRUD
{
	public class Post
	{
		public int Id { get; set; }
		public string Message { get; set; }
		public string CreatedByUserId { get; set; }
		public int? ParentPostId { get; set; }
		public string UrlTipHref { get; set; }
		public int LineupId { get; set; }
		public IEnumerable<int> ThreadIds { get; set; }
		public IEnumerable<string> NewThreadNames { get; set; }
	}
}