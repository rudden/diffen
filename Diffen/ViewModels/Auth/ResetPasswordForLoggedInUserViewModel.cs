using System.ComponentModel.DataAnnotations;

namespace Diffen.ViewModels.Auth
{
	public class ResetPasswordForLoggedInUserViewModel
	{
		[Required]
		public string UserId { get; set; }

		[Required]
		public string NewPassword { get; set; }

		[Required]
		public string ConfirmNewPassword { get; set; }
	}
}
