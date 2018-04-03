using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Forum
{
	public class PostToPost
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[ForeignKey("PostId")]
		public int PostId { get; set; }

		[ForeignKey("ParentPostId")]
		public int ParentPostId { get; set; }
	}
}
