using System.Linq;
using System.Collections.Generic;

namespace Diffen.Helpers.Business
{
	public static class PollList
	{
		public static List<PollItem> All() => new List<PollItem>
		{
			new PollItem("Bästa anfallaren 2017?", new []
			{
				"Gustav Engvall",
				"Aliou Badji",
				"Tinotenda Kadewere",
				"Haruna Garba",
				"Amadou Jawo",
				"Julian Kristoffersen"
			}),
			new PollItem("Nöjd med 3e platsen 2017?", new [] { "Ja", "Nej", "Sådär" }),
			new PollItem("Topp 3 2018 eller vinna Svenska Cupen?", new []
			{
				"Topp 3",
				"Vinna Svenska Cupen",
				"Va? Vinna Allsvenskan!"
			}),
			new PollItem("Bästa nyförvärvet 2018", new []
			{
				"Marcus Danielsson",
				"Hampus Finndell",
				"Johan Andersson",
				"Yura Movsisyan",
				"Dzenis Kozica",
				"Fredrik Ulvestad",
				"Edward Chilufya"
			}),
			new PollItem("Viktigaste spelaren att förlänga med?", new []
			{
				"Andreas Isaksson",
				"Jonas Olsson",
				"Kerim Mrabti",
				"Niclas Gunnarsson"
			}),
			new PollItem("Bästa mittbackskonstellationen", new []
			{
				"Danielsson & Olsson",
				"Danielsson & Une Larsson",
				"Danielsson & Gunnarsson",
				"Olsson & Une Larsson",
				"Olsson & Gunnarsson",
				"Une Larsson & Gunnarsson"
			}),
			new PollItem("Tino eller Badji", new [] { "Tino", "Badji" })
		};
	}

	public class PollItem
	{
		public string Name { get; set; }
		public List<PollSelectionItem> Selections { get; set; }

		public PollItem(string name, string[] selections)
		{
			Name = name;
			Selections = selections.Select(selection => new PollSelectionItem(selection)).ToList();
		}
	}

	public class PollSelectionItem
	{
		public string Name { get; set; }

		public PollSelectionItem(string name)
		{
			Name = name;
		}
	}
}
