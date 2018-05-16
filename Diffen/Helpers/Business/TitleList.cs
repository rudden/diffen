using System.Collections.Generic;

namespace Diffen.Helpers.Business
{
	using Enum;

	public static class TitleList
	{
		public static List<TitleEvent> All() => new List<TitleEvent>
		{
			new TitleEvent
			{
				Type = TitleType.League,
				Year = "1912",
				Description = "Finalseger i 2:a omspelsmatchen mot Örgryte IS med 3-1 på Råsunda IP. Detta var Djurgårdens IF:s första SM-guld."
			},
			new TitleEvent
			{
				Type = TitleType.League,
				Year = "1915",
				Description = "Finalseger mot Örgryte IS med 4-1 på Stockholms Stadion i Stockholm. Detta var Djurgårdens IF:s andra SM-guld."
			},
			new TitleEvent
			{
				Type = TitleType.League,
				Year = "1917",
				Description = "Finalseger mot AIK med 3-1 på Stockholms Stadion i Stockholm. Detta var Djurgårdens IF:s tredje SM-guld."
			},
			new TitleEvent
			{
				Type = TitleType.League,
				Year = "1920",
				Description = "Finalseger mot IK Sleipner med 1-0 på Stockholms stadion den. Detta var Djurgårdens IF:s fjärde SM-guld."
			},
			new TitleEvent
			{
				Type = TitleType.League,
				Year = "1954/1955",
				Description = "Fotbollsallsvenskan 1954/1955 vanns av Djurgårdens IF.Höstsäsongen spelades 1 augusti-7 november 1954 och vårsäsongen spelades 17 april-10 juni 1955."
			},
			new TitleEvent
			{
				Type = TitleType.League,
				Year = "1959",
				Description = "Vårsäsongen spelades 19 april-17 juni 1959 och höstsäsongen spelades 2 augusti-11 oktober 1959."
			},
			new TitleEvent
			{
				Type = TitleType.League,
				Year = "1966",
				Description = "Vårsäsongen spelades 24 april-19 juni 1966 och höstsäsongen spelades 3 augusti-30 oktober 1966."
			},
			new TitleEvent
			{
				Type = TitleType.Cup,
				Year = "1990"
			},
			new TitleEvent
			{
				Type = TitleType.League,
				Year = "2002",
				Description = "Vårsäsongen spelades 6 april-14 maj 2002 och höstsäsongen spelades 2 juli-2 november 2002."
			},
			new TitleEvent
			{
				Type = TitleType.Cup,
				Year = "2002"
			},
			new TitleEvent
			{
				Type = TitleType.League,
				Year = "2003",
				Description = "Fotbollsallsvenskan 2003 spelades 5 april-26 oktober 2003 och vanns av Djurgårdens IF."
			},
			new TitleEvent
			{
				Type = TitleType.Cup,
				Year = "2005"
			},
			new TitleEvent
			{
				Type = TitleType.League,
				Year = "2005",
				Description = "Fotbollsallsvenskan 2005 spelades 9 april-23 oktober 2005 och vanns av Djurgårdens IF."
			},
			new TitleEvent
			{
				Type = TitleType.Cup,
				Year = "2018"
			}
		};
	}

	public class TitleEvent
	{
		public TitleType Type { get; set; }
		public string Year { get; set; }
		public string Description { get; set; }
	}
}
