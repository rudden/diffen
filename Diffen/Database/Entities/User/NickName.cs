using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.User
{
	public class NickName
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[ForeignKey("UserId")]
		public AppUser User { get; set; }
		public string UserId { get; set; }

		public string Nick { get; set; }
		public DateTime Created { get; set; }
	}
}