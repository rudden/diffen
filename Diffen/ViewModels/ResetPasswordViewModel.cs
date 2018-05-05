using System.ComponentModel.DataAnnotations;

namespace Diffen.ViewModels
{
	public class ResetPasswordViewModel
	{
		[Required]
		public string UserId { get; set; }

		[Required]
		public string NewPassword { get; set; }

		[Required]
		public string ConfirmNewPassword { get; set; }
	}
}
