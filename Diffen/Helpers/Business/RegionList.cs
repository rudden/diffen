using System.Collections.Generic;

namespace Diffen.Helpers.Business
{
	public static class RegionList
	{
		public static List<RegionItem> All() => new List<RegionItem>
		{
			new RegionItem("Kungsholmen", 59.331163, 18.040816),
			new RegionItem("Östermalm", 59.336438, 18.085318),
			new RegionItem("Södermalm", 59.311581, 18.067217),
			new RegionItem("Farsta", 59.258045, 18.083167),
			new RegionItem("Haninge", 59.127934, 18.125995),
			new RegionItem("Hässelby", 59.382339, 17.798077),
			new RegionItem("Sundbyberg", 59.367520, 17.966124),
			new RegionItem("Nacka", 59.307813, 18.155037),
			new RegionItem("Stockholm", 59.329276, 18.064449),
			new RegionItem("Vasastan", 59.344083, 18.045148),
			new RegionItem("Lidingö", 59.363581, 18.147128),
			new RegionItem("Salem", 59.207541, 17.774415),
			new RegionItem("Rågsved", 59.255765, 18.033058),
			new RegionItem("Nynäshamn", 58.903234, 17.946350),
			new RegionItem("Skarpnäck", 59.264708, 18.142203),
			new RegionItem("Täby", 59.441905, 18.070584),
			new RegionItem("Upplands Väsby", 59.520250, 17.928076),
			new RegionItem("Åkersberga", 59.480762, 18.310267),
			new RegionItem("Tyresö", 59.243781, 18.283259),
			new RegionItem("Johanneshov", 59.296181, 18.074086),
			new RegionItem("Märsta", 59.619669, 17.854878),
			new RegionItem("Danderyd", 59.408217, 18.018983),
			new RegionItem("Enskede", 59.274730, 18.074173),
			new RegionItem("Sickla Sjöstad", 59.305993, 18.113059),
			new RegionItem("Bromma", 59.339689, 17.938966),
			new RegionItem("Boston, USA", 42.377831, -71.075819),
			new RegionItem("Borås", 57.721138, 12.939854)
		};
	}

	public class RegionItem
	{
		public string Name { get; set; }
		public double Longitud { get; set; }
		public double Latitud { get; set; }

		public RegionItem(string name, double latitud, double longitud)
		{
			Name = name;
			Latitud = latitud;
			Longitud = longitud;
		}
	}
}
