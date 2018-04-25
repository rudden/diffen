using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

using AutoMapper;

namespace Diffen.Controllers.Pages
{
	using ViewModels;
	using Repositories.Contracts;

	[Route("profile")]
	public class ProfileController : CommonController<ProfilePageViewModel>
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
			Model.SelectedUserId = id;
			return View("index", Model);
		}
	}
}