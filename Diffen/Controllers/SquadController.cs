using Microsoft.AspNetCore.Mvc;

namespace Diffen.Controllers
{
	public class SquadController : Controller
	{
		// GET
		public IActionResult Index()
		{
			return View();
		}
	}
}