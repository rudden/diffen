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
					var created = RandomDateTime.Get(DateTime.Now.AddDays(-10), DateTime.Now);
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
					new KeyValuePair<string, string>("Brooklyn pork belly tote bag",
						"<p><br></p><p><strong style=\"color: rgb(0, 0, 0);\">Hur ska Djurgården lyckas att vinna derbyt? Vad kommer bli avgörande? Här är tre nycklar inför derbyt mot Hammarby.</strong></p><p><strong style=\"color: rgb(0, 0, 0);\">Street art actually microdosing, 3 wolf moon woke VHS vinyl kogi pour-over pitchfork? Taiyaki vice portland keffiyeh enamel pin tattooed chia selfies selvage echo park ennui distillery tofu.</strong></p><p><br></p><p><strong>Waistcoat butcher</strong></p><p>Här handlar det om båda lagens presspel. Djurgårdens presspel med Kerim Mrabti var väldigt lyckat, mindre lyckat med Badji på banan. Men man har ändå lyckats slipa presspelet i senaste matcherna. Hammarbys presspel har också fungerat väl ut men också straffat dem. Bolltap eller genombrottspassningar igenom deras presspel kan såra Hammarby enormt. Titta bara på senaste matchen mot Häcken.</p><p>Iceland echo park fashion axe, gochujang you probably haven't heard of them adaptogen organic retro meggings. Waistcoat butcher hell of vice next level banjo jianbing. Freegan hell of letterpress, irony hammock hashtag intelligentsia food truck fixie echo park church-key photo booth. Pickled cray beard cliche sriracha, tbh yuccie coloring book. Shaman meh pinterest jean shorts. Mumblecore gastropub crucifix tbh pok pok bespoke.</p><p><br></p><p><strong>Farm-to-table</strong></p><p>Copper mug cold-pressed aesthetic lyft banjo, kickstarter gochujang offal echo park portland. PBR&B glossier photo booth ennui chartreuse kinfolk live-edge tousled tacos post-ironic marfa kickstarter flannel actually. Schlitz iPhone prism small batch, portland gochujang irony occupy poutine fingerstache hoodie PBR&B leggings. Pour-over jean shorts keffiyeh edison bulb. Waistcoat you probably haven't heard of them flannel helvetica tacos organic ugh cardigan deep v. Banh mi seitan tumblr four loko irony.</p><p><br></p><p><strong>Mod</strong></p><p>Derbyn handlar mycket om mod. Att våga leva med tempot, att ha modet att göra sitt jobb och det lilla extra. Att tro på sin gameplan och njuta. Inte gömma sig i skuggor och tro att någon annan ska lösa problemen.</p><p>Lorem ipsum dolor amet meditation stumptown bitters woke jean shorts. Brooklyn pork belly tote bag, kombucha cardigan enamel pin fixie poke stumptown echo park tofu raw denim.</p><p><br></p><p>***</p><p><br></p><p><strong>Enamel pin paleo vinyl: fixie prism</strong></p><p>Unicorn disrupt enamel pin VHS selfies. Coloring book stumptown pug twee whatever literally. Ethical next level you probably haven't heard of them art party etsy aesthetic crucifix. Squid fanny pack blog kale chips.</p><p><br></p><p>Asymmetrical actually iceland.</p>"),
					new KeyValuePair<string, string>("Flexitarian adipisicing mustache", "<p><br></p><blockquote><strong style=\"color: rgb(0, 0, 0);\">\"Laboris est aliqua voluptate. Sed fingerstache before they sold out master cleanse, tumblr chambray deep v pop-up duis. Man braid chartreuse selfies.\", Cray non est umami readymade.</strong></blockquote><p><br></p><p><strong>Laboris est aliqua voluptate</strong>&nbsp;tumblr chambray deep v pop-up duis. Raw denim green juice magna, affogato anim nostrud in et tilde hot chicken shabby.</p><p><br></p><p>+1 culpa fugiat, photo booth in ut nulla. Fugiat vegan eu laboris intelligentsia mollit crucifix microdosing. Keffiyeh sunt veniam chillwave salvia chicharrones hammock pitchfork paleo succulents aute meditation mumblecore. Ea kitsch salvia, selvage nostrud small batch gentrify cold-pressed ut pug food truck vice artisan. Direct trade cornhole literally mixtape. Cronut freegan in officia ipsum PBR&B enamel pin consectetur poutine narwhal kogi. Fingerstache activated charcoal ut proident chartreuse.</p><p><br></p><p>Twee voluptate synth dreamcatcher reprehenderit. Eiusmod live-edge cardigan raw denim, retro aute offal poutine skateboard. Try-hard godard bitters bushwick stumptown. Seitan pinterest ut quis.</p><p><br></p><p><strong>Officia normcore ut skateboard</strong>&nbsp;direct trade flannel venmo velit authentic next level letterpress lumbersexual duis poutine. Sint qui intelligentsia consequat beard. Cray humblebrag magna iceland selvage authentic sint in man bun PBR&B. Et banjo keytar magna literally, kickstarter stumptown labore you probably haven't heard of them vexillologist wolf roof party eiusmod. Id irure austin VHS +1 tumblr. Cronut lomo af, banh mi leggings neutra gochujang.</p><p><br></p><p>Brooklyn succulents id nostrud, four dollar toast crucifix cillum ramps scenester marfa vexillologist kombucha next level jean shorts microdosing. Schlitz biodiesel fingerstache beard. YOLO intelligentsia bespoke, aesthetic woke kinfolk whatever. Irony wolf enim eiusmod taxidermy knausgaard artisan kale chips commodo. Messenger bag kale chips hexagon wayfarers, waistcoat lumbersexual four loko ullamco taiyaki tumblr etsy artisan ex jianbing ramps. Shabby chic in thundercats sed culpa intelligentsia adipisicing unicorn edison bulb everyday carry +1 deserunt. Bicycle rights swag mollit keffiyeh succulents dreamcatcher kinfolk quis cornhole disrupt.</p><p><br></p><p>Irure adaptogen shabby chic blog, pour-over kinfolk echo park stumptown narwhal labore cray.</strong></p>"),
					new KeyValuePair<string, string>("Banh mi try-hard blog, chartreuse skateboard", "<p><br></p><p><strong style=\"color: rgb(0, 0, 0);\">Marfa raw denim portland salvia craft beer cronut jean shorts mustache unicorn. Kickstarter craft beer cred pok pok. Ethical vinyl microdosing.</strong></p><p><br></p><p>Literally ethical biodiesel quinoa cronut, flexitarian lo-fi offal next level pok pok blog green juice cornhole wayfarers. Irony direct trade humblebrag, try-hard narwhal cardigan tousled stumptown beard adaptogen. Intelligentsia iPhone migas farm-to-table.</p><p><br></p><p>Banh mi try-hard blog, chartreuse skateboard ramps venmo pinterest fanny pack microdosing humblebrag polaroid pork belly tofu. Messenger bag slow-carb roof party, pabst tumblr leggings vexillologist. You probably haven't heard of them iceland shoreditch fanny pack, mustache skateboard lomo normcore celiac. Heirloom quinoa forage disrupt franzen.</p><p><br></p><p>Shoreditch art party mlkshk, butcher meh selfies roof party stumptown ugh bitters chillwave. Seitan church-key yuccie, master cleanse viral synth unicorn flannel butcher trust fund brunch lumbersexual retro echo park.</p><p><br></p><p>Whatever flexitarian cred keffiyeh tilde banh mi. Mustache meditation sartorial cronut, mlkshk XOXO poutine. Next level poutine cloud bread try-hard truffaut tilde letterpress beard gentrify distillery artisan. Man braid retro green juice, paleo ugh beard crucifix aesthetic. Everyday carry kinfolk umami live-edge paleo polaroid migas +1. Vice blue bottle sriracha pug green juice VHS cardigan beard etsy fanny pack microdosing cred chillwave.&nbsp;</p><blockquote>– Truffaut YOLO pour-over, +1 church-key gluten-free four dollar toast bespoke readymade wayfarers normcore tilde shoreditch single-origin coffee.</blockquote><p><br></p><p>Taxidermy kitsch jean shorts, crucifix butcher typewriter austin hashtag</p><blockquote>– Farm-to-table you probably haven't heard of them palo santo shabby chic fanny pack, mixtape glossier tacos poke.</blockquote><p><br></p><p>Copper mug keytar iPhone, hashtag fashion axe biodiesel whatever. Church-key you probably haven't heard of them viral selfies. Roof party street art raclette marfa, shabby chic slow-carb ethical messenger bag etsy banjo flexitarian hammock. Narwhal biodiesel church-key, tbh bushwick vice stumptown vegan etsy kale chips ethical pabst farm-to-table lomo flexitarian. Godard succulents helvetica pop-up gochujang selvage pug kinfolk seitan jean shorts keffiyeh church-key.</p><p><br></p><p>Coloring book chicharrones vexillologist fanny pack shaman celiac beard swag.</p>"),
					new KeyValuePair<string, string>("Umami chicharrones meh blog helvetica", "<p><br></p><p><strong style=\"color: rgb(0, 0, 0);\">Gastropub 90's banjo freegan sustainable literally vape blog. Cardigan gentrify messenger bag, craft beer small batch.</strong></p><p><br></p><p>Iceland migas tbh meggings succulents jean shorts williamsburg ramps intelligentsia keytar tattooed pickled flannel shoreditch. Crucifix vape asymmetrical, authentic try-hard chia pabst biodiesel cloud bread woke taiyaki mixtape fanny pack. Cornhole ennui freegan adaptogen scenester keffiyeh, butcher tofu af pitchfork.</p><blockquote>– Letterpress hexagon keytar, hashtag cred tacos helvetica.</blockquote><p><br></p><p>Narwhal biodiesel church-key, tbh bushwick vice stumptown vegan etsy kale chips ethical pabst farm-to-table lomo flexitarian. Godard succulents helvetica pop-up gochujang selvage pug kinfolk seitan jean shorts keffiyeh church-key.</p><blockquote>– Bicycle rights yr sriracha dreamcatcher af. Meggings godard cred 3 wolf moon gastropub, pug artisan chicharrones yuccie gentrify migas selvage chia mumblecore. Blog ugh marfa, flexitarian humblebrag beard butcher pabst messenger bag flannel pour-over kinfolk. Readymade marfa chicharrones austin, selvage hoodie forage.</blockquote><p><br></p><p>Farm-to-table you probably haven't heard of them palo santo shabby chic fanny pack, mixtape glossier tacos poke. Vexillologist kinfolk chambray master cleanse.</p>"),
					new KeyValuePair<string, string>("Helvetica schlitz authentic chicharrones", "<p><br></p><p><strong style=\"color: rgb(0, 0, 0);\">Pabst synth XOXO pinterest skateboard disrupt flannel palo santo hoodie venmo la croix mustache. Tote bag butcher beard, raclette vegan ugh meggings church-key shabby chic. Listicle migas farm-to-table chambray whatever food truck VHS yr af woke mlkshk chicharrones.</strong></p><p><br></p><p>Lorem ipsum dolor amet roof party pok pok gentrify photo booth shoreditch selvage PBR&B mlkshk. Jean shorts tattooed la croix knausgaard, sriracha scenester echo park. Franzen seitan leggings organic tofu shabby chic readymade before they sold out hell of. Microdosing fingerstache vegan single-origin coffee aesthetic direct trade flexitarian knausgaard lumbersexual raw denim bespoke meditation.</p><blockquote>– Occupy kogi everyday carry bushwick cliche williamsburg. Mustache freegan tumblr four dollar toast blog. Raw denim snackwave hot chicken distillery,&nbsp;Enamel pin hella bespoke offal tattooed banjo brunch coloring book flexitarian PBR&B cray yr.</blockquote><p><br></p><p>IPhone put a bird on it leggings fixie tumeric.&nbsp;</p><blockquote>– Gochujang locavore godard la croix listicle whatever lyft copper mug post-ironic iceland taxidermy microdosing.</blockquote><p><br></p>")
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
