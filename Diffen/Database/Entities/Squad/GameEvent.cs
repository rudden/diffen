using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Squad
{
	using Helpers.Enum;

	public class GameEvent
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[ForeignKey("GameId")]
		public Game Game { get; set; }
		public int GameId { get; set; }

		[ForeignKey("PlayerId")]
		public Player Player { get; set; }
		public int PlayerId { get; set; }

		public GameEventType Event { get; set; }
		public int HappenedInGameMinute { get; set; }
	}
}
