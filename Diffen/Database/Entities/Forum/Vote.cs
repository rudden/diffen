using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Forum
{
	using User;
	using Helpers.Enum;

	public class Vote
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public VoteType Type { get; set; }

		[ForeignKey("CreatedByUserId")]
		public AppUser User { get; set; }
		public string CreatedByUserId { get; set; }

		[ForeignKey("PostId")]
		public Post Post { get; set; }
		public int PostId { get; set; }

		public DateTime Created { get; set; }
	}
}
