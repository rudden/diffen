using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.User
{
	using Forum;

	public class SavedPost
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[ForeignKey("SavedByUserId")]
		public AppUser User { get; set; }
		public string SavedByUserId { get; set; }

		[ForeignKey("PostId")]
		public Post Post { get; set; }
		public int PostId { get; set; }

		public DateTime Created { get; set; }
	}
}
