using System.Collections.Generic;

namespace Diffen.Helpers.Business
{
	public static class PositionList
	{
		public static List<PositionItem> All() => new List<PositionItem>
		{
			new PositionItem
			{
				Name = "MV",
				Types = new List<PositionType>
				{
					PositionType.None
				}
			},
			new PositionItem
			{
				Name = "HB",
				Types = new List<PositionType>
				{
					PositionType.Defence
				}
			},
			new PositionItem
			{
				Name = "HYB",
				Types = new List<PositionType>
				{
					PositionType.Defence
				}
			},
			new PositionItem
			{
				Name = "HMB",
				Types = new List<PositionType>
				{
					PositionType.Defence
				}
			},
			new PositionItem
			{
				Name = "CMB",
				Types = new List<PositionType>
				{
					PositionType.Defence
				}
			},
			new PositionItem
			{
				Name = "VMB",
				Types = new List<PositionType>
				{
					PositionType.Defence
				}
			},
			new PositionItem
			{
				Name = "VYB",
				Types = new List<PositionType>
				{
					PositionType.Defence
				}
			},
			new PositionItem
			{
				Name = "VB",
				Types = new List<PositionType>
				{
					PositionType.Defence
				}
			},
			new PositionItem
			{
				Name = "HY",
				Types = new List<PositionType>
				{
					PositionType.Attack
				}
			},
			new PositionItem
			{
				Name = "HM",
				Types = new List<PositionType>
				{
					PositionType.Midfield, PositionType.Attack
				}
			},
			new PositionItem
			{
				Name = "HCDM",
				Types = new List<PositionType>
				{
					PositionType.Defence, PositionType.Midfield
				}
			},
			new PositionItem
			{
				Name = "HCM",
				Types = new List<PositionType>
				{
					 PositionType.Midfield
				}
			},
			new PositionItem
			{
				Name = "CM",
				Types = new List<PositionType>
				{
					PositionType.Midfield
				}
			},
			new PositionItem
			{
				Name = "COM",
				Types = new List<PositionType>
				{
					PositionType.Midfield, PositionType.Attack
				}
			},
			new PositionItem
			{
				Name = "CDM",
				Types = new List<PositionType>
				{
					PositionType.Defence, PositionType.Midfield
				}
			},
			new PositionItem
			{
				Name = "VCDM",
				Types = new List<PositionType>
				{
					PositionType.Defence, PositionType.Midfield
				}
			},
			new PositionItem
			{
				Name = "VCM",
				Types = new List<PositionType>
				{
					PositionType.Midfield
				}
			},
			new PositionItem
			{
				Name = "VM",
				Types = new List<PositionType>
				{
					PositionType.Midfield, PositionType.Attack
				}
			},
			new PositionItem
			{
				Name = "VY",
				Types = new List<PositionType>
				{
					PositionType.Attack
				}
			},
			new PositionItem
			{
				Name = "VANF",
				Types = new List<PositionType>
				{
					PositionType.Attack
				}
			},
			new PositionItem
			{
				Name = "CANF",
				Types = new List<PositionType>
				{
					PositionType.Attack
				}
			},
			new PositionItem
			{
				Name = "HANF",
				Types = new List<PositionType>
				{
					PositionType.Attack
				}
			}
		};
	}

	public class PositionItem
	{
		public string Name { get; set; }
		public IEnumerable<PositionType> Types { get; set; }
	}

	public enum PositionType
	{
		None, Defence, Midfield, Attack
	}
}
