using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

using AutoMapper;

namespace Diffen.Controllers
{
	using ViewModels;
	using Repositories.Contracts;

	public abstract class CommonController : Controller
	{
		private readonly IConfigurationRoot _configuration;

		private readonly IMapper _mapper;
		private readonly IUserRepository _userRepository;

		protected PageViewModel Model;

		protected CommonController(IConfigurationRoot configuration, IMapper mapper, IUserRepository userRepository)
		{
			_configuration = configuration;
			_mapper = mapper;
			_userRepository = userRepository;
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (Model == null)
			{
				var controller = (ControllerBase) context.Controller;
				Model = new PageViewModel
				{
					Api = _configuration["Api:Url"],
					LoggedInUser = _mapper.Map<Models.User.User>(_userRepository.GetUserOnEmailAsync(User.Identity.Name).Result),
					Page = controller.ControllerContext.ActionDescriptor.ControllerName.ToLower()
				};
			}
			base.OnActionExecuting(context);
		}
	}
}
