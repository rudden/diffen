﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

using AutoMapper;

namespace Diffen.Controllers.Pages
{
	using ViewModels.Pages;
	using Repositories.Contracts;

	[Route("profil")]
	public class ProfileController : CommonController<ProfilePageViewModel>
	{
		public ProfileController(IConfigurationRoot configuration, IMapper mapper, IUserRepository userRepository)
			: base(configuration, mapper, userRepository)
		{
		}

		[Authorize]
		public IActionResult Index()
		{
			return View("_Page", Model);
		}

		[Authorize]
		[Route("{id}")]
		public IActionResult Index(string id)
		{
			Model.SelectedUserId = id;
			return View("_Page", Model);
		}
	}
}