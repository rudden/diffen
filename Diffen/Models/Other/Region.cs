using System.Collections.Generic;

namespace Diffen.Models.Other
{
	public class Region
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public double Longitud { get; set; }
		public double Latitud { get; set; }
		public IEnumerable<IdAndNickNameUser> Users { get; set; }
	}
}
