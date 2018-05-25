using Diffen.Models.User;

namespace Diffen.ViewModels.Pages
{
	public class PageViewModel
	{
		public string Api { get; set; }
		public User LoggedInUser { get; set; }
		public string Page { get; set; }
		public virtual string PageTitle { get; set; }
	}
}
