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
		public string BirthDay { get; set; }
		public int HeightInCentimeters { get; set; }
		public int Weight { get; set; }
		public PreferredFoot PreferredFoot { get; set; }
		public string About { get; set; }
		public string ContractUntil { get; set; }
		public string ImageUrl { get; set; }

		public string Name => $"{FirstName[0]}. {LastName.ToUpper()}";
		public string FullName => $"{FirstName.FirstUpperCase()} {LastName.FirstUpperCase()}";

		public int KitNumber { get; set; }
		public PlayerAttributes Attributes { get; set; }

		public PlayerStatistics Statistics { get; set; }
		
		public IEnumerable<Position> AvailablePositions { get; set; }
	}

	public class PlayerStatistics
	{
		public IEnumerable<Game> GamesWithoutEvents { get; set; }
		public IEnumerable<Game> DistinctGamesWithEvents { get; set; }
		public IEnumerable<PlayerEventOnPlayer> Events { get; set; }
	}
}
