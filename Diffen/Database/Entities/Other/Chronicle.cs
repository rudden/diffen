using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Other
{
	using User;

	public class Chronicle
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string Title { get; set; }
		public string Slug { get; set; }
		public string HeaderFileName { get; set; }
		public string Text { get; set; }

		[ForeignKey("WrittenByUserId")]
		public AppUser WrittenByUser { get; set; }
		public string WrittenByUserId { get; set; }

		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
	}
}
