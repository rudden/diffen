using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Forum
{
	public class Scissored
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[ForeignKey("PostId")]
		public Post Post { get; set; }
		public int PostId { get; set; }

		public DateTime Created { get; set; }
	}
}
