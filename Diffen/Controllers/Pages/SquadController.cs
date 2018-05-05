using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

using AutoMapper;

namespace Diffen.Controllers.Pages
{
	using ViewModels.Pages;
	using Repositories.Contracts;

	[Route("trupp")]
	public class SquadController : CommonController<PageViewModel>
	{
		public SquadController(IConfigurationRoot configuration, IMapper mapper, IUserRepository userRepository)
			: base(configuration, mapper, userRepository)
		{
		}

		[Authorize]
		public IActionResult Index()
		{
			return View("_Page", Model);
		}
	}
}