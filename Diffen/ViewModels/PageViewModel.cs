using System.Collections.Generic;

namespace Diffen.ViewModels
{
	using Models.User;

	public class PageViewModel
	{
		public string Api { get; set; }
		public Models.User.User LoggedInUser { get; set; }
		public string Page { get; set; }
		public int PostId { get; set; }
		public int PageNumber { get; set; }
		public string SelectedUserId { get; set; }
	}

	public class LoggedInUser
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Nick { get; set; }
		public string AvatarFileName { get; set; }
		public string Bio { get; set; }
		public string SelectedId { get; set; }
		public string SecludedUntil { get; set; }
		public IEnumerable<string> InRoles { get; set; }
		public Filter Filter { get; set; }
	}
}
