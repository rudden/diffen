namespace Diffen.ViewModels
{
	using Models.User;

	public class PageViewModel
	{
		public string Api { get; set; }
		public User LoggedInUser { get; set; }
		public string Page { get; set; }
	}
}
