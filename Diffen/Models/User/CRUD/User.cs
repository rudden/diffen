using System.Collections.Generic;

namespace Diffen.Models.User.CRUD
{
	public class User
	{
		public string NickName { get; set; }
		public string Bio { get; set; }
		public string Region { get; set; }
		public IEnumerable<string> Roles { get; set; }
		public int FavoritePlayerId { get; set; }
		public string SecludeUntil { get; set; }
	}
}
