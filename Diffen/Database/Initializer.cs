using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Slugify;
using Newtonsoft.Json;

namespace Diffen.Database
{
	using Helpers;
	using Helpers.Enum;
	using Helpers.Business;
	using Helpers.Extensions;

	using Entities.User;
	using Entities.Squad;
	using Entities.Forum;
	using Entities.Other;

	internal class FillTextNickName
	{
		public string NickName { get; set; }
	}

	public class Initializer
	{
		private const int NumberOfUsers = 20;
		private const int NumberOfPosts = 50;

		public static async Task SeedAsync(IServiceProvider services)
		{
			var dbContext = services.GetRequiredService<DiffenDbContext>();
			var userManager = services.GetRequiredService<UserManager<AppUser>>();
			var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

			await SeedUsersAndNickNamesAsync(dbContext, userManager);
			await SeedRolesAsync(roleManager);
			await SeedUsersToRolesAsync(userManager);
			await SeedPositionsAsync(dbContext);
			await SeedFormationsAsync(dbContext);
			await SeedPlayersAsync(dbContext);
			await SeedAvailablePositionsToPlayers(dbContext);
			await SeedInvitesAsync(dbContext);
			await SeedPostsAsync(dbContext);
			await SeedConversationsAsync(dbContext);
			await SeedLineupsAsync(dbContext);
			await SeedUrlTipsAsync(dbContext);
			await SeedLineupsToPostsAsync(dbContext);
			await SeedPersonalMessagesAsync(dbContext);
			await SeedFavoritePlayerToUsers(dbContext);
			await SeedSavedPostsAsync(dbContext);
			await SeedVotesOnPostsAsync(dbContext);
			await SeedUserFiltersAsync(dbContext);
			await SeedScissoredPostsAsync(dbContext);
			await SeedPollsAsync(dbContext);
			await SeedRegionsAsync(dbContext);
			await SeedChroniclesAsync(dbContext, userManager);
		}

		private static async Task<string[]> GetRandomNickNamesFromFillTextApiAsync()
		{
			using (var client = new HttpClient())
			{
				var uri = "http://www.filltext.com/?rows=&nickName={username}&pretty=true".Replace("rows=", $"rows={NumberOfUsers}");
				var response = await client.GetAsync(uri);

				var nickNames = JsonConvert.DeserializeObject<IEnumerable<FillTextNickName>>(await response.Content.ReadAsStringAsync());
				return nickNames.Select(x => x.NickName).ToArray();
			}
		}

		private static async Task<string[]> GetRandomTextAsync(int amount)
		{
			using (var client = new HttpClient())
			{
				var uri = "https://baconipsum.com/api/?type=all-meat&paras=".Replace("paras=", $"paras={amount}");
				var response = await client.GetAsync(uri);
				return JsonConvert.DeserializeObject<string[]>(await response.Content.ReadAsStringAsync());
			}
		}

		private static async Task SeedUsersAndNickNamesAsync(DiffenDbContext dbContext, UserManager<AppUser> userManager)
		{
			if (userManager.Users.Any())
				return;

			var nickNames = await GetRandomNickNamesFromFillTextApiAsync();

			for (var i = 0; i < NumberOfUsers; i++)
			{
				var user = new AppUser
				{
					Email = $"seeded_user_{i}@diffen.se",
					UserName = $"seeded_user_{i}@diffen.se",
					Joined = RandomDateTime.Get(new DateTime(2017, 12, 1), DateTime.Now)
				};
				await userManager.CreateAsync(user, "P@ssw0rd!");
				await dbContext.SaveChangesAsync();

				var nickName = new NickName
				{
					UserId = user.Id,
					Nick = nickNames[i],
					Created = user.Joined
				};
				dbContext.NickNames.Add(nickName);
				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
		{
			if (!await roleManager.RoleExistsAsync("Admin"))
			{
				var role = new IdentityRole
				{
					Name = "Admin"
				};
				await roleManager.CreateAsync(role);
			}
			if (!await roleManager.RoleExistsAsync("Scissor"))
			{
				var role = new IdentityRole
				{
					Name = "Scissor"
				};
				await roleManager.CreateAsync(role);
			}
			if (!await roleManager.RoleExistsAsync("Author"))
			{
				var role = new IdentityRole
				{
					Name = "Author"
				};
				await roleManager.CreateAsync(role);
			}
		}

		private static async Task SeedUsersToRolesAsync(UserManager<AppUser> userManager)
		{
			var admins = await userManager.GetUsersInRoleAsync("Admin");
			if (!admins.Any())
			{
				var administrator = userManager.Users.OrderBy(x => x.Joined).FirstOrDefault();
				await userManager.AddToRoleAsync(administrator, "Admin");
			}

			var scissors = await userManager.GetUsersInRoleAsync("Scissor");
			if (!scissors.Any())
			{
				var newScissors = userManager.Users.ToList().PickRandom(4);
				foreach (var scissor in newScissors)
				{
					await userManager.AddToRoleAsync(scissor, "Scissor");
				}
			}

			var authors = await userManager.GetUsersInRoleAsync("Author");
			if (!authors.Any())
			{
				var newAuthors = userManager.Users.ToList().PickRandom(5);
				foreach (var author in newAuthors)
				{
					await userManager.AddToRoleAsync(author, "Author");
				}
			}
		}

		private static async Task SeedPlayersAsync(DiffenDbContext dbContext)
		{
			if (!dbContext.Players.Any())
			{
				var players = new List<Player>
				{
					// Goalkeepers
					new Player
					{
						FirstName = "Andreas",
						LastName = "Isaksson",
						KitNumber = 1
					},
					new Player
					{
						FirstName = "Tommy",
						LastName = "Vaiho",
						KitNumber = 30
					},
					new Player
					{
						FirstName = "Oscar",
						LastName = "Jonsson",
						KitNumber = 35
					},
					// Defenders
					new Player
					{
						FirstName = "Felix",
						LastName = "Beijmo",
						KitNumber = 22
					},
					new Player
					{
						FirstName = "Johan",
						LastName = "Andersson",
						KitNumber = 2
					},
					new Player
					{
						FirstName = "Jacob",
						LastName = "Une Larsson",
						KitNumber = 4
					},
					new Player
					{
						FirstName = "Niclas",
						LastName = "Gunnarsson",
						KitNumber = 5
					},
					new Player
					{
						FirstName = "Jonas",
						LastName = "Olsson",
						KitNumber = 13,
						IsCaptain = true
					},
					new Player
					{
						FirstName = "Marcus",
						LastName = "Danielsson",
						KitNumber = 3
					},
					new Player
					{
						FirstName = "Jonathan",
						LastName = "Augustinsson",
						KitNumber = 15
					},
					// Midfielders
					new Player
					{
						FirstName = "Haris",
						LastName = "Radetinac",
						KitNumber = 9
					},
					new Player
					{
						FirstName = "Jonathan",
						LastName = "Ring",
						KitNumber = 11
					},
					new Player
					{
						FirstName = "Dzenis",
						LastName = "Kozica",
						KitNumber = 7
					},
					new Player
					{
						FirstName = "Hampus",
						LastName = "Finndell",
						KitNumber = 17
					},
					new Player
					{
						FirstName = "Kevin",
						LastName = "Walker",
						KitNumber = 8
					},
					new Player
					{
						FirstName = "Jesper",
						LastName = "Karlström",
						KitNumber = 6
					},
					new Player
					{
						FirstName = "Fredrik",
						LastName = "Ulvestad",
						KitNumber = 23
					},
					new Player
					{
						FirstName = "Besard",
						LastName = "Sabovic",
						KitNumber = 14
					},
					new Player
					{
						FirstName = "Edward",
						LastName = "Chilufya",
						KitNumber = 18
					},
					// Attackers
					new Player
					{
						FirstName = "Tinotenda",
						LastName = "Kadewere",
						KitNumber = 24
					},
					new Player
					{
						FirstName = "Aliou",
						LastName = "Badji",
						KitNumber = 20
					},
					new Player
					{
						FirstName = "Kerim",
						LastName = "Mrabti",
						KitNumber = 10
					},
					new Player
					{
						FirstName = "Julian",
						LastName = "Kristoffersen",
						KitNumber = 21
					},
					// Out on loan
					new Player
					{
						FirstName = "Souleymane",
						LastName = "Kone",
						KitNumber = 0,
						IsOutOnLoan = true
					},
					new Player
					{
						FirstName = "Marcus",
						LastName = "Hansson",
						KitNumber = 0,
						IsOutOnLoan = true
					},
					new Player
					{
						FirstName = "Amadou",
						LastName = "Jawo",
						KitNumber = 0,
						IsOutOnLoan = true
					},
					new Player
					{
						FirstName = "Mihlali",
						LastName = "Mayambela",
						KitNumber = 0,
						IsOutOnLoan = true
					},
					new Player
					{
						FirstName = "Joseph",
						LastName = "Ceesay",
						KitNumber = 0,
						IsOutOnLoan = true
					},
					new Player
					{
						FirstName = "Haruna",
						LastName = "Garba",
						KitNumber = 0,
						IsOutOnLoan = true
					},
					// Is here on loan
					new Player
					{
						FirstName = "Yura",
						LastName = "Movsisyan",
						KitNumber = 0,
						IsHereOnLoan = true
					},
				};
				dbContext.Players.AddRange(players);
				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedAvailablePositionsToPlayers(DiffenDbContext dbContext)
		{
			if (!dbContext.PlayersToPositions.Any())
			{
				var defendingPositions = PositionList.All().Where(p => p.Types.Contains(PositionType.Defence))
					.Select(position => dbContext.Positions.FirstOrDefault(x => x.Name == position.Name)).ToList();
				var midfieldPositions = PositionList.All().Where(p => p.Types.Contains(PositionType.Midfield))
					.Select(position => dbContext.Positions.FirstOrDefault(x => x.Name == position.Name)).ToList();
				var attackingPositions = PositionList.All().Where(p => p.Types.Contains(PositionType.Attack))
					.Select(position => dbContext.Positions.FirstOrDefault(x => x.Name == position.Name)).ToList();

				var andreasIsaksson = dbContext.Players.FirstOrDefault(x => x.KitNumber == 1);
				dbContext.PlayersToPositions.Add(new PlayerToPosition
				{
					PlayerId = andreasIsaksson.Id,
					PositionId = dbContext.Positions.FirstOrDefault(p => p.Name == "MV").Id
				});
				var tommyVaiho = dbContext.Players.FirstOrDefault(x => x.KitNumber == 30);
				dbContext.PlayersToPositions.Add(new PlayerToPosition
				{
					PlayerId = tommyVaiho.Id,
					PositionId = dbContext.Positions.FirstOrDefault(p => p.Name == "MV").Id
				});
				var oscarJonsson = dbContext.Players.FirstOrDefault(x => x.KitNumber == 35);
				dbContext.PlayersToPositions.Add(new PlayerToPosition
				{
					PlayerId = oscarJonsson.Id,
					PositionId = dbContext.Positions.FirstOrDefault(p => p.Name == "MV").Id
				});
				var felixBeijmo = dbContext.Players.FirstOrDefault(x => x.KitNumber == 22);
				foreach (var position in defendingPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = felixBeijmo.Id,
						PositionId = position.Id
					});
				}
				var johanAndersson = dbContext.Players.FirstOrDefault(x => x.KitNumber == 2);
				foreach (var position in defendingPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = johanAndersson.Id,
						PositionId = position.Id
					});
				}
				var jacobUneLarsson = dbContext.Players.FirstOrDefault(x => x.KitNumber == 4);
				foreach (var position in defendingPositions.Where(x => !x.Name.Contains("Y")))
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = jacobUneLarsson.Id,
						PositionId = position.Id
					});
				}
				var niclasGunnarsson = dbContext.Players.FirstOrDefault(x => x.KitNumber == 5);
				foreach (var position in defendingPositions.Where(x => !x.Name.Contains("Y")))
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = niclasGunnarsson.Id,
						PositionId = position.Id
					});
				}
				var jonasOlsson = dbContext.Players.FirstOrDefault(x => x.KitNumber == 13);
				foreach (var position in defendingPositions.Where(x => !x.Name.Contains("Y")))
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = jonasOlsson.Id,
						PositionId = position.Id
					});
				}
				var marcusDanielsson = dbContext.Players.FirstOrDefault(x => x.KitNumber == 3);
				foreach (var position in defendingPositions.Where(x => !x.Name.Contains("Y")))
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = marcusDanielsson.Id,
						PositionId = position.Id
					});
				}
				var jonathanAugustinsson = dbContext.Players.FirstOrDefault(x => x.KitNumber == 15);
				foreach (var position in defendingPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = jonathanAugustinsson.Id,
						PositionId = position.Id
					});
				}
				var harisRadetinac = dbContext.Players.FirstOrDefault(x => x.KitNumber == 9);
				foreach (var position in attackingPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = harisRadetinac.Id,
						PositionId = position.Id
					});
				}
				foreach (var position in defendingPositions.Where(x => x.Name.Contains("Y")))
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = harisRadetinac.Id,
						PositionId = position.Id
					});
				}
				var jonathanRing = dbContext.Players.FirstOrDefault(x => x.KitNumber == 11);
				foreach (var position in attackingPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = jonathanRing.Id,
						PositionId = position.Id
					});
				}
				foreach (var position in defendingPositions.Where(x => x.Name.Contains("Y")))
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = jonathanRing.Id,
						PositionId = position.Id
					});
				}
				var dzenisKozica = dbContext.Players.FirstOrDefault(x => x.KitNumber == 7);
				foreach (var position in attackingPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = dzenisKozica.Id,
						PositionId = position.Id
					});
				}
				var hampusFinndell = dbContext.Players.FirstOrDefault(x => x.KitNumber == 17);
				foreach (var position in midfieldPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = hampusFinndell.Id,
						PositionId = position.Id
					});
				}
				var kevinWalker = dbContext.Players.FirstOrDefault(x => x.KitNumber == 8);
				foreach (var position in midfieldPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = kevinWalker.Id,
						PositionId = position.Id
					});
				}
				var jesperKarlstrom = dbContext.Players.FirstOrDefault(x => x.KitNumber == 6);
				foreach (var position in midfieldPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = jesperKarlstrom.Id,
						PositionId = position.Id
					});
				}
				var fredrikUlvestad = dbContext.Players.FirstOrDefault(x => x.KitNumber == 23);
				foreach (var position in midfieldPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = fredrikUlvestad.Id,
						PositionId = position.Id
					});
				}
				var besardSabovic = dbContext.Players.FirstOrDefault(x => x.KitNumber == 14);
				foreach (var position in midfieldPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = besardSabovic.Id,
						PositionId = position.Id
					});
				}
				var edwardChilufya = dbContext.Players.FirstOrDefault(x => x.KitNumber == 18);
				foreach (var position in attackingPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = edwardChilufya.Id,
						PositionId = position.Id
					});
				}
				var tinotendaKadewere = dbContext.Players.FirstOrDefault(x => x.KitNumber == 24);
				foreach (var position in attackingPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = tinotendaKadewere.Id,
						PositionId = position.Id
					});
				}
				var aliouBadji = dbContext.Players.FirstOrDefault(x => x.KitNumber == 20);
				foreach (var position in attackingPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = aliouBadji.Id,
						PositionId = position.Id
					});
				}
				var kerimMrabti = dbContext.Players.FirstOrDefault(x => x.KitNumber == 10);
				foreach (var position in attackingPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = kerimMrabti.Id,
						PositionId = position.Id
					});
				}
				foreach (var position in midfieldPositions.Where(x => !x.Name.Contains("D")))
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = kerimMrabti.Id,
						PositionId = position.Id
					});
				}
				var julianKristoffersen = dbContext.Players.FirstOrDefault(x => x.KitNumber == 21);
				foreach (var position in attackingPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = julianKristoffersen.Id,
						PositionId = position.Id
					});
				}
				var souleymaneKone = dbContext.Players.FirstOrDefault(x => x.LastName == "Kone");
				foreach (var position in defendingPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = souleymaneKone.Id,
						PositionId = position.Id
					});
				}
				var marcusHansson = dbContext.Players.FirstOrDefault(x => x.FirstName == "Marcus" && x.LastName == "Hansson");
				foreach (var position in defendingPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = marcusHansson.Id,
						PositionId = position.Id
					});
				}
				var amadouJawo = dbContext.Players.FirstOrDefault(x => x.FirstName == "Amadou");
				foreach (var position in attackingPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = amadouJawo.Id,
						PositionId = position.Id
					});
				}
				var mihlaliMayambela = dbContext.Players.FirstOrDefault(x => x.FirstName == "Mihlali");
				foreach (var position in attackingPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = mihlaliMayambela.Id,
						PositionId = position.Id
					});
				}
				var josephCeesay = dbContext.Players.FirstOrDefault(x => x.FirstName == "Joseph");
				foreach (var position in attackingPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = josephCeesay.Id,
						PositionId = position.Id
					});
				}
				var harunaGarba = dbContext.Players.FirstOrDefault(x => x.FirstName == "Haruna");
				foreach (var position in attackingPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = harunaGarba.Id,
						PositionId = position.Id
					});
				}
				var yuraMovsisyan = dbContext.Players.FirstOrDefault(x => x.FirstName == "Yura");
				foreach (var position in attackingPositions)
				{
					dbContext.PlayersToPositions.Add(new PlayerToPosition
					{
						PlayerId = yuraMovsisyan.Id,
						PositionId = position.Id
					});
				}
				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedInvitesAsync(DiffenDbContext dbContext)
		{
			if (!dbContext.Invites.Any())
			{
				for (var i = 1; i <= 20; i++)
				{
					var invite = new Invite
					{
						Email = $"seeded_invite_user_{i}@diffen.se",
						InvitedByUserId = dbContext.Users.PickRandom().Id,
						InviteSent = DateTime.Now
					};
					dbContext.Invites.Add(invite);
				}
				foreach (var user in dbContext.Users)
				{
					var invite = new Invite
					{
						Email = user.Email,
						InviteSent = user.Joined.AddDays(-new Random().Next(1, 10)),
						AccountCreated = user.Joined,
						AccountIsCreated = true,
						InvitedByUserId = dbContext.Users.Where(x => x.Id != user.Id).PickRandom().Id
					};
					dbContext.Invites.Add(invite);
				}
				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedPositionsAsync(DiffenDbContext dbContext)
		{
			if (!dbContext.Positions.Any())
			{
				var positions = PositionList.All().Select(position => new Position
				{
					Name = position.Name
				});
				dbContext.Positions.AddRange(positions);
				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedPostsAsync(DiffenDbContext dbContext)
		{
			if (!dbContext.Posts.Any())
			{
				var postMessages = await GetRandomTextAsync(NumberOfPosts);

				for (var i = 0; i < NumberOfPosts; i++)
				{
					var randomUser = dbContext.Users.PickRandom();
					var post = new Post
					{
						Message = postMessages[i],
						CreatedByUserId = randomUser.Id,
						Created = RandomDateTime.Get(randomUser.Joined, DateTime.Now)
					};
					dbContext.Posts.Add(post);
				}
				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedConversationsAsync(DiffenDbContext dbContext)
		{
			if (dbContext.Posts.All(x => x.ParentPost == null))
			{
				var posts = dbContext.Posts.PickRandom(20).ToList();
				var postMessages = await GetRandomTextAsync(20);
				for (var i = 0; i < posts.Count; i++)
				{
					var post = posts[i];
					var randomUser = dbContext.Users.Where(u => u.Id != post.CreatedByUserId).PickRandom();
					var answer = new Post
					{
						Message = postMessages[i],
						CreatedByUserId = randomUser.Id,
						ParentPostId = post.Id,
						Created = RandomDateTime.Get(posts[i].Created, post.Created.AddMinutes(30))
					};
					dbContext.Posts.Add(answer);
					await dbContext.SaveChangesAsync();
				}
				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedFormationsAsync(DiffenDbContext dbContext)
		{
			if (!dbContext.Formations.Any())
			{
				var formations = FormationList.All().Select(f => new Formation
				{
					Name = f.Name,
					ComponentName = f.ComponentName
				});
				dbContext.Formations.AddRange(formations);
				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedFavoritePlayerToUsers(DiffenDbContext dbContext)
		{
			if (!dbContext.FavoritePlayers.Any())
			{
				foreach (var userId in dbContext.Users.Select(user => user.Id))
				{
					var favoritePlayer = new FavoritePlayer
					{
						UserId = userId,
						PlayerId = dbContext.Players.PickRandom().Id
					};
					dbContext.FavoritePlayers.Add(favoritePlayer);
				}
				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedVotesOnPostsAsync(DiffenDbContext dbContext)
		{
			if (!dbContext.Votes.Any())
			{
				for (var i = 0; i < 2; i++)
				{
					var randomPosts = dbContext.Posts.Include(x => x.Votes).PickRandom(dbContext.Posts.Count() / 2);
					foreach (var post in randomPosts)
					{
						var users = dbContext.Users
							.Where(user => user.Id != post.CreatedByUserId && post.Votes.All(x => x.CreatedByUserId != user.Id))
							.ToList();
						var randomUserId = users.PickRandom().Id;
						var vote = new Vote
						{
							Type = (VoteType)new Random().Next(0, 2),
							PostId = post.Id,
							CreatedByUserId = randomUserId,
							Created = RandomDateTime.Get(post.Created, post.Created.AddMinutes(30))
						};
						dbContext.Votes.Add(vote);
						await dbContext.SaveChangesAsync();
					}
				}
			}
		}

		private static async Task SeedLineupsAsync(DiffenDbContext dbContext)
		{
			if (!dbContext.Lineups.Any())
			{
				for (var i = 0; i < 3; i++)
				{
					var randomUsers = dbContext.Users.PickRandom(dbContext.Users.Count() / 3);
					foreach (var user in randomUsers)
					{
						var randomFormation = dbContext.Formations.PickRandom();
						var availablePositions = FormationList.All().Find(f => f.Name == randomFormation.Name).Positions.ToList();

						var playersToLineup = new List<PlayerToLineup>();
						var playerIds = new List<int>();
						foreach (var position in availablePositions)
						{
							var playerId = dbContext.Players.Include(x => x.AvailablePositions)
								.Where(p => p.AvailablePositions.Select(ap => ap.Position.Name).Contains(position.Name) && !playerIds.Contains(p.Id)).PickRandom().Id;
							playerIds.Add(playerId);
							playersToLineup.Add(new PlayerToLineup
							{
								PositionId = dbContext.Positions.FirstOrDefault(x => x.Name == position.Name).Id,
								PlayerId = playerId
							});
						}
						var lineup = new Lineup
						{
							FormationId = randomFormation.Id,
							Players = playersToLineup,
							CreatedByUserId = user.Id,
							Created = RandomDateTime.Get(user.Joined, DateTime.Now)
						};
						dbContext.Lineups.Add(lineup);
					}
				}
				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedUrlTipsAsync(DiffenDbContext dbContext)
		{
			if (!dbContext.UrlTips.Any())
			{
				var hrefs = new[]
				{
					"http://fotboll.dif.se/",
					"https://twitter.com/DIF_Fotboll/status/980105262802636800",
					"https://www.youtube.com/watch?v=h6LTkdHVGvc",
					"https://www.youtube.com/watch?v=3RxSJ1VArqA",
					"https://www.youtube.com/watch?v=z6Ye_qgupAo",
					"https://twitter.com/DIF_Fotboll/status/979404315679907841",
					"https://twitter.com/DIF_Fotboll/status/979344438194573314",
					"https://twitter.com/08Fotboll_Fantv/status/978997711843323904",
					"https://twitter.com/JohnsonBjorn/status/977973553558638594",
					"https://twitter.com/JohnsonBjorn/status/977953842993168387",
					"https://twitter.com/DIF_Boa/status/978198625154105344",
					"https://twitter.com/DIF_Boa/status/977951773091684357",
					"https://twitter.com/DIF_Fotboll/status/977199583074844673",
					"https://twitter.com/DIF_Boa/status/959479734332772352",
					"https://www.youtube.com/watch?v=mrtQBXJ_Ce8"
				};
				foreach (var href in hrefs)
				{
					var post = dbContext.Posts.PickRandom();
					var urlTip = new UrlTip
					{
						Href = href,
						Clicks = new Random().Next(0, 100),
						PostId = post.Id
					};
					dbContext.UrlTips.Add(urlTip);
				}
				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedSavedPostsAsync(DiffenDbContext dbContext)
		{
			if (!dbContext.SavedPosts.Any())
			{
				var randomPosts = dbContext.Posts.PickRandom(dbContext.Posts.Count() / 4);
				foreach (var post in randomPosts)
				{
					var randomUserId = dbContext.Users.PickRandom().Id;
					var savedPost = new SavedPost
					{
						PostId = post.Id,
						SavedByUserId = randomUserId,
						Created = RandomDateTime.Get(post.Created, DateTime.Now)
					};
					dbContext.SavedPosts.Add(savedPost);
				}
				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedLineupsToPostsAsync(DiffenDbContext dbContext)
		{
			if (!dbContext.LineupsOnPosts.Any())
			{
				var postsCreatedByUsersWhoHasCreatedALineup = dbContext.Posts.Where(p => dbContext.Lineups.Select(l => l.CreatedByUserId).Contains(p.CreatedByUserId)).ToList();
				var postToLineups = postsCreatedByUsersWhoHasCreatedALineup.Select(post => new PostToLineup
				{
					PostId = post.Id,
					LineupId = dbContext.Lineups.FirstOrDefault(l => l.CreatedByUserId == post.CreatedByUserId).Id
				});
				dbContext.LineupsOnPosts.AddRange(postToLineups);
				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedPersonalMessagesAsync(DiffenDbContext dbContext)
		{
			if (!dbContext.PersonalMessages.Any())
			{
				for (var i = 0; i < 10; i++)
				{
					var users = dbContext.Users.PickRandom(dbContext.Users.Count() / 3);
					var pmMessages = await GetRandomTextAsync(10);
					foreach (var user in users)
					{
						var randomUser = dbContext.Users.Where(toUser => toUser.Id != user.Id).PickRandom();
						var personalMessage = new PersonalMessage
						{
							FromUserId = user.Id,
							ToUserId = randomUser.Id,
							Message = pmMessages[i],
							Created = RandomDateTime.Get(user.Joined, DateTime.Now)
						};
						dbContext.PersonalMessages.Add(personalMessage);
					}
					await dbContext.SaveChangesAsync();
				}
			}
		}

		private static async Task SeedUserFiltersAsync(DiffenDbContext dbContext)
		{
			if (!dbContext.UserFilters.Any())
			{
				var users = dbContext.Users.PickRandom(dbContext.Users.Count() / 3);
				foreach (var user in users)
				{
					var excludedUsers = dbContext.Users.Where(u => u.Id != user.Id).ToList().PickRandom(3);
					var filter = new Filter
					{
						UserId = user.Id,
						PostsPerPage = new Random().Next(1, 50),
						ExcludedUserIds = string.Join(",", excludedUsers.Select(x => x.Id))
					};
					dbContext.UserFilters.Add(filter);
				}
				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedScissoredPostsAsync(DiffenDbContext dbContext)
		{
			if (!dbContext.ScissoredPosts.Any())
			{
				var posts = dbContext.Posts.ToList().PickRandom(7);
				var scissoredPosts = posts.Select(p => new Scissored
				{
					PostId = p.Id,
					Created = RandomDateTime.Get(p.Created, p.Created.AddMinutes(30))
				});
				dbContext.ScissoredPosts.AddRange(scissoredPosts);
				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedPollsAsync(DiffenDbContext dbContext)
		{
			if (!dbContext.Polls.Any())
			{
				foreach (var pollItem in PollList.All())
				{
					var user = dbContext.Users.PickRandom();
					var created = RandomDateTime.Get(user.Joined, DateTime.Now);
					var poll = new Poll
					{
						Name = pollItem.Name,
						Slug = new SlugHelper().GenerateSlug(pollItem.Name),
						CreatedByUserId = user.Id,
						Created = created
					};
					dbContext.Polls.Add(poll);
					await dbContext.SaveChangesAsync();
					foreach (var selectionItem in pollItem.Selections)
					{
						var selection = new PollSelection
						{
							Name = selectionItem.Name,
							PollId = poll.Id
						};
						dbContext.PollSelections.Add(selection);
						await dbContext.SaveChangesAsync();
					}
				}

				for (var i = 0; i < 3; i++)
				{
					foreach (var poll in dbContext.Polls.Include(x => x.Selections).ThenInclude(x => x.Votes))
					{
						for (var q = 0; q < 5; q++)
						{
							var selection = poll.Selections.PickRandom();
							var user = dbContext.Users.Where(x => x.Id != poll.CreatedByUserId && !selection.Votes.Select(y => y.VotedByUserId).Contains(x.Id)).PickRandom();
							var vote = new PollVote
							{
								PollSelectionId = selection.Id,
								VotedByUserId = user.Id,
								Created = poll.Created.AddDays(new Random().Next(1, 7))
							};
							dbContext.PollVotes.Add(vote);
						}
					}
				}
				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedRegionsAsync(DiffenDbContext dbContext)
		{
			if (!dbContext.Regions.Any())
			{
				foreach (var regionItem in RegionList.All())
				{
					var region = new Region
					{
						Name = regionItem.Name,
						Latitud = regionItem.Latitud,
						Longitud = regionItem.Longitud,
						Created = DateTime.Now
					};
					dbContext.Regions.Add(region);
				}
				await dbContext.SaveChangesAsync();
				foreach (var user in dbContext.Users)
				{
					var userToRegion = new RegionToUser
					{
						UserId = user.Id,
						RegionId = dbContext.Regions.PickRandom().Id
					};
					dbContext.UsersToRegions.Add(userToRegion);
				}
				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedChroniclesAsync(DiffenDbContext dbContext, UserManager<AppUser> userManager)
		{
			if (!dbContext.Chronicles.Any())
			{
				var chronicleTexts = new List<KeyValuePair<string, string>>
				{
					new KeyValuePair<string, string>("Tre nycklar i derbyt",
						"<p><br></p><p><strong style=\"color: rgb(0, 0, 0);\">Hur ska Djurgården lyckas att vinna derbyt? Vad kommer bli avgörande? Här är tre nycklar inför derbyt mot Hammarby.</strong></p><p><br></p><p><strong>Presspelet</strong></p><p>Här handlar det om båda lagens presspel. Djurgårdens presspel med Kerim Mrabti var väldigt lyckat, mindre lyckat med Badji på banan. Men man har ändå lyckats slipa presspelet i senaste matcherna. Hammarbys presspel har också fungerat väl ut men också straffat dem. Bolltap eller genombrottspassningar igenom deras presspel kan såra Hammarby enormt. Titta bara på senaste matchen mot Häcken.</p><p><br></p><p><strong>Kanterna</strong></p><p>Djurgården är skickliga via kanterna, inte minst med Jonathan Ring på planen. Men det är kanske inte nödvändigtvis inlägg utan inspel från kanterna och att spelare som Ring kliver in från kanten och in centralt i banan. Defensivt kommer Djurgården försöka trycka ut Hammarby på kanterna för att tvingas slå inlägg, något Blårändernas nickstarka mittbackar bör klara av. Det kräver att Radetinac och Ring kliver in centralt när Djurgården tappar boll och måste försvara. Hos Bajen finns en tydlig svag punkt och det är på deras vänsterkant. Neto Borges har varit lyckosam offensivt, men han visar också brister i det defensiva. Där kommer det finnas luckor.</p><p><br></p><p><strong>Mod</strong></p><p>Derbyn handlar mycket om mod. Att våga leva med tempot, att ha modet att göra sitt jobb och det lilla extra. Att tro på sin gameplan och njuta. Inte gömma sig i skuggor och tro att någon annan ska lösa problemen.</p><p><br></p><p>***</p><p><br></p><p><strong>Bonus-nyckel: stödet från läktarna</strong></p><p>Mod måste också komma från läktarhåll. Att från sittplats till klacken, från junis till klackrävar visa alla andra vilka som ska vinna det här derbyt.</p><p><br></p><p>Avspark 15:00.</p>"),
					new KeyValuePair<string, string>("Krönika: Allt vi drömde om på Östra 2010", "<p><br></p><blockquote><strong style=\"color: rgb(0, 0, 0);\">\"Två år senare är vi snart framme vid toppmatcher som tidigare endast fanns på ett teoretiskt plan, över en öl på Östra efter en match\", skriver Gustaf Nilsson i en krönika.</strong></blockquote><p><br></p><p><strong>Det är bara några dagar</strong>&nbsp;kvar av april. En månad som för DIF fotboll får ett medelbetyg med plus i kanten.</p><p><br></p><p>Och ena sidan spelade vi skjortan av de regerande mästarna och tagit ner Östersund på jorden. Och andra sidan gjorde vi ett uselt derby sång- och spelmässigt mot aik. Med de senaste tio åren i nära minne, där man behövt käka magsårstabletter inför varenda kortpassning inom laget, är det lättare att fastna med det som är negativt just nu.</p><p><br></p><p>Att vi saknar ett given anfallsuppsättning, att vi tappar onödiga poäng mot Trelleborg och Elfsborg. Att läktarinsatserna varit ojämna på både Sofia och Slaktis i år. Men vi får inte glömma bort var vi befinner oss idag.</p><p><br></p><p><strong>Om exakt två veckor</strong>&nbsp;väntar en cupfinal mot Malmö och en chans att ta vår första stora titel sedan 2005 – dessutom på hemmaplan. Vi ska ut i Europa mitt i sommarvärmen – något jag och polarna bokstavligt bara drömde om åren 2008-2013. Att det SKULLE inträffa en dag enades vi om – men då skulle vi troligen vara väldigt, väldigt gamla.</p><p><br></p><p>För ibland glömmer vi att allt har gått väldigt fort. För mindre än två år sen dansade vi i botten av tabellen och tvingades sparka en tränare (Pelle Olsson) som vi nyligen hade förlängt med. Två år senare är vi snart framme vid toppmatcher som tidigare endast fanns på ett teoretiskt plan, över en öl på Östra Station efter en match. På söndag väntar Bajen på hemmaplan. Allt talar för utsålda läktare, 15 grader varmt och strålande sol. När du tar dig till arenan och kanske tar dig en öl för att lugna derbynerverna: Passa på att stanna upp och njut av den tid vi har framför oss.</p><p><br></p><p>Sen går du ut på läktaren och sjunger utav bara satan.&nbsp;<strong>Ur spår bönder, vi är DIF.</strong></p>"),
					new KeyValuePair<string, string>("Elfsborg - Djurgården 2-2: Passivt Djurgården tappade segern", "<p><br></p><p><strong style=\"color: rgb(0, 0, 0);\">Blåränderna tappade segern i slutminuterna efter en passiv andra halvlek där bytena lämnade frågetecken.</strong></p><p><br></p><p>Som Forum 1891 rapporterade inför matchen valde Özcan Melkemichel att rotera i laget. Marcus Danielsson och Jacob Une Larsson in som mittbackspar och Jesper Karlström och Kevin Walker centralt på mittfältet.</p><p><br></p><p>Matchen inleddes med ganska högt tempo där båda lagen ville styra matchen, men det var Elfsborg som skulle ta ledningen tidigt efter ett snyggt distansskott av Simon Lundevall. Efter målet tog Djurgården över matchbilden och efter drygt 20 minuter snappade Jesper Karlström upp en boll efter misstag av Jon Jönsson och spelade fram Jonathan Ring som drog en tåpaj i mål.</p><p><br></p><p>Djurgården fortsatte att trycka framåt och strax innan halvtidspaus spelade Jonathan Ring fram Aliou Badji som sköt ett stenhårt skott i mål.</p><p><br></p><p>I andra halvlek fortsatte Djurgården att dominera och trycka ner Elfsborg. Men efter halva andra halvlek blev Djurgården väldigt passiva och gav initiativet till hemmalaget som blev allt mer desperata och skapade fler lägen. Tränare Özcan Melkemichel gjorde defensiva byten för att krympa ytorna, men det gjorde laget än mer passivt.&nbsp;</p><blockquote>– Det var så sent in och det var så trötta ben. Vi var tvungna att göra något, sade Özcan Melkemichel till C More efter matchen som menade att det inte var meningen att laget skulle bli så passiva.</blockquote><p><br></p><p>Kvitteringsmålet låg i luften och i 90:e minuten sköt Jon Jönsson in bollen ottagbart för målvakt Andreas Isaksson</p><blockquote>– Det känns som en förlust, summerade Özcan Melkemichel efter matchen.</blockquote><p><br></p><p>Djurgården gör en bra match i ungefär 60 minuter, men blir sedan alldeles för passiva och ger bort den här matchen. Båda anfallarna byttes ut för defensiva spelare och så här i efterhand var det inget lyckat drag. Till Özcan Melkemichels försvar fanns det ingen (!) forward på bänken.</p><p><br></p><p>Härnäst väntar Hammarby på Tele2 Arena på söndag klockan 15:00.</p><p><br></p><p><strong>Elfsborg - Djurgården 2-2</strong></p><p>1-0 Simon Lundevall (9 min)</p><p>1-1 Jonathan Ring (21 min)</p><p>1-2 Aliou Badji (45 min)</p><p>2-2 Jon Jönsson (90 min)</p><p><br></p>"),
					new KeyValuePair<string, string>("Kerim Mrabti närmar sig comeback", "<p><br></p><p><strong style=\"color: rgb(0, 0, 0);\">En av Djurgårdens viktigaste spelare, Kerim Mrabti, närmar sig comeback.</strong></p><p><br></p><p>Det var i derbyt mot AIK som Kerim Mrabti skadade sig när han drog baksida lår. Prognosen sade att den offensive mittfältaren skulle bli borta några veckor. Nu har det snart gått två veckor sedan skadan och 23-åringen upplever en bra rehabperiod.</p><blockquote>– Det känns väldigt bra, säger Kerim Mrabti till DIFTV.</blockquote><p><br></p><p>Mrabti kommer missa derbyt mot Hammarby och förmodligen stå över bortamatchen mot Malmö FF på torsdag, men han har siktet inställt på att spela Svenska cupen-finalen mot Malmö FF 10 maj.</p><blockquote>– Det är klart man önskar att man hade kunnat spela nu på söndag, men jag tror förmodligen att vi kommer få stå över den så det inte blir en risk att jag inte kan spela de andra matcherna här under våren. Vi har en tuff match veckan efter mot Malmö som är minst lika viktig och även en Svenska cupen-final som jag definitivt siktar in på, berättar Kerim Mrabti.</blockquote><p><br></p><p>Över 24 000 biljetter är sålda till söndagens derby mot Hammarby. Avspark 15:00 på Tele2 Arena.</p>"),
					new KeyValuePair<string, string>("Yura Movsisyan stängs av mot Elfsborg", "<p><br></p><p><strong style=\"color: rgb(0, 0, 0);\">Djurgårdens anfallare Yura Movsisyan tilldelades rött kort i derbyt mot AIK och blev automatiskt avstängd mot Malmö FF. Nu har disciplinnämnden fastställt att anfallaren även stängs av i nästa match, mot Elfsborg.</strong></p><p><br></p><p>Det var i slutet av matchen mot AIK som&nbsp;Yura Movsisyan satte sin panna mot Daniel Sundgren och visades ut. Rött kort och avstängning i nästkommande match som naturlig påföljd. Nu har disciplinnämnden beslutat att anfallaren blir avstängd ytterligare en match, alltså mot Elfsborg på måndag.</p><blockquote>– Därför att vi menar att han uppträtt olämpligt. Det betyder en match till. Sedan var det en ledamot som ville bestraffa honom med tre matcher, men det var skiljeaktigt, då majoriteten tyckte att det räckte med en match,&nbsp;säger Kerstin Elserth, ordförande i disciplinnämnden till SportExpressen.</blockquote><p><br></p><p>Djurgården kommer inte överklaga beslutet.&nbsp;</p><blockquote>– Vi har tagit del av beslutet som vi respekterar och kommer inte att överklaga det, säger sportchef Bosse Andersson till Djurgårdens hemsida.</blockquote><p><br></p>")
				};
				foreach (var chronicleText in chronicleTexts)
				{
					var randomUser = userManager.GetUsersInRoleAsync("Author").Result.PickRandom();
					var created = RandomDateTime.Get(randomUser.Joined, DateTime.Now);
					var chronicle = new Chronicle
					{
						Title = chronicleText.Key,
						Text = chronicleText.Value,
						Slug = new SlugHelper().GenerateSlug(chronicleText.Key),
						WrittenByUserId = randomUser.Id,
						Created = created,
						Published = RandomDateTime.Get(created, created.AddDays(5))
					};
					dbContext.Chronicles.Add(chronicle);
				}
				await dbContext.SaveChangesAsync();
			}
		}
	}
}
