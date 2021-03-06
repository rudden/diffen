﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.User
{
	public class Invite
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string UniqueCode { get; set; }
		public bool AccountIsCreated { get; set; }

		[ForeignKey("InvitedByUserId")]
		public AppUser InvitedByUser { get; set; }
		public string InvitedByUserId { get; set; }

		[ForeignKey("InviteUsedByUserId")]
		public AppUser InviteUsedByUser { get; set; }
		public string InviteUsedByUserId { get; set; }

		public DateTime InviteSent { get; set; }
		public DateTime? AccountCreated { get; set; }
	}
}
