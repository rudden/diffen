using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Forum
{
	public class UrlTip
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[ForeignKey("PostId")]
		public Post Post { get; set; }
		public virtual int? PostId { get; set; }

		public string Href { get; set; }
		public int Clicks { get; set; }

		public DateTime Created { get; set; }
	}
}