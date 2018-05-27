using System;
using System.Collections.Generic;

namespace Diffen.Models.Squad.CRUD
{
	using Helpers.Enum;

	public class Player
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int KitNumber { get; set; }
		public PlayerAttributes Attributes { get; set; }
		public IEnumerable<int> AvailablePositionsIds { get; set; }
		public DateTime BirthDay { get; set; }
		public int HeightInCentimeters { get; set; }
		public int Weight { get; set; }
		public PreferredFoot PreferredFoot { get; set; }
		public string About { get; set; }
		public DateTime ContractUntil { get; set; }
		public string ImageUrl { get; set; }
	}
}
