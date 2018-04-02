using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.User
{
	public class Invite
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string Email { get; set; }
		public bool AccountIsCreated { get; set; }

		[ForeignKey("InvitedByUserId")]
		public AppUser InvitedByUser { get; set; }
		public string InvitedByUserId { get; set; }

		public DateTime InviteSent { get; set; }
		public DateTime? AccountCreated { get; set; }
	}
}
