using System.Collections.Generic;

namespace Diffen.Models.Squad
{
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

		public int InNumberOfStartingElevens { get; set; }

		public string Name => $"{FirstName[0]}. {LastName.ToUpper()}";

		public string FullName => $"{FirstName.FirstUpperCase()} {LastName.FirstUpperCase()}";
	}
}
