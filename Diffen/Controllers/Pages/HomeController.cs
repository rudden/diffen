using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Diffen.Controllers.Pages
{
	public class HomeController : Controller
	{
		[Authorize]
		public IActionResult Index()
		{
			return View();
		}

		[Authorize]
		public IActionResult Error()
		{
			return View();
		}
	}
}