using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Squad
{
	public class PlayerToPosition
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[ForeignKey("PlayerId")]
		public Player Player { get; set; }
		public int PlayerId { get; set; }

		[ForeignKey("PositionId")]
		public Position Position { get; set; }
		public int PositionId { get; set; }
	}
}
