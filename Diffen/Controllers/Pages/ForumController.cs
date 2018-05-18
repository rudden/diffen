using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

using AutoMapper;

namespace Diffen.Controllers.Pages
{
	using ViewModels.Pages;
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
		[Route("inlagg/{id}")]
		public IActionResult SelectedPost(int id)
		{
			Model.SelectedPostId = id;
			return View("_Page", Model);
		}

		[Authorize]
		[Route("inlagg/{id}/konversation")]
		public IActionResult ConversationForPost(int id)
		{
			Model.SelectedPostId = id;
			Model.FullConversationMode = true;
			return View("_Page", Model);
		}

		[Authorize]
		[Route("sida/{pageNumber}")]
		public IActionResult SelectedPage(int pageNumber)
		{
			Model.SelectedPageNumber = pageNumber;
			return View("_Page", Model);
		}
	}
}