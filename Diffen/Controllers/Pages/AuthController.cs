using System;
using System.Linq;
using System.Threading.Tasks;
using Diffen.Helpers.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using Serilog;

namespace Diffen.Controllers.Pages
{
	using ViewModels.Auth;
	using Repositories.Contracts;
	using Database.Entities.User;

	public class AuthController : Controller
	{
		private readonly IUserRepository _userRepository;
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly IUploadRepository _uploadRepository;
		private readonly IRegionRepository _regionRepository;

		private readonly ILogger _logger = Log.ForContext<AuthController>();

		public AuthController(IUserRepository userRepository, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IUploadRepository uploadRepository, IRegionRepository regionRepository)
		{
			_userRepository = userRepository;
			_userManager = userManager;
			_signInManager = signInManager;
			_uploadRepository = uploadRepository;
			_regionRepository = regionRepository;
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
			if (!string.IsNullOrEmpty(attempt.SecludedUntil) && Convert.ToDateTime(attempt.SecludedUntil) > DateTime.Now)
			{
				_logger.Information(
					"Login: User with email {userEmail} tried to login even though he or she is secluded until {secludedUntil}",
					attempt.Email, attempt.SecludedUntil);
				ModelState.AddModelError("All", $"du är spärrad till och med {attempt.SecludedUntil}");
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

		public async Task<IActionResult> Register()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("index", "forum");
			}
			var model = new RegisterViewModel
			{
				Regions = await _regionRepository.GetRegionsAsync()
			};
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel vm, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View(vm);
			}
			if (!await _userRepository.InviteExistsAsync(vm.UniqueCode))
			{
				ModelState.AddModelError("", "hittade ingen inbjudan på denna kod");
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
					await _userRepository.CreateNewNickNameAsync(user.Id, vm.NickName);
					await _userRepository.SetInviteAsAccountCreatedAsync(user.Id, vm.UniqueCode);

					if (vm.Avatar?.Length <= 700000)
					{
						var fileName = await _uploadRepository.UploadFileAsync("avatars", vm.Avatar, user.Id);
						await _userRepository.UpdateAvatarFileNameForUserWithIdAsync(user.Id, fileName);
					}

					if (vm.RegionId > 0)
					{
						await _userRepository.CreateRegionToUserAsync(user.Id, vm.RegionId);
					}

					await _signInManager.SignInAsync(user, isPersistent: false);

					if (string.IsNullOrWhiteSpace(returnUrl))
					{
						return RedirectToAction("index", "forum");
					}

					return Redirect(returnUrl);
				}
				foreach (var error in result.Errors)
				{
					switch (error.Code)
					{
						case "DuplicateUserName":
							ModelState.AddModelError("", "- användarnamnet (email) finns redan registrerad");
							break;
						case "PasswordRequiresUpper":
							ModelState.AddModelError("", "- lösenordet måste innehålla minst en stor bokstav");
							break;
						case "PasswordRequiresNonAlphanumeric":
							ModelState.AddModelError("", "- lösenordet måste innehålla minst ett icke alfanumeriskt tecken, t.ex ett utropstecken eller punkt");
							break;
						case "PasswordRequiresDigit":
							ModelState.AddModelError("", "- lösenordet måste innehålla minst en siffra");
							break;
					}
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

		public IActionResult ResetPassword(string userId)
		{
			if (User.GetUserId() != userId)
			{
				return RedirectToAction("index", "profile");
			}
			var vm = new ResetPasswordViewModel
			{
				UserId = userId
			};
			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			if (vm.NewPassword != vm.ConfirmNewPassword)
			{
				ModelState.AddModelError("", "Lösenorden matchar inte");
				return View();
			}
			if (User.GetUserId() != vm.UserId)
			{
				ModelState.AddModelError("", "Du kan inte ändra lösenord för en annan användare");
			}
			var user = await _userManager.GetUserAsync(User);
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			var result = await _userManager.ResetPasswordAsync(user, token, vm.NewPassword);

			if (result.Succeeded)
			{
				return RedirectToAction("index", "profile");
			}
			_logger.Debug("Result errors {errors}", result.Errors);
			return View(vm);
		}
	}
}