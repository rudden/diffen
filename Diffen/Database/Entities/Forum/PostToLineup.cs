using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Forum
{
	using Squad;

	public class PostToLineup
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[ForeignKey("PostId")]
		public Post Post { get; set; }
		public int PostId { get; set; }

		[ForeignKey("LineupId")]
		public Lineup Lineup { get; set; }
		public int LineupId { get; set; }

		public DateTime Created { get; set; }
	}
}
