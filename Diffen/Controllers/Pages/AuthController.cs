using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using Serilog;

namespace Diffen.Controllers.Pages
{
	using ViewModels.Auth;
	using Repositories.Contracts;
	using Database.Entities.User;
	using Helpers.Authorize;
	using Helpers.Extensions;

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
				_logger.Warning("Could not find a user with email {userEmail} in the user repository", vm.Email);
				ModelState.AddModelError("All", "kontot existerar inte. var god skapa ett nytt!");
				return View();
			}
			if (!string.IsNullOrEmpty(attempt.SecludedUntil) && Convert.ToDateTime(attempt.SecludedUntil) > DateTime.Now)
			{
				_logger.Information(
					"Login: User with nickname {userNickName} tried to login even though he or she is secluded until {secludedUntil}",
					attempt.NickName, attempt.SecludedUntil);
				ModelState.AddModelError("All", $"du är spärrad till och med {attempt.SecludedUntil}");
				return View();
			}

			var result = await _signInManager.PasswordSignInAsync(
				vm.Email,
				vm.Password,
				isPersistent: vm.RememberMe, lockoutOnFailure: false);

			if (result.Succeeded)
			{
				_logger.Information("Successfully logged in user with email {userEmail}", vm.Email);
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
				_logger.Information("A user with email {userEmail} tried to register a new account using invite code {inviteCode}, but that code does not have an active invite", vm.Email, vm.UniqueCode);
				ModelState.AddModelError("", "hittade ingen inbjudan på denna kod");
				return View(vm);
			}
			if (await _userRepository.NickExistsAsync(vm.NickName))
			{
				_logger.Information("A user with email {userEmail} tried to register a new account using nickname {nickName}, but that nickname already exist", vm.Email, vm.NickName);
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

					await _signInManager.SignInAsync(user, isPersistent: true);

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
				_logger.Information("Could not register account due to the following errors {@registerModelErrors}", result.Errors.Select(x => x.Code));
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

		public IActionResult ResetPassword()
		{
			return View();
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
			if (!await _userRepository.EmailAndInviteCodeIsAMatchAsync(vm.InviteCode, vm.Email))
			{
				_logger.Information($"A user tried to reset password. But there is no account created using code {vm.InviteCode} and email {vm.Email}.");
				ModelState.AddModelError("", $"Hittade inget skapat konto för kod {vm.InviteCode} och email {vm.Email}");
				return View();
			}
			var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == vm.Email);
			var result = await GenerateNewPasswordAsync(user, vm.NewPassword);
			if (result.Succeeded)
			{
				await _signInManager.SignInAsync(user, isPersistent: true);
				return RedirectToAction("index", "forum");
			}
			_logger.Debug("Result errors {errors}", result.Errors);
			return View(vm);
		}

		[Authorize]
		[VerifyInputToLoggedInUserId("userId")]
		public IActionResult ResetPasswordForLoggedInUser(string userId)
		{
			if (User.GetUserId() != userId)
			{
				return RedirectToAction("index", "profile");
			}
			var vm = new ResetPasswordForLoggedInUserViewModel
			{
				UserId = userId
			};
			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPasswordForLoggedInUser(ResetPasswordForLoggedInUserViewModel vm)
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
				return View();
			}
			var user = await _userManager.GetUserAsync(User);
			var result = await GenerateNewPasswordAsync(user, vm.NewPassword);
			if (result.Succeeded)
			{
				return RedirectToAction("index", "profile");
			}
			_logger.Debug("Result errors {errors}", result.Errors);
			return View(vm);
		}

		private async Task<IdentityResult> GenerateNewPasswordAsync(AppUser user, string newPassword)
		{
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			return await _userManager.ResetPasswordAsync(user, token, newPassword);
		}
	}
}