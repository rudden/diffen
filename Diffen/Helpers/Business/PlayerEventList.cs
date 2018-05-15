using System;
using System.Collections.Generic;
using Diffen.Helpers.Enum;

namespace Diffen.Helpers.Business
{
	public static class PlayerEventList
	{
		public static List<GameEventItem> All() => new List<GameEventItem>
		{
			new GameEventItem
			{
				Type = GameType.Cup,
				OnDate = new DateTime(2018, 02, 19),
				Events = new List<PlayerEventItem>
				{
					new PlayerEventItem
					{
						KitNumber = 9,
						GameEventType = GameEventType.Goal,
					},
					new PlayerEventItem
					{
						KitNumber = 10,
						GameEventType = GameEventType.Goal,
					},
					new PlayerEventItem
					{
						KitNumber = 15,
						GameEventType = GameEventType.Goal,
					},
					new PlayerEventItem
					{
						KitNumber = 24,
						GameEventType = GameEventType.Goal,
					},
					new PlayerEventItem
					{
						KitNumber = 24,
						GameEventType = GameEventType.Goal,
					},
					new PlayerEventItem
					{
						KitNumber = 9,
						GameEventType = GameEventType.Goal,
					}
				}
			},
			new GameEventItem
			{
				Type = GameType.Cup,
				OnDate = new DateTime(2018, 02, 24),
				Events = new List<PlayerEventItem>
				{
					new PlayerEventItem
					{
						KitNumber = 13,
						GameEventType = GameEventType.YellowCard,
					},
					new PlayerEventItem
					{
						KitNumber = 5,
						GameEventType = GameEventType.Goal,
					},
					new PlayerEventItem
					{
						KitNumber = 6,
						GameEventType = GameEventType.YellowCard,
					}
				}
			},
			new GameEventItem
			{
				Type = GameType.Cup,
				OnDate = new DateTime(2018, 03, 03),
				Events = new List<PlayerEventItem>
				{
					new PlayerEventItem
					{
						KitNumber = 8,
						GameEventType = GameEventType.Goal,
					}
				}
			},
			new GameEventItem
			{
				Type = GameType.Cup,
				OnDate = new DateTime(2018, 03, 12),
				Events = new List<PlayerEventItem>
				{
					new PlayerEventItem
					{
						KitNumber = 24,
						GameEventType = GameEventType.Goal,
					}
				}
			},
			new GameEventItem
			{
				Type = GameType.Cup,
				OnDate = new DateTime(2018, 03, 18),
				Events = new List<PlayerEventItem>
				{
					new PlayerEventItem
					{
						KitNumber = 24,
						GameEventType = GameEventType.Goal,
					},
					new PlayerEventItem
					{
						KitNumber = 24,
						GameEventType = GameEventType.Assist,
					},
					new PlayerEventItem
					{
						KitNumber = 10,
						GameEventType = GameEventType.Goal,
					},
					new PlayerEventItem
					{
						KitNumber = 15,
						GameEventType = GameEventType.YellowCard,
					}
					,
					new PlayerEventItem
					{
						KitNumber = 6,
						GameEventType = GameEventType.YellowCard,
					}
				}
			},
			new GameEventItem
			{
				Type = GameType.League,
				OnDate = new DateTime(2018, 04, 01),
				Events = new List<PlayerEventItem>
				{
					new PlayerEventItem
					{
						KitNumber = 3,
						GameEventType = GameEventType.Goal,
					},
					new PlayerEventItem
					{
						KitNumber = 8,
						GameEventType = GameEventType.YellowCard,
					},
					new PlayerEventItem
					{
						KitNumber = 3,
						GameEventType = GameEventType.YellowCard,
					}
				}
			},
			new GameEventItem
			{
				Type = GameType.League,
				OnDate = new DateTime(2018, 04, 08),
				Events = new List<PlayerEventItem>
				{
					new PlayerEventItem
					{
						KitNumber = 3,
						GameEventType = GameEventType.Goal,
					},
					new PlayerEventItem
					{
						KitNumber = 23,
						GameEventType = GameEventType.YellowCard,
					}
				}
			},
			new GameEventItem
			{
				Type = GameType.League,
				OnDate = new DateTime(2018, 04, 15),
				Events = new List<PlayerEventItem>
				{
					new PlayerEventItem
					{
						KitNumber = 13,
						GameEventType = GameEventType.YellowCard
					},
					new PlayerEventItem
					{
						KitNumber = 23,
						GameEventType = GameEventType.YellowCard
					},
					new PlayerEventItem
					{
						LastName = "Movsisyan",
						GameEventType = GameEventType.RedCard
					},
					new PlayerEventItem
					{
						KitNumber = 20,
						GameEventType = GameEventType.YellowCard
					}
				}
			},
			new GameEventItem
			{
				Type = GameType.League,
				OnDate = new DateTime(2018, 04, 18),
				Events = new List<PlayerEventItem>
				{
					new PlayerEventItem
					{
						KitNumber = 24,
						GameEventType = GameEventType.Goal
					},
					new PlayerEventItem
					{
						KitNumber = 24,
						GameEventType = GameEventType.Goal
					},
					new PlayerEventItem
					{
						KitNumber = 7,
						GameEventType = GameEventType.Goal
					},
					new PlayerEventItem
					{
						KitNumber = 24,
						GameEventType = GameEventType.Assist
					},
				}
			},
			new GameEventItem
			{
				Type = GameType.League,
				OnDate = new DateTime(2018, 04, 23),
				Events = new List<PlayerEventItem>
				{
					new PlayerEventItem
					{
						KitNumber = 11,
						GameEventType = GameEventType.Goal
					},
					new PlayerEventItem
					{
						KitNumber = 11,
						GameEventType = GameEventType.Assist
					},
					new PlayerEventItem
					{
						KitNumber = 6,
						GameEventType = GameEventType.Assist
					},
					new PlayerEventItem
					{
						KitNumber = 20,
						GameEventType = GameEventType.Goal
					},
					new PlayerEventItem
					{
						KitNumber = 15,
						GameEventType = GameEventType.YellowCard
					},
					new PlayerEventItem
					{
						KitNumber = 13,
						GameEventType = GameEventType.YellowCard
					},
				}
			},
			new GameEventItem
			{
				Type = GameType.League,
				OnDate = new DateTime(2018, 04, 29),
				Events = new List<PlayerEventItem>
				{
					new PlayerEventItem
					{
						KitNumber = 24,
						GameEventType = GameEventType.YellowCard
					},
					new PlayerEventItem
					{
						KitNumber = 4,
						GameEventType = GameEventType.YellowCard
					},
					new PlayerEventItem
					{
						KitNumber = 8,
						GameEventType = GameEventType.Goal
					}
				}
			},
			new GameEventItem
			{
				Type = GameType.League,
				OnDate = new DateTime(2018, 05, 03),
				Events = new List<PlayerEventItem>
				{
					new PlayerEventItem
					{
						KitNumber = 13,
						GameEventType = GameEventType.YellowCard
					},
					new PlayerEventItem
					{
						KitNumber = 3,
						GameEventType = GameEventType.YellowCard
					},
					new PlayerEventItem
					{
						KitNumber = 13,
						GameEventType = GameEventType.YellowCard
					},
					new PlayerEventItem
					{
						KitNumber = 13,
						GameEventType = GameEventType.RedCard
					}
				}
			},
			new GameEventItem
			{
				Type = GameType.League,
				OnDate = new DateTime(2018, 05, 06),
				Events = new List<PlayerEventItem>
				{
					new PlayerEventItem
					{
						KitNumber = 6,
						GameEventType = GameEventType.YellowCard
					}
				}
			},
			new GameEventItem
			{
				Type = GameType.Cup,
				OnDate = new DateTime(2018, 05, 10),
				Events = new List<PlayerEventItem>
				{
					new PlayerEventItem
					{
						KitNumber = 4,
						GameEventType = GameEventType.Goal
					},
					new PlayerEventItem
					{
						KitNumber = 10,
						GameEventType = GameEventType.YellowCard
					},
					new PlayerEventItem
					{
						KitNumber = 6,
						GameEventType = GameEventType.YellowCard
					},
					new PlayerEventItem
					{
						KitNumber = 10,
						GameEventType = GameEventType.Goal
					},
					new PlayerEventItem
					{
						KitNumber = 11,
						GameEventType = GameEventType.Goal
					},
					new PlayerEventItem
					{
						KitNumber = 24,
						GameEventType = GameEventType.YellowCard
					},
					new PlayerEventItem
					{
						KitNumber = 23,
						GameEventType = GameEventType.Assist
					},
					new PlayerEventItem
					{
						KitNumber = 22,
						GameEventType = GameEventType.Assist
					}
				}
			},
			new GameEventItem
			{
				Type = GameType.League,
				OnDate = new DateTime(2018, 05, 13),
				Events = new List<PlayerEventItem>
				{
					new PlayerEventItem
					{
						KitNumber = 24,
						GameEventType = GameEventType.Goal
					}
				}
			},
		};
	}

	public class GameEventItem
	{
		public GameType Type { get; set; }
		public DateTime OnDate { get; set; }
		public List<PlayerEventItem> Events { get; set; }
	}

	public class PlayerEventItem
	{
		public int KitNumber { get; set; }
		public string LastName { get; set; }
		public GameEventType GameEventType { get; set; }
	}
}
