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

		private int _unReadMessages;

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
				var user = _mapper.Map<Models.User.User>(_userRepository.GetUserOnEmailAsync(User.Identity.Name).Result);
				Model = new TModel
				{
					Api = _configuration["Api:Url"],
					LoggedInUser = user,
					Page = pageName
				};
				_unReadMessages = user.NumberOfUnReadPersonalMessages;
			}

			ViewBag.Title = $"{(!string.IsNullOrEmpty(Model.Page) ? $"{(_unReadMessages > 0 ? $"({_unReadMessages})" : "")} |" : "")} {Model.PageTitle}";

			base.OnActionExecuting(context);
		}
	}
}
