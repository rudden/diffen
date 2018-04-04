using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

using AutoMapper;

namespace Diffen.Controllers.Pages
{
	using Repositories.Contracts;

	[Route("profile")]
	public class ProfileController : CommonController
	{
		public ProfileController(IConfigurationRoot configuration, IMapper mapper, IUserRepository userRepository)
			: base(configuration, mapper, userRepository)
		{
		}

		[Authorize]
		public IActionResult Index()
		{
			return View(Model);
		}

		[Authorize]
		[Route("{id}")]
		public IActionResult Index(string id)
		{
			Model.LoggedInUser.SelectedId = id;
			return View("index", Model);
		}
	}
}