using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Squad
{
	using User;

	public class Lineup
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[ForeignKey("FormationId")]
		public Formation Formation { get; set; }
		public int FormationId { get; set; }

		[ForeignKey("CreatedByUserId")]
		public AppUser User { get; set; }
		public string CreatedByUserId { get; set; }

		public ICollection<PlayerToLineup> Players { get; set; }

		public DateTime Created { get; set; }
	}
}
