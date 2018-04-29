using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Other
{
	using User;

	public class Poll
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string Name { get; set; }
		public string Slug { get; set; }

		[ForeignKey("CreatedByUserId")]
		public AppUser CreatedByUser { get; set; }
		public string CreatedByUserId { get; set; }

		public DateTime Created { get; set; }

		// Linked Tables
		public ICollection<PollSelection> Selections { get; set; }
	}
}