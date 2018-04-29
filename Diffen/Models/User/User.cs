using System.Collections.Generic;

namespace Diffen.Models.User
{
	using Squad;

	public class User
	{
		public string Id { get; set; }
		public string Bio { get; set; }
		public string Email { get; set; }
		public string NickName { get; set; }
		public string Avatar { get; set; }
		public string Region { get; set; }

		public int Karma { get; set; }
		public int NumberOfPosts { get; set; }

		public Filter Filter { get; set; }
		public VoteStatistics VoteStatistics { get; set; }
		public Player FavoritePlayer { get; set; }
		public IEnumerable<int> SavedPostsIds { get; set; }
		public IEnumerable<string> InRoles { get; set; }

		public string SecludedUntil { get; set; }
	}
}