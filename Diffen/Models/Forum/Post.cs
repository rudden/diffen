﻿using System.Collections.Generic;

namespace Diffen.Models.Forum
{
	public class Post
	{
		public int Id { get; set; }
		public string Message { get; set; }

		public User User { get; set; }

		public string UrlTipHref { get; set; }
		public IEnumerable<Vote> Votes { get; set; }

		public string Since { get; set; }
		public string Edited { get; set; }

		public bool HasLineup { get; set; }
		public bool IsPartOfConversation { get; set; }
		public bool IsSaved { get; set; }
		public bool IsScissored { get; set; }
		public bool LoggedInUserCanVote { get; set; }

		public bool InEdit => false;
		public bool InReply => false;
		public bool InScissor => false;
		public bool Disabled => false;
	}
}
