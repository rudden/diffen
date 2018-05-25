using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Forum
{
	public class PostToThread
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[ForeignKey("PostId")]
		public Post Post { get; set; }
		public int PostId { get; set; }

		[ForeignKey("ThreadId")]
		public Thread Thread { get; set; }
		public int ThreadId { get; set; }
	}
}
