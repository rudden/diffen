using Microsoft.AspNetCore.Mvc;

namespace Diffen.Controllers
{
	public class AuthController : Controller
	{
		// GET
		public IActionResult Login()
		{
			return View();
		}

		public IActionResult Register()
		{
			return View();
		}
	}
}