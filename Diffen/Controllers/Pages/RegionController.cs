using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

using AutoMapper;

namespace Diffen.Controllers.Pages
{
	using ViewModels;
	using Repositories.Contracts;

	[Route("region")]
	public class RegionController : CommonController<RegionPageViewModel>
	{
		private readonly IConfigurationRoot _configuration;

		public RegionController(IConfigurationRoot configuration, IMapper mapper, IUserRepository userRepository)
			: base(configuration, mapper, userRepository)
		{
			_configuration = configuration;
		}

		[Authorize]
		public IActionResult Index()
		{
			Model.GoogleMapsApiKey = _configuration["GoogleMapsApi:Key"];
			return View(Model);
		}
	}
}