using System.Collections.Generic;

namespace Diffen.Models.Squad
{
	using Helpers.Enum;
	using Helpers.Extensions;

	public class Player
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int KitNumber { get; set; }
		public bool IsCaptain { get; set; }
		public bool IsViceCaptain { get; set; }
		public bool IsOutOnLoan { get; set; }
		public bool IsHereOnLoan { get; set; }
		public bool IsSold { get; set; }
		public IEnumerable<Position> AvailablePositions { get; set; }
		public IEnumerable<PlayerEventOnPlayer> Events { get; set; }

		public string BirthDay { get; set; }
		public int HeightInCentimeters { get; set; }
		public int Weight { get; set; }
		public PreferredFoot PreferredFoot { get; set; }
		public string About { get; set; }
		public string ContractUntil { get; set; }
		public string ImageUrl { get; set; }

		public int InNumberOfStartingElevens { get; set; }

		public string Name => $"{FirstName[0]}. {LastName.ToUpper()}";

		public string FullName => $"{FirstName.FirstUpperCase()} {LastName.FirstUpperCase()}";
	}
}
