using System.Collections.Generic;

namespace Diffen.Helpers.Business
{
	public static class PollList
	{
		public static List<PollItem> All() => new List<PollItem>
		{
			new PollItem
			{
				Name = "Bästa anfallaren 2017?",
				Selections = new List<PollSelectionItem>
				{
					new PollSelectionItem
					{
						Name = "Gustav Engvall"
					},
					new PollSelectionItem
					{
						Name = "Aliou Badji"
					},
					new PollSelectionItem
					{
						Name = "Tinotenda Kadewere"
					},
					new PollSelectionItem
					{
						Name = "Haruna Garba"
					},
					new PollSelectionItem
					{
						Name = "Amadou Jawo"
					}
				}
			},
			new PollItem
			{
				Name = "Nöjd med 3e platsen 2017?",
				Selections = new List<PollSelectionItem>
				{
					new PollSelectionItem
					{
						Name = "Ja"
					},
					new PollSelectionItem
					{
						Name = "Nej"
					},
					new PollSelectionItem
					{
						Name = "Sådär"
					}
				}
			},
			new PollItem
			{
				Name = "Topp 3 2018 eller vinna Svenska Cupen?",
				Selections = new List<PollSelectionItem>
				{
					new PollSelectionItem
					{
						Name = "Topp 3"
					},
					new PollSelectionItem
					{
						Name = "Vinna Svenska Cupen"
					},
					new PollSelectionItem
					{
						Name = "Va? Vinna Allsvenskan!"
					}
				}
			},
			new PollItem
			{
				Name = "Bästa nyförvärvet 2018",
				Selections = new List<PollSelectionItem>
				{
					new PollSelectionItem
					{
						Name = "Marcus Danielsson"
					},
					new PollSelectionItem
					{
						Name = "Hampus Finndell"
					},
					new PollSelectionItem
					{
						Name = "Johan Andersson"
					},
					new PollSelectionItem
					{
						Name = "Yura Movsisyan"
					},
					new PollSelectionItem
					{
						Name = "Dzenis Kozica"
					},
					new PollSelectionItem
					{
						Name = "Fredrik Ulvestad"
					},
					new PollSelectionItem
					{
						Name = "Edward Chilufya"
					}
				}
			},
			new PollItem
			{
				Name = "Viktigaste spelaren att förlänga med?",
				Selections = new List<PollSelectionItem>
				{
					new PollSelectionItem
					{
						Name = "Andreas Isaksson"
					},
					new PollSelectionItem
					{
						Name = "Jonas Olsson"
					},
					new PollSelectionItem
					{
						Name = "Kerim Mrabti"
					},
					new PollSelectionItem
					{
						Name = "Niclas Gunnarsson"
					}
				}
			},
			new PollItem
			{
				Name = "Bästa mittbackskonstellationen?",
				Selections = new List<PollSelectionItem>
				{
					new PollSelectionItem
					{
						Name = "Danielsson & Olsson"
					},
					new PollSelectionItem
					{
						Name = "Danielsson & Une Larsson"
					},
					new PollSelectionItem
					{
						Name = "Danielsson & Gunnarsson"
					},
					new PollSelectionItem
					{
						Name = "Olsson & Une Larsson"
					},
					new PollSelectionItem
					{
						Name = "Olsson & Gunnarsson"
					},
					new PollSelectionItem
					{
						Name = "Une Larsson & Gunnarsson"
					}
				}
			}
		};
	}

	public class PollItem
	{
		public string Name { get; set; }
		public List<PollSelectionItem> Selections { get; set; }
	}

	public class PollSelectionItem
	{
		public string Name { get; set; }
	}
}
