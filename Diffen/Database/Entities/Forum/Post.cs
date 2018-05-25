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

		[ForeignKey("ParentPostId")]
		public virtual Post ParentPost { get; set; }
		public int? ParentPostId { get; set; }

		public DateTime Created { get; set; }
		public DateTime? Updated { get; set; }

		// Linked Tables
		public ICollection<Vote> Votes { get; set; }
		public ICollection<UrlTip> UrlTips { get; set; }
		public ICollection<PostToLineup> Lineups { get; set; }
		public Scissored Scissored { get; set; }
		public ICollection<PostToThread> InThreads { get; set; }
	}
}
