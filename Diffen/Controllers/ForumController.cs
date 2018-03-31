using Microsoft.AspNetCore.Mvc;

namespace Diffen.Controllers
{
	public class ForumController : Controller
	{
		// GET
		public IActionResult Index()
		{
			return View();
		}
	}
}