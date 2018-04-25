using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

using AutoMapper;

namespace Diffen.Controllers.Pages
{
	using ViewModels;
	using Repositories.Contracts;

	public class HomeController : CommonController<PageViewModel>
	{
		public HomeController(IConfigurationRoot configuration, IMapper mapper, IUserRepository userRepository) : base(configuration, mapper, userRepository)
		{
		}

		[Authorize]
		public IActionResult Index()
		{
			return View(Model);
		}
	}
}