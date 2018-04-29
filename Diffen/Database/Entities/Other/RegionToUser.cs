using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Other
{
	using User;

	public class RegionToUser
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[ForeignKey("UserId")]
		public AppUser User { get; set; }
		public string UserId { get; set; }

		[ForeignKey("RegionId")]
		public Region Region { get; set; }
		public int RegionId { get; set; }
	}
}
