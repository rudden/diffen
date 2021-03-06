﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Other
{
	public class PollSelection
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string Name { get; set; }

		[ForeignKey("PollId")]
		public Poll Poll { get; set; }
		public int PollId { get; set; }

		// Linked Tables
		public ICollection<PollVote> Votes { get; set; }
	}
}
