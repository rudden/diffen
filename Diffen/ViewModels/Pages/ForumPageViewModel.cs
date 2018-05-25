namespace Diffen.ViewModels.Pages
{
	public class ForumPageViewModel : PageViewModel
	{
		public int SelectedPostId { get; set; }
		public int SelectedPageNumber { get; set; }
		public bool FullConversationMode { get; set; }

		public override string PageTitle => "Forum";
	}
}
