using System.Collections.Generic;

namespace Diffen.Models.User
{
	using Database.Entities.User;

	public class Filter
	{
		public Filter() {}

		public Filter(AppUser user)
		{
			UserId = user.Id;
			PostsPerPage = 20;
			ExcludedUsers = new List<KeyValuePair<string, string>>();
			ExcludedThreads = new List<KeyValuePair<int, string>>();
		}

		public Filter(int postsPerPage)
		{
			PostsPerPage = postsPerPage;
		}

		public string UserId { get; set; }
		public int PostsPerPage { get; set; }
		public bool HideLeftMenu { get; set; }
		public bool HideRightMenu { get; set; }
		public IEnumerable<KeyValuePair<string, string>> ExcludedUsers { get; set; }
		public IEnumerable<KeyValuePair<int, string>> ExcludedThreads { get; set; }
	}
}
