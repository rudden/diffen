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
			PostsPerPage = 5;
			ExcludedUsers = new List<KeyValuePair<string, string>>();
		}

		public Filter(int postsPerPage)
		{
			PostsPerPage = postsPerPage;
		}

		public string UserId { get; set; }
		public int PostsPerPage { get; set; }
		public IEnumerable<KeyValuePair<string, string>> ExcludedUsers { get; set; }
	}
}
