using System.ComponentModel.DataAnnotations;

namespace Diffen.ViewModels.Auth
{
	public class LoginViewModel
	{
		[Required]
		public string Email { get; set; }

		[Required]
		public string Password { get; set; }
	}
}
