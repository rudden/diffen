using System.ComponentModel.DataAnnotations;

namespace Diffen.ViewModels.Auth
{
	public class ResetPasswordViewModel
	{
		[Required]
		public string Email { get; set; }

		[Required]
		public string InviteCode { get; set; }

		[Required]
		public string NewPassword { get; set; }

		[Required]
		public string ConfirmNewPassword { get; set; }
	}
}
