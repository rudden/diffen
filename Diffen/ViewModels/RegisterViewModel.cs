using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

namespace Diffen.ViewModels
{
	public class RegisterViewModel
	{
		[Required]
		public string Email { get; set; }

		[Required]
		public string NickName { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		public string ConfirmPassword { get; set; }

		[MaxLength(100, ErrorMessage = "får max vara 100 tecken")]
		public string Bio { get; set; }

		public IFormFile Avatar { get; set; }
	}
}
