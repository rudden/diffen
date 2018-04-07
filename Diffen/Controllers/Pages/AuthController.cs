using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

using Serilog;

namespace Diffen.Controllers.Pages
{
	using ViewModels;
	using Repositories.Contracts;
	using Database.Entities.User;

	public class AuthController : Controller
	{
		private readonly IUserRepository _userRepository;
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;

		private readonly IHostingEnvironment _environment;

		private readonly ILogger _logger = Log.ForContext<AuthController>();

		public AuthController(IUserRepository userRepository, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IHostingEnvironment environment)
		{
			_userRepository = userRepository;
			_userManager = userManager;
			_signInManager = signInManager;
			_environment = environment;
		}

		public IActionResult Login()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("index", "forum");
			}
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel vm, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			var attempt = await _userRepository.GetUserOnEmailAsync(vm.Email);
			if (attempt == null)
			{
				ModelState.AddModelError("All", "kontot existerar inte. var god skapa ett nytt!");
				return View();
			}
			if (attempt.SecludedUntil > DateTime.Now)
			{
				_logger.Information("Login: User with email {userEmail} tried to login even though he or she is secluded until {secludedUntil}", attempt.Email, attempt.SecludedUntil.ToString("yyyy-MM-dd"));
				ModelState.AddModelError("All", $"du är spärrad till och med {attempt.SecludedUntil:yyyy-MM-dd}");
				return View();
			}

			var result = await _signInManager.PasswordSignInAsync(
				vm.Email,
				vm.Password,
				true, false);

			if (result.Succeeded)
			{
				if (string.IsNullOrWhiteSpace(returnUrl))
				{
					return RedirectToAction("index", "forum");
				}
				return Redirect(returnUrl);
			}

			ModelState.AddModelError("All", "felaktiga inloggningsuppgifter");
			return View();
		}

		public IActionResult Register()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("index", "forum");
			}
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel vm, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View(vm);
			}

			if (!await _userRepository.EmailHasInvite(vm.Email))
			{
				_logger.Information("Register: Someone tried to register an account without an invite with email {modelEmail}", vm.Email);
				ModelState.AddModelError("", "hittade ingen inbjudan på den valda emailen");
				return View(vm);
			}

			if (await _userRepository.NickExistsAsync(vm.NickName))
			{
				ModelState.AddModelError("", "nicket finns redan registrerat");
				return View(vm);
			}

			var user = new AppUser
			{
				UserName = vm.Email,
				Email = vm.Email,
				Bio = vm.Bio,
				Joined = DateTime.Now
			};

			if (string.Equals(vm.Password, vm.ConfirmPassword))
			{
				var result = await _userManager.CreateAsync(user, vm.Password);

				if (result.Succeeded)
				{
					var newUser = await _userRepository.GetUserOnEmailAsync(vm.Email);
					await _userRepository.AddNickNameAsync(new NickName
					{
						UserId = newUser.Id,
						Nick = vm.NickName,
						Created = DateTime.Now
					});

					var invite = await _userRepository.GetInviteOnEmailAsync(vm.Email);
					invite.AccountCreated = DateTime.Now;
					invite.AccountIsCreated = true;
					await _userRepository.UpdateInviteAsync(invite);

					_logger.Information("Register: Invite for email {inviteEmail} has now been registered as used", invite.Email);

					if (vm.Avatar != null)
					{
						var uploads = Path.Combine(_environment.WebRootPath, $"uploads\\avatars\\{newUser.Id}");
						Directory.CreateDirectory(uploads);
						if (vm.Avatar.Length > 0)
						{
							var path = Path.Combine(uploads, vm.Avatar.FileName);
							using (var fileStream = new FileStream(path, FileMode.Create))
							{
								await vm.Avatar.CopyToAsync(fileStream);
								newUser.AvatarFileName = vm.Avatar.FileName;
							}
							_logger.Information("Register: Stored avatar with filename {avatarFileName} for user {userName}", newUser.AvatarFileName, newUser.UserName);
							await _userRepository.UpdateUserAsync(newUser);
						}
					}

					await _signInManager.SignInAsync(user, isPersistent: false);

					if (string.IsNullOrWhiteSpace(returnUrl))
					{
						return RedirectToAction("index", "forum");
					}
					return Redirect(returnUrl);
				}
				if (result.Errors.Any(x => x.Code == "DuplicateUserName"))
				{
					ModelState.AddModelError("", "användarnamnet (email) finns redan registrerad");
				}
			}
			else
			{
				ModelState.AddModelError("", "lösenorden matchar inte");
			}
			return View(vm);
		}

		public async Task<IActionResult> Logout()
		{
			if (User.Identity.IsAuthenticated)
			{
				await _signInManager.SignOutAsync();
			}
			return RedirectToAction("index", "forum");
		}
	}
}