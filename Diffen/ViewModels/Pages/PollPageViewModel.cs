namespace Diffen.ViewModels.Pages
{
	public class PollPageViewModel : PageViewModel
	{
		public string SelectedPollSlug { get; set; }

		public override string PageTitle => "Omröstningar";
	}
}
