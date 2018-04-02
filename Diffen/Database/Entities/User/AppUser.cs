using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;

namespace Diffen.Database.Entities.User
{
	public class AppUser : IdentityUser
	{
		public string Bio { get; set; }
		public string AvatarFileName { get; set; }
		public DateTime Joined { get; set; }
		public DateTime SecludedUntil { get; set; }

		public ICollection<NickName> NickNames { get; set; }
	}
}