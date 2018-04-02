using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Forum
{
	using User;

	public class Post
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string Message { get; set; }

		[ForeignKey("CreatedByUserId")]
		public AppUser User { get; set; }
		public string CreatedByUserId { get; set; }

		public DateTime Created { get; set; }
		public DateTime? Edited { get; set; }

		// Linked Tables
		public ICollection<Vote> Votes { get; set; }
		public UrlTip UrlTip { get; set; }
		public PostToLineup Lineup { get; set; }
		public PostToPost Conversation { get; set; }
		public Scissored Scissored { get; set; }
	}
}
