using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

using AutoMapper;

namespace Diffen.Controllers.Pages
{
	using ViewModels;
	using Repositories.Contracts;

	[Route("chronicle")]
	public class ChronicleController : CommonController<ChroniclePageViewModel>
	{
		public ChronicleController(IConfigurationRoot configuration, IMapper mapper, IUserRepository userRepository)
			: base(configuration, mapper, userRepository)
		{
		}

		[Authorize]
		public IActionResult Index()
		{
			return View(Model);
		}

		[Authorize]
		[Route("{slug}")]
		public IActionResult Index(string slug)
		{
			Model.SelectedChronicleSlug = slug;
			return View("index", Model);
		}

		[Authorize]
		[Route("new")]
		public IActionResult New()
		{
			Model.InCreate = true;
			return View("index", Model);
		}

		[Authorize]
		[Route("new/{slug}")]
		public IActionResult New(string slug)
		{
			Model.InCreate = true;
			Model.SelectedChronicleSlug = slug;
			return View("index", Model);
		}
	}
}