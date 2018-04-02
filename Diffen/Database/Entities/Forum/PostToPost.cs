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
		public virtual Post Post { get; set; }
		public int? PostId { get; set; }

		[ForeignKey("ParentPostId")]
		public virtual Post ParentPost { get; set; }
		public int? ParentPostId { get; set; }
	}
}
