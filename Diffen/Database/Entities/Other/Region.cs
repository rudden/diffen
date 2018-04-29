using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diffen.Database.Entities.Other
{
	public class Region
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string Name { get; set; }
		public double Longitud { get; set; }
		public double Latitud { get; set; }

		public DateTime Created { get; set; }

		// Linked Tables
		public ICollection<RegionToUser> UsersInRegion { get; set; }
	}
}
