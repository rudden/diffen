using System.Collections.Generic;

namespace Diffen.Helpers.Business
{
	public static class FormationList
	{
		public static List<FormationItem> All() => new List<FormationItem>
		{
			new FormationItem
			{
				Name = "3-5-2",
				ComponentName = "ThreeFiveTwo",
				Positions = new List<PositionItem>
				{
					PositionList.All().Find(p => p.Name == "MV"),
					PositionList.All().Find(p => p.Name == "VMB"),
					PositionList.All().Find(p => p.Name == "CMB"),
					PositionList.All().Find(p => p.Name == "HMB"),
					PositionList.All().Find(p => p.Name == "VCM"),
					PositionList.All().Find(p => p.Name == "CM"),
					PositionList.All().Find(p => p.Name == "HCM"),
					PositionList.All().Find(p => p.Name == "VYB"),
					PositionList.All().Find(p => p.Name == "HYB"),
					PositionList.All().Find(p => p.Name == "VANF"),
					PositionList.All().Find(p => p.Name == "HANF")
				}
			},
			new FormationItem
			{
				Name = "3-1-4-2",
				ComponentName = "ThreeOneFourTwo",
				Positions = new List<PositionItem>
				{
					PositionList.All().Find(p => p.Name == "MV"),
					PositionList.All().Find(p => p.Name == "VMB"),
					PositionList.All().Find(p => p.Name == "CMB"),
					PositionList.All().Find(p => p.Name == "HMB"),
					PositionList.All().Find(p => p.Name == "CDM"),
					PositionList.All().Find(p => p.Name == "VYB"),
					PositionList.All().Find(p => p.Name == "VCM"),
					PositionList.All().Find(p => p.Name == "HCM"),
					PositionList.All().Find(p => p.Name == "HYB"),
					PositionList.All().Find(p => p.Name == "VANF"),
					PositionList.All().Find(p => p.Name == "HANF")
				}
			},
			new FormationItem
			{
				Name = "3-4-1-2",
				ComponentName = "ThreeFourOneTwo",
				Positions = new List<PositionItem>
				{
					PositionList.All().Find(p => p.Name == "MV"),
					PositionList.All().Find(p => p.Name == "VMB"),
					PositionList.All().Find(p => p.Name == "CMB"),
					PositionList.All().Find(p => p.Name == "HMB"),
					PositionList.All().Find(p => p.Name == "VCDM"),
					PositionList.All().Find(p => p.Name == "HCDM"),
					PositionList.All().Find(p => p.Name == "VYB"),
					PositionList.All().Find(p => p.Name == "HYB"),
					PositionList.All().Find(p => p.Name == "COM"),
					PositionList.All().Find(p => p.Name == "VANF"),
					PositionList.All().Find(p => p.Name == "HANF")
				}
			},
			new FormationItem
			{
				Name = "3-4-3",
				ComponentName = "ThreeFourThree",
				Positions = new List<PositionItem>
				{
					PositionList.All().Find(p => p.Name == "MV"),
					PositionList.All().Find(p => p.Name == "VMB"),
					PositionList.All().Find(p => p.Name == "CMB"),
					PositionList.All().Find(p => p.Name == "HMB"),
					PositionList.All().Find(p => p.Name == "VCDM"),
					PositionList.All().Find(p => p.Name == "HCDM"),
					PositionList.All().Find(p => p.Name == "VYB"),
					PositionList.All().Find(p => p.Name == "HYB"),
					PositionList.All().Find(p => p.Name == "VY"),
					PositionList.All().Find(p => p.Name == "HY"),
					PositionList.All().Find(p => p.Name == "CANF")
				}
			},
			new FormationItem
			{
				Name = "4-4-2",
				ComponentName = "FourFourTwo",
				Positions = new List<PositionItem>
				{
					PositionList.All().Find(p => p.Name == "MV"),
					PositionList.All().Find(p => p.Name == "VB"),
					PositionList.All().Find(p => p.Name == "VMB"),
					PositionList.All().Find(p => p.Name == "HMB"),
					PositionList.All().Find(p => p.Name == "HB"),
					PositionList.All().Find(p => p.Name == "VM"),
					PositionList.All().Find(p => p.Name == "VCM"),
					PositionList.All().Find(p => p.Name == "HCM"),
					PositionList.All().Find(p => p.Name == "HM"),
					PositionList.All().Find(p => p.Name == "VANF"),
					PositionList.All().Find(p => p.Name == "HANF")
				}
			},
			new FormationItem
			{
				Name = "4-4-1-1",
				ComponentName = "FourFourOneOne",
				Positions = new List<PositionItem>
				{
					PositionList.All().Find(p => p.Name == "MV"),
					PositionList.All().Find(p => p.Name == "VB"),
					PositionList.All().Find(p => p.Name == "VMB"),
					PositionList.All().Find(p => p.Name == "HMB"),
					PositionList.All().Find(p => p.Name == "HB"),
					PositionList.All().Find(p => p.Name == "VM"),
					PositionList.All().Find(p => p.Name == "VCM"),
					PositionList.All().Find(p => p.Name == "HCM"),
					PositionList.All().Find(p => p.Name == "HM"),
					PositionList.All().Find(p => p.Name == "COM"),
					PositionList.All().Find(p => p.Name == "CANF")
				}
			},
			new FormationItem
			{
				Name = "4-2-3-1",
				ComponentName = "FourTwoThreeOne",
				Positions = new List<PositionItem>
				{
					PositionList.All().Find(p => p.Name == "MV"),
					PositionList.All().Find(p => p.Name == "VB"),
					PositionList.All().Find(p => p.Name == "VMB"),
					PositionList.All().Find(p => p.Name == "HMB"),
					PositionList.All().Find(p => p.Name == "HB"),
					PositionList.All().Find(p => p.Name == "VM"),
					PositionList.All().Find(p => p.Name == "VCDM"),
					PositionList.All().Find(p => p.Name == "HCDM"),
					PositionList.All().Find(p => p.Name == "HM"),
					PositionList.All().Find(p => p.Name == "COM"),
					PositionList.All().Find(p => p.Name == "CANF")
				}
			},
			new FormationItem
			{
				Name = "4-3-2-1",
				ComponentName = "FourThreeTwoOne",
				Positions = new List<PositionItem>
				{
					PositionList.All().Find(p => p.Name == "MV"),
					PositionList.All().Find(p => p.Name == "VB"),
					PositionList.All().Find(p => p.Name == "VMB"),
					PositionList.All().Find(p => p.Name == "HMB"),
					PositionList.All().Find(p => p.Name == "HB"),
					PositionList.All().Find(p => p.Name == "VM"),
					PositionList.All().Find(p => p.Name == "VCM"),
					PositionList.All().Find(p => p.Name == "CM"),
					PositionList.All().Find(p => p.Name == "HCM"),
					PositionList.All().Find(p => p.Name == "HM"),
					PositionList.All().Find(p => p.Name == "CANF")
				}
			},
			new FormationItem
			{
				Name = "4-3-3",
				ComponentName = "FourThreeThree",
				Positions = new List<PositionItem>
				{
					PositionList.All().Find(p => p.Name == "MV"),
					PositionList.All().Find(p => p.Name == "VB"),
					PositionList.All().Find(p => p.Name == "VMB"),
					PositionList.All().Find(p => p.Name == "HMB"),
					PositionList.All().Find(p => p.Name == "HB"),
					PositionList.All().Find(p => p.Name == "VY"),
					PositionList.All().Find(p => p.Name == "VCM"),
					PositionList.All().Find(p => p.Name == "CM"),
					PositionList.All().Find(p => p.Name == "HCM"),
					PositionList.All().Find(p => p.Name == "HY"),
					PositionList.All().Find(p => p.Name == "CANF")
				}
			},
			new FormationItem
			{
				Name = "4-1-2-1-2",
				ComponentName = "FourOneTwoOneTwo",
				Positions = new List<PositionItem>
				{
					PositionList.All().Find(p => p.Name == "MV"),
					PositionList.All().Find(p => p.Name == "VB"),
					PositionList.All().Find(p => p.Name == "VMB"),
					PositionList.All().Find(p => p.Name == "HMB"),
					PositionList.All().Find(p => p.Name == "HB"),
					PositionList.All().Find(p => p.Name == "VCM"),
					PositionList.All().Find(p => p.Name == "CDM"),
					PositionList.All().Find(p => p.Name == "HCM"),
					PositionList.All().Find(p => p.Name == "COM"),
					PositionList.All().Find(p => p.Name == "VANF"),
					PositionList.All().Find(p => p.Name == "HANF")
				}
			},
			new FormationItem
			{
				Name = "4-3-1-2",
				ComponentName = "FourThreeOneTwo",
				Positions = new List<PositionItem>
				{
					PositionList.All().Find(p => p.Name == "MV"),
					PositionList.All().Find(p => p.Name == "VB"),
					PositionList.All().Find(p => p.Name == "VMB"),
					PositionList.All().Find(p => p.Name == "HMB"),
					PositionList.All().Find(p => p.Name == "HB"),
					PositionList.All().Find(p => p.Name == "VCM"),
					PositionList.All().Find(p => p.Name == "CM"),
					PositionList.All().Find(p => p.Name == "HCM"),
					PositionList.All().Find(p => p.Name == "COM"),
					PositionList.All().Find(p => p.Name == "VANF"),
					PositionList.All().Find(p => p.Name == "HANF")
				}
			},
			new FormationItem
			{
				Name = "4-1-4-1",
				ComponentName = "FourOneFourOne",
				Positions = new List<PositionItem>
				{
					PositionList.All().Find(p => p.Name == "MV"),
					PositionList.All().Find(p => p.Name == "VB"),
					PositionList.All().Find(p => p.Name == "VMB"),
					PositionList.All().Find(p => p.Name == "HMB"),
					PositionList.All().Find(p => p.Name == "HB"),
					PositionList.All().Find(p => p.Name == "VM"),
					PositionList.All().Find(p => p.Name == "VCM"),
					PositionList.All().Find(p => p.Name == "CDM"),
					PositionList.All().Find(p => p.Name == "HCM"),
					PositionList.All().Find(p => p.Name == "HM"),
					PositionList.All().Find(p => p.Name == "CANF")
				}
			},
			new FormationItem
			{
				Name = "4-5-1",
				ComponentName = "FourFiveOne",
				Positions = new List<PositionItem>
				{
					PositionList.All().Find(p => p.Name == "MV"),
					PositionList.All().Find(p => p.Name == "VB"),
					PositionList.All().Find(p => p.Name == "VMB"),
					PositionList.All().Find(p => p.Name == "HMB"),
					PositionList.All().Find(p => p.Name == "HB"),
					PositionList.All().Find(p => p.Name == "VM"),
					PositionList.All().Find(p => p.Name == "VCM"),
					PositionList.All().Find(p => p.Name == "CM"),
					PositionList.All().Find(p => p.Name == "HCM"),
					PositionList.All().Find(p => p.Name == "HM"),
					PositionList.All().Find(p => p.Name == "CANF")
				}
			},
			new FormationItem
			{
				Name = "5-3-2",
				ComponentName = "FiveThreeTwo",
				Positions = new List<PositionItem>
				{
					PositionList.All().Find(p => p.Name == "MV"),
					PositionList.All().Find(p => p.Name == "VYB"),
					PositionList.All().Find(p => p.Name == "VMB"),
					PositionList.All().Find(p => p.Name == "CMB"),
					PositionList.All().Find(p => p.Name == "HMB"),
					PositionList.All().Find(p => p.Name == "HYB"),
					PositionList.All().Find(p => p.Name == "VCM"),
					PositionList.All().Find(p => p.Name == "CM"),
					PositionList.All().Find(p => p.Name == "HCM"),
					PositionList.All().Find(p => p.Name == "VANF"),
					PositionList.All().Find(p => p.Name == "HANF")
				}
			},
			new FormationItem
			{
				Name = "5-4-1",
				ComponentName = "FiveFourOne",
				Positions = new List<PositionItem>
				{
					PositionList.All().Find(p => p.Name == "MV"),
					PositionList.All().Find(p => p.Name == "VYB"),
					PositionList.All().Find(p => p.Name == "VMB"),
					PositionList.All().Find(p => p.Name == "CMB"),
					PositionList.All().Find(p => p.Name == "HMB"),
					PositionList.All().Find(p => p.Name == "HYB"),
					PositionList.All().Find(p => p.Name == "VM"),
					PositionList.All().Find(p => p.Name == "VCM"),
					PositionList.All().Find(p => p.Name == "HCM"),
					PositionList.All().Find(p => p.Name == "HM"),
					PositionList.All().Find(p => p.Name == "CANF")
				}
			},
			new FormationItem
			{
				Name = "5-2-1-2",
				ComponentName = "FiveTwoOneTwo",
				Positions = new List<PositionItem>
				{
					PositionList.All().Find(p => p.Name == "MV"),
					PositionList.All().Find(p => p.Name == "VYB"),
					PositionList.All().Find(p => p.Name == "VMB"),
					PositionList.All().Find(p => p.Name == "CMB"),
					PositionList.All().Find(p => p.Name == "HMB"),
					PositionList.All().Find(p => p.Name == "HYB"),
					PositionList.All().Find(p => p.Name == "VCDM"),
					PositionList.All().Find(p => p.Name == "HCDM"),
					PositionList.All().Find(p => p.Name == "COM"),
					PositionList.All().Find(p => p.Name == "VANF"),
					PositionList.All().Find(p => p.Name == "HANF")
				}
			}
		};
	}

	public class FormationItem
	{
		public string Name { get; set; }
		public string ComponentName { get; set; }
		public IEnumerable<PositionItem> Positions { get; set; }
	}
}
