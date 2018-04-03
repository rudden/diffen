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

	using DateTime = DateTime;

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
			await SeedPlayersAsync(dbContext);
			await SeedInvitesAsync(dbContext);
			await SeedPostsAsync(dbContext);
			await SeedConversationsAsync(dbContext);
			await SeedPositionsAsync(dbContext);
			await SeedFormationsAsync(dbContext);
			await SeedLineupsAsync(dbContext);
			await SeedUrlTipsAsync(dbContext);
			await SeedLineupsToPostsAsync(dbContext);
			await SeedPersonalMessagesAsync(dbContext);
			await SeedFavoritePlayerToUsers(dbContext);
			await SeedSavedPostsAsync(dbContext);
			await SeedVotesOnPostsAsync(dbContext);
			await SeedUserFiltersAsync(dbContext);
			await SeedScissoredPostsAsync(dbContext);
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
					Joined = DateTime.Now
				};
				await userManager.CreateAsync(user, "P@ssw0rd!");
				await dbContext.SaveChangesAsync();

				var nickName = new NickName
				{
					UserId = user.Id,
					Nick = $"seeded_user_{i}",
					Created = DateTime.Now
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
			if (!await roleManager.RoleExistsAsync("Sax"))
			{
				var role = new IdentityRole
				{
					Name = "Sax"
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

			var scissors = await userManager.GetUsersInRoleAsync("Sax");
			if (!scissors.Any())
			{
				var newScissors = userManager.Users.ToList().PickRandom(10);
				foreach (var scissor in newScissors)
				{
					await userManager.AddToRoleAsync(scissor, "Sax");
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
					new Player
					{
						FirstName = "Johan",
						LastName = "Andersson",
						KitNumber = 2
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
						KitNumber = 15,
						IsOutOnLoan = true
					},
					new Player
					{
						FirstName = "Marcus",
						LastName = "Hansson",
						KitNumber = 19,
						IsOutOnLoan = true
					},
					new Player
					{
						FirstName = "Amadou",
						LastName = "Jawo",
						KitNumber = 11,
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
						KitNumber = 29,
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
				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedPositionsAsync(DiffenDbContext dbContext)
		{
			if (!dbContext.Positions.Any())
			{
				var positions = PositionList.All().Select(position => new Position
				{
					Name = position
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
						Created = RandomDateTime.Get(randomUser.Joined, new DateTime(2018, 06, 1))
					};
					dbContext.Posts.Add(post);
				}
				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedConversationsAsync(DiffenDbContext dbContext)
		{
			if (!dbContext.Conversations.Any())
			{
				var posts = dbContext.Posts.ToList().PickRandom(20);
				foreach (var post in posts)
				{
					var randomUser = dbContext.Users.Where(u => u.Id != post.CreatedByUserId).PickRandom();
					var answer = new Post
					{
						Message = $"Autogenererat svar till inlägg {post.Id}. Scrolla vidare! \n\nMvh Admin",
						CreatedByUserId = randomUser.Id,
						Created = RandomDateTime.Get(post.Created, new DateTime(2018, 06, 1))
					};
					dbContext.Posts.Add(answer);
					await dbContext.SaveChangesAsync();

					var conversationEntry = new PostToPost
					{
						PostId = answer.Id,
						ParentPostId = post.Id
					};
					dbContext.Conversations.Add(conversationEntry);
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
					Name = f.Name
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
							Created = RandomDateTime.Get(post.Created, new DateTime(2018, 6, 1))
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
				var randomUsers = dbContext.Users.PickRandom(dbContext.Users.Count() / 3);
				foreach (var user in randomUsers)
				{
					var randomPlayers = dbContext.Players.PickRandom(12).Select(p => new PlayerToLineup
					{
						PlayerId = p.Id,
						PositionId = dbContext.Positions.PickRandom().Id
					}).ToList();
					var randomFormationId = dbContext.Formations.PickRandom().Id;
					var lineup = new Lineup
					{
						FormationId = randomFormationId,
						Players = randomPlayers,
						CreatedByUserId = user.Id,
						Created = RandomDateTime.Get(user.Joined, new DateTime(2018, 6, 1))
					};
					dbContext.Lineups.Add(lineup);
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
						PostId = post.Id,
						Created = post.Created
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
						Created = RandomDateTime.Get(post.Created, new DateTime(2018, 6, 1))
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
				for (var i = 0; i < 3; i++)
				{
					var users = dbContext.Users.PickRandom(dbContext.Users.Count() / 3);
					foreach (var user in users)
					{
						var randomUser = dbContext.Users.Where(toUser => toUser.Id != user.Id).PickRandom();
						var personalMessage = new PersonalMessage
						{
							FromUserId = user.Id,
							ToUserId = randomUser.Id,
							Message = $"Ville bara säga hej {randomUser.NickNames.OrderByDescending(x => x.Created).FirstOrDefault()?.Nick}!\n\nMvh {user.NickNames.OrderByDescending(x => x.Created).FirstOrDefault()?.Nick}",
							Created = RandomDateTime.Get(user.Joined, new DateTime(2018, 6, 1))
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
						ExcludedUserIds = string.Join(",", excludedUsers)
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
					Created = RandomDateTime.Get(p.Created, new DateTime(2018, 6, 1))
				});
				dbContext.ScissoredPosts.AddRange(scissoredPosts);
				await dbContext.SaveChangesAsync();
			}
		}
	}
}
