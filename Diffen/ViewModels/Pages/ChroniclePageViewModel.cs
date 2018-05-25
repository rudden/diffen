namespace Diffen.ViewModels.Pages
{
	public class ChroniclePageViewModel : PageViewModel
	{
		public bool InCreate { get; set; }
		public string SelectedChronicleSlug { get; set; }

		public override string PageTitle => "Krönikor";
	}
}
