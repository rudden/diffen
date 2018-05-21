using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

using AutoMapper;

namespace Diffen.Controllers
{
	using ViewModels.Pages;
	using Repositories.Contracts;

	public abstract class CommonController<TModel> : Controller where TModel : PageViewModel, new() 
	{
		private readonly IConfigurationRoot _configuration;

		private readonly IMapper _mapper;
		private readonly IUserRepository _userRepository;

		protected TModel Model;

		protected CommonController(IConfigurationRoot configuration, IMapper mapper, IUserRepository userRepository)
		{
			_configuration = configuration;
			_mapper = mapper;
			_userRepository = userRepository;
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var controller = (ControllerBase)context.Controller;
			var pageName = controller.ControllerContext.ActionDescriptor.ControllerName.ToLower();
			if (Model == null)
			{
				Model = new TModel
				{
					Api = _configuration["Api:Url"],
					LoggedInUser = _mapper.Map<Models.User.User>(_userRepository.GetUserOnEmailAsync(User.Identity.Name).Result),
					Page = pageName
				};
			}
			SetPageTitle(pageName);
			base.OnActionExecuting(context);
		}

		private void SetPageTitle(string pageName)
		{
			string pageTitle;
			switch (pageName)
			{
				case "home":
					pageTitle = "Hem";
					break;
				case "forum":
					pageTitle = "Forum";
					break;
				case "chronicle":
					pageTitle = "Krönikor";
					break;
				case "poll":
					pageTitle = "Omröstningar";
					break;
				case "region":
					pageTitle = "Områden";
					break;
				case "squad":
					pageTitle = "Trupp";
					break;
				case "profile":
					pageTitle = "Profil";
					break;
				case "aboutdif":
					pageTitle = "Om DIF";
					break;
				default:
					pageTitle = "";
					break;
			}
			ViewBag.Title = $"{(!string.IsNullOrEmpty(pageTitle) ? "| " : "")} {pageTitle}";
		}
	}
}
