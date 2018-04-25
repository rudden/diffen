using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Diffen.Database.Entities.User;

namespace Diffen.Database.Entities.Other
{
	public class PollVote
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[ForeignKey("PollSelectionId")]
		public PollSelection PollSelection { get; set; }
		public int PollSelectionId { get; set; }

		[ForeignKey("VotedByUserId")]
		public AppUser VotedByUser { get; set; }
		public string VotedByUserId { get; set; }

		public DateTime Created { get; set; }
	}
}
