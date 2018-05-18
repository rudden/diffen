using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.User
{
	public class PersonalMessage
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string Message { get; set; }

		[ForeignKey("FromUserId")]
		public AppUser FromUser { get; set; }
		public string FromUserId { get; set; }

		[ForeignKey("ToUserId")]
		public AppUser ToUser { get; set; }
		public string ToUserId { get; set; }

		public bool IsReadByToUser { get; set; }

		public DateTime Created { get; set; }
	}
}
