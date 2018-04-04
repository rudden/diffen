using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

using AutoMapper;

namespace Diffen.Controllers.Pages
{
	using Repositories.Contracts;

	[Route("forum")]
	public class ForumController : CommonController
	{
		public ForumController(IConfigurationRoot configuration, IMapper mapper, IUserRepository userRepository) 
			: base(configuration, mapper, userRepository)
		{
		}

		[Authorize]
		public IActionResult Index()
		{
			return View(Model);
		}

		[Authorize]
		[Route("post/{id}")]
		public IActionResult Index(int id)
		{
			Model.PostId = id;
			return View(Model);
		}
	}
}