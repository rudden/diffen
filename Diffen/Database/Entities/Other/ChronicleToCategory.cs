using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Other
{
	public class ChronicleToCategory
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[ForeignKey("ChronicleId")]
		public Chronicle Chronicle { get; set; }
		public int ChronicleId { get; set; }

		[ForeignKey("CategoryId")]
		public ChronicleCategory Category { get; set; }
		public int CategoryId { get; set; }
	}
}
