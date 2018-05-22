using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Squad
{
	using Helpers.Enum;

	public class Player
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int KitNumber { get; set; }
		public bool IsCaptain { get; set; }
		public bool IsViceCaptain { get; set; }
		public bool IsOutOnLoan { get; set; }
		public bool IsHereOnLoan { get; set; }
		public bool IsSold { get; set; }

		public DateTime BirthDay { get; set; }
		public int HeightInCentimeters { get; set; }
		public int Weight { get; set; }
		public PreferredFoot PreferredFoot { get; set; }
		public string About { get; set; }
		public DateTime ContractUntil { get; set; }
		public string ImageUrl { get; set; }

		// Linked Tables
		public ICollection<PlayerToPosition> AvailablePositions { get; set; }
		public ICollection<PlayerToLineup> InLineups { get; set; }
		public ICollection<PlayerEvent> PlayerEvents { get; set; }
	}
}