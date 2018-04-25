using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

	public class Initializer
	{
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
		}

		private static async Task SeedUsersAndNickNamesAsync(DiffenDbContext dbContext, UserManager<AppUser> userManager)
		{
			if (userManager.Users.Any())
				return;

			for (var i = 1; i <= 20; i++)
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
					Nick = $"seeded_user_{i}",
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
				for (var i = 0; i <= 50; i++)
				{
					var randomUser = dbContext.Users.PickRandom();
					var randomUserNick = randomUser.NickNames.OrderByDescending(x => x.Created).FirstOrDefault()?.Nick;
					var post = new Post
					{
						Message = $"Autogenererat inlägg för {randomUserNick}. Scrolla vidare! \n\nMvh Admin",
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
				var posts = dbContext.Posts.ToList().PickRandom(20);
				foreach (var post in posts)
				{
					var randomUser = dbContext.Users.Where(u => u.Id != post.CreatedByUserId).PickRandom();
					var answer = new Post
					{
						Message = $"Autogenererat svar till inlägg {post.Id}. Scrolla vidare! \n\nMvh Admin",
						CreatedByUserId = randomUser.Id,
						ParentPostId = post.Id,
						Created = RandomDateTime.Get(post.Created, post.Created.AddMinutes(30))
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
					foreach (var user in users)
					{
						var randomUser = dbContext.Users.Where(toUser => toUser.Id != user.Id).PickRandom();
						var personalMessage = new PersonalMessage
						{
							FromUserId = user.Id,
							ToUserId = randomUser.Id,
							Message = $"Ville bara säga hej {randomUser.NickNames.OrderByDescending(x => x.Created).FirstOrDefault()?.Nick}!\n\nLorem ipsum dolor amet prism intelligentsia fashion axe skateboard, tilde etsy small batch distillery. Yuccie cornhole artisan taxidermy iceland raclette prism drinking vinegar truffaut health goth ennui.\n\nMvh {user.NickNames.OrderByDescending(x => x.Created).FirstOrDefault()?.Nick}",
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
	}
}
