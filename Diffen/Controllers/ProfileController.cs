using Microsoft.AspNetCore.Mvc;

namespace Diffen.Controllers
{
	public class ProfileController : Controller
	{
		// GET
		public IActionResult Index()
		{
			return View();
		}
	}
}