using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

using AutoMapper;

namespace Diffen.Controllers.Pages
{
	using ViewModels;
	using Repositories.Contracts;

	[Route("forum")]
	public class ForumController : CommonController<ForumPageViewModel>
	{
		public ForumController(IConfigurationRoot configuration, IMapper mapper, IUserRepository userRepository) 
			: base(configuration, mapper, userRepository)
		{
		}

		[Authorize]
		public IActionResult Index()
		{
			return View("_Page", Model);
		}

		[Authorize]
		[Route("post/{id}")]
		public IActionResult Index(int id)
		{
			Model.SelectedPostId = id;
			return View("_Page", Model);
		}
	}
}