using Microsoft.AspNetCore.Mvc;

namespace Diffen.Controllers
{
	public class HomeController : Controller
	{
		// GET
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Error()
		{
			return View();
		}
	}
}