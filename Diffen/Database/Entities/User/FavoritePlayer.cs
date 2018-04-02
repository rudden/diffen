using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.User
{
	using Squad;

	public class FavoritePlayer
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[ForeignKey("PlayerId")]
		public Player Player { get; set; }
		public int PlayerId { get; set; }

		[ForeignKey("UserId")]
		public AppUser User { get; set; }
		public string UserId { get; set; }
	}
}