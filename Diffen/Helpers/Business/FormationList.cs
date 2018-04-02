using System.Collections.Generic;

namespace Diffen.Helpers.Business
{
	public static class FormationList
	{
		public static List<FormationItem> All() => new List<FormationItem>
		{
			new FormationItem
			{
				Name = "3-5-2"
			},
			new FormationItem
			{
				Name = "3-1-4-2"
			},
			new FormationItem
			{
				Name = "3-4-1-2"
			},
			new FormationItem
			{
				Name = "3-4-3"
			},
			new FormationItem
			{
				Name = "4-4-2"
			},
			new FormationItem
			{
				Name = "4-4-1-1"
			},
			new FormationItem
			{
				Name = "4-2-3-1"
			},
			new FormationItem
			{
				Name = "4-3-2-1"
			},
			new FormationItem
			{
				Name = "4-3-3"
			},
			new FormationItem
			{
				Name = "4-1-2-1-2"
			},
			new FormationItem
			{
				Name = "4-3-1-2"
			},
			new FormationItem
			{
				Name = "4-1-4-1"
			},
			new FormationItem
			{
				Name = "4-5-1"
			},
			new FormationItem
			{
				Name = "5-3-2"
			},
			new FormationItem
			{
				Name = "5-4-1"
			},
			new FormationItem
			{
				Name = "5-2-1-2"
			}
		};
	}

	public class FormationItem
	{
		public string Name { get; set; }
		public string ComponentName { get; set; }
	}
}
