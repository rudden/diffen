using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

using AutoMapper;

namespace Diffen.Controllers.Pages
{
	using ViewModels;
	using Repositories.Contracts;

	[Route("kronika")]
	public class ChronicleController : CommonController<ChroniclePageViewModel>
	{
		private readonly IChronicleRepository _chronicleRepository;

		public ChronicleController(IConfigurationRoot configuration, IMapper mapper, IUserRepository userRepository, IChronicleRepository chronicleRepository)
			: base(configuration, mapper, userRepository)
		{
			_chronicleRepository = chronicleRepository;
		}

		[Authorize]
		public IActionResult Index()
		{
			return View("_Page", Model);
		}

		[Authorize]
		[Route("{slug}")]
		public IActionResult Index(string slug)
		{
			Model.SelectedChronicleSlug = slug;
			return View("_Page", Model);
		}

		[Authorize(Policy = "IsAuthor")]
		[Route("ny")]
		public IActionResult New()
		{
			Model.InCreate = true;
			return View("_Page", Model);
		}

		[Authorize(Policy = "IsAuthor")]
		[Route("uppdatera/{slug}")]
		public async Task<IActionResult> New(string slug)
		{
			if (!await _chronicleRepository.SelectedChronicleIsCreatedByLoggedInUserAsync(slug))
			{
				return Forbid();
			}
			Model.InCreate = true;
			Model.SelectedChronicleSlug = slug;
			return View("_Page", Model);
		}
	}
}