using System;
using System.Linq;
using System.Collections.Generic;

using AutoMapper;

namespace Diffen.Helpers.Mapper.Resolvers
{
	using Enum;
	using Models;
	using Business;
	using Extensions;
	using Database.Clients.Contracts;

	public class SquadResolver : 
		ITypeConverter<Database.Entities.Squad.Player, Models.Squad.Player>, 
		ITypeConverter<Database.Entities.Squad.Lineup, Models.Squad.Lineup>,
		ITypeConverter<Database.Entities.Squad.PlayerToLineup, Models.Squad.PlayerToLineup>,
		ITypeConverter<Models.Squad.CRUD.Lineup, Database.Entities.Squad.Lineup>, 
		ITypeConverter<Database.Entities.User.FavoritePlayer, Models.Squad.Player>, 
		ITypeConverter<Models.Squad.CRUD.Player, Database.Entities.Squad.Player>,
		ITypeConverter<Database.Entities.Squad.Game, Models.Squad.Game>,
		ITypeConverter<Database.Entities.Squad.PlayerEvent, Models.Squad.PlayerEvent>,
		ITypeConverter<Database.Entities.Squad.PlayerEvent, Models.Squad.PlayerEventOnPlayer>,
		ITypeConverter<Models.Squad.CRUD.Game, Database.Entities.Squad.Game>,
		ITypeConverter<Database.Entities.Squad.Title, Models.Squad.Title>,
		ITypeConverter<Models.Squad.CRUD.GameResultGuess, Database.Entities.Squad.GameResultGuess>,
		ITypeConverter<Database.Entities.Squad.GameResultGuess, Models.Squad.GameResultGuess>,
		ITypeConverter<Database.Entities.Squad.Player, Models.Squad.PlayerAttributes>
	{
		private readonly IDiffenDbClient _dbClient;

		public SquadResolver(IDiffenDbClient dbClient)
		{
			_dbClient = dbClient;
		}

		public Models.Squad.Player Convert(Database.Entities.Squad.Player source, Models.Squad.Player destination, ResolutionContext context)
		{
			return new Models.Squad.Player
			{
				Id = source.Id,
				FirstName = source.FirstName,
				LastName = source.LastName,
				KitNumber = source.KitNumber,
				Attributes = context.Mapper.Map<Models.Squad.PlayerAttributes>(source),
				BirthDay = source.BirthDay.Year > 0001 ? source.BirthDay.ToString("yyyy-MM-dd") : null,
				HeightInCentimeters = source.HeightInCentimeters,
				Weight = source.Weight,
				PreferredFoot = source.PreferredFoot,
				About = source.About,
				ContractUntil = source.ContractUntil.Year > 0001 ? source.ContractUntil.ToString("yyyy-MM-dd") : null,
				ImageUrl = source.ImageUrl,
				AvailablePositions = source.AvailablePositions.Select(x => new Models.Squad.Position
				{
					Id = x.Position.Id,
					Name = x.Position.Name
				}),
				InNumberOfStartingElevens = source.InLineups.Count,
				Events = context.Mapper.Map<IEnumerable<Models.Squad.PlayerEventOnPlayer>>(source.PlayerEvents),
				Data = GetPlayerTableData(source)
			};
		}

		public Models.Squad.PlayerAttributes Convert(Database.Entities.Squad.Player source,
			Models.Squad.PlayerAttributes destination, ResolutionContext context)
		{
			return new Models.Squad.PlayerAttributes
			{
				IsCaptain = source.IsCaptain,
				IsViceCaptain = source.IsViceCaptain,
				IsHereOnLoan = source.IsHereOnLoan,
				IsOutOnLoan = source.IsOutOnLoan,
				IsSold = source.IsSold
			};
		}

		private Models.Squad.PlayerTableData GetPlayerTableData(Database.Entities.Squad.Player player)
		{
			var data = new Models.Squad.PlayerTableData();

			var startedButNoEvents = _dbClient.GetGamesWherePlayerStartedButNoEventsAsync(player.Id).Result;
			if (startedButNoEvents.Any())
			{
				startedButNoEvents.ForEach(game =>
				{
					data.NumberOfGames++;
					data.NumberOfGamesFromStart++;
					data.NumberOfMinutesPlayed += 90;
				});
			}

			if (player.PlayerEvents == null || !player.PlayerEvents.Any())
			{
				return data;
			}

			var games = player.PlayerEvents.Select(x => x.Game).Where(y => y.OnDate < DateTime.Now.AddHours(4)).Distinct().ToList();
			var gamesFromStart = games.Where(x => x.Lineup != null &&  x.Lineup.Players.Select(y => y.PlayerId).Contains(player.Id)).ToList();

			var gamesFromStartSubstitutedOut = gamesFromStart.Where(x => x.PlayerEvents.Any(y => y.Type == GameEventType.SubstituteOut && y.PlayerId == player.Id)).ToList();
			var gamesFromStartNotSubstitutedOut = gamesFromStart.Where(x => !x.PlayerEvents.Any(y => y.Type == GameEventType.SubstituteOut && y.PlayerId == player.Id)).ToList();
			var gamesSubstitutedIn = games.Where(x => x.PlayerEvents.Any(y => y.Type == GameEventType.SubstituteIn && y.PlayerId == player.Id)).ToList();

			if (gamesFromStartNotSubstitutedOut.Any())
			{
				data.NumberOfMinutesPlayed += 90 * gamesFromStartNotSubstitutedOut.Count;
			}

			if (gamesFromStartSubstitutedOut.Any())
			{
				foreach (var game in gamesFromStartSubstitutedOut)
				{
					var playerEvent = game.PlayerEvents.FirstOrDefault(e => e.Type == GameEventType.SubstituteOut && e.PlayerId == player.Id);
					if (playerEvent != null)
					{
						data.NumberOfMinutesPlayed += playerEvent.InMinuteOfGame;
					}
				}
			}
			if (gamesSubstitutedIn.Any())
			{
				foreach (var game in gamesSubstitutedIn)
				{
					var playerEvent = game.PlayerEvents.FirstOrDefault(e => e.Type == GameEventType.SubstituteIn && e.PlayerId == player.Id);
					if (playerEvent != null)
					{
						data.NumberOfMinutesPlayed += 90 + game.NumberOfAddonMinutes - playerEvent.InMinuteOfGame;
					}
				}
			}

			data.NumberOfGames += games.Count;
			data.NumberOfGamesFromStart += gamesFromStart.Count;
			data.NumberOfGamesSubstituteIn = player.PlayerEvents.Count(x => x.Type == GameEventType.SubstituteIn);
			data.NumberOfGamesSubstituteOut = player.PlayerEvents.Count(x => x.Type == GameEventType.SubstituteOut);
			data.NumberOfGoals = player.PlayerEvents.Count(x => x.Type == GameEventType.Goal);
			data.NumberOfAssists = player.PlayerEvents.Count(x => x.Type == GameEventType.Assist);
			data.NumberOfYellowCards = player.PlayerEvents.Count(x => x.Type == GameEventType.YellowCard);
			data.NumberOfRedCards = player.PlayerEvents.Count(x => x.Type == GameEventType.RedCard);
			data.NumberOfPoints = data.NumberOfGoals + data.NumberOfAssists;

			return data;
		}

		public Models.Squad.Lineup Convert(Database.Entities.Squad.Lineup source, Models.Squad.Lineup destination, ResolutionContext context)
		{
			return new Models.Squad.Lineup
			{
				Id = source.Id,
				Formation = new Models.Squad.Formation
				{
					Id = source.FormationId,
					Name = source.Formation.Name,
					ComponentName = FormationList.All().FirstOrDefault(f => f.Name == source.Formation.Name)?.ComponentName
				},
				Players = context.Mapper.Map<List<Models.Squad.PlayerToLineup>>(source.Players),
				Type = source.Type,
				Created = source.Created.ToString("yyyy-MM-dd")
			};
		}

		public Models.Squad.PlayerToLineup Convert(Database.Entities.Squad.PlayerToLineup source, Models.Squad.PlayerToLineup destination, ResolutionContext context)
		{
			return new Models.Squad.PlayerToLineup
			{
				Id = source.Id,
				Player = new Models.Squad.PlayerToLineupPlayer
				{
					Id = source.PlayerId,
					FullName = $"{source.Player.FirstName} {source.Player.LastName}",
					ShortName = $"{source.Player.FirstName[0]}. {source.Player.LastName.ToUpper()}",
					Attributes = context.Mapper.Map<Models.Squad.PlayerAttributes>(source.Player),
					AvailablePositions = source.Player.AvailablePositions.Select(x => new Models.Squad.Position
					{
						Id = x.Position.Id,
						Name = x.Position.Name
					})
				},
				Position = new Models.Squad.Position
				{
					Id = source.Position.Id,
					Name = source.Position.Name
				}
			};
		}

		public Database.Entities.Squad.Lineup Convert(Models.Squad.CRUD.Lineup source, Database.Entities.Squad.Lineup destination, ResolutionContext context)
		{
			return new Database.Entities.Squad.Lineup
			{
				Id = source.Id,
				FormationId = source.FormationId,
				Players = source.Players.Select(player => new Database.Entities.Squad.PlayerToLineup
				{
					PlayerId = player.PlayerId,
					PositionId = player.PositionId
				}).ToList(),
				CreatedByUserId = source.CreatedByUserId,
				Type = source.Type,
				Created = DateTime.Now
			};
		}

		public Models.Squad.Player Convert(Database.Entities.User.FavoritePlayer source, Models.Squad.Player destination, ResolutionContext context)
		{
			return new Models.Squad.Player
			{
				Id = source.PlayerId,
				FirstName = source.Player.FirstName,
				LastName = source.Player.LastName,
				KitNumber = source.Player.KitNumber,
				IsOutOnLoan = source.Player.IsOutOnLoan,
				IsViceCaptain = source.Player.IsViceCaptain,
				IsHereOnLoan = source.Player.IsHereOnLoan,
				IsCaptain = source.Player.IsCaptain,
				IsSold = source.Player.IsSold
			};
		}

		public Database.Entities.Squad.Player Convert(Models.Squad.CRUD.Player source, Database.Entities.Squad.Player destination, ResolutionContext context)
		{
			return new Database.Entities.Squad.Player
			{
				Id = source.Id,
				FirstName = source.FirstName,
				LastName = source.LastName,
				IsCaptain = source.Attributes.IsCaptain,
				IsOutOnLoan = source.Attributes.IsOutOnLoan,
				IsHereOnLoan = source.Attributes.IsHereOnLoan,
				IsViceCaptain = source.Attributes.IsViceCaptain,
				IsSold = source.Attributes.IsSold,
				KitNumber = source.KitNumber,
				BirthDay = source.BirthDay,
				HeightInCentimeters = source.HeightInCentimeters,
				Weight = source.Weight,
				PreferredFoot = source.PreferredFoot,
				About = source.About,
				ContractUntil = source.ContractUntil,
				ImageUrl = source.ImageUrl
			};
		}

		public Models.Squad.Game Convert(Database.Entities.Squad.Game source, Models.Squad.Game destination, ResolutionContext context)
		{
			return new Models.Squad.Game
			{
				Id = source.Id,
				Type = source.Type,
				ArenaType = source.ArenaType,
				Opponent = source.OpponentTeamName,
				NumberOfGoalsScoredByOpponent = source.NumberOfGoalsScoredByOpponent,
				NumberOfAddonMinutes = source.NumberOfAddonMinutes,
				Lineup = source.Lineup != null ? context.Mapper.Map<Models.Squad.Lineup>(source.Lineup) : null,
				PlayedOn = source.OnDate.ToString("yyyy-MM-dd HH:mm"),
				PlayerEvents = context.Mapper.Map<IEnumerable<Models.Squad.PlayerEvent>>(source.PlayerEvents)
			};
		}

		public Models.Squad.PlayerEvent Convert(Database.Entities.Squad.PlayerEvent source, Models.Squad.PlayerEvent destination, ResolutionContext context)
		{
			return new Models.Squad.PlayerEvent
			{
				Id = source.Id,
				Player = new Models.Squad.EventPlayer
				{
					Id = source.PlayerId,
					FullName = $"{source.Player.FirstName} {source.Player.LastName}"
				},
				EventType = source.Type,
				InMinute = source.InMinuteOfGame
			};
		}

		public Models.Squad.PlayerEventOnPlayer Convert(Database.Entities.Squad.PlayerEvent source, Models.Squad.PlayerEventOnPlayer destination, ResolutionContext context)
		{
			return new Models.Squad.PlayerEventOnPlayer
			{
				GameId = source.GameId,
				GameType = source.Game.Type,
				EventType = source.Type,
				Date = source.Game.OnDate.ToString("yyyy-MM-dd")
			};
		}

		public Database.Entities.Squad.Game Convert(Models.Squad.CRUD.Game source, Database.Entities.Squad.Game destination, ResolutionContext context)
		{
			return new Database.Entities.Squad.Game
			{
				Id = source.Id,
				Type = source.Type,
				ArenaType = source.ArenaType,
				OpponentTeamName = source.Opponent,
				NumberOfGoalsScoredByOpponent = source.NumberOfGoalsScoredByOpponent,
				NumberOfAddonMinutes = source.NumberOfAddonMinutes,
				OnDate = System.Convert.ToDateTime(source.PlayedDate)
			};
		}

		public Models.Squad.Title Convert(Database.Entities.Squad.Title source, Models.Squad.Title destination, ResolutionContext context)
		{
			return new Models.Squad.Title
			{
				Id = source.Id,
				Type = source.Type,
				Year = source.Year,
				Description = source.Description
			};
		}

		public Database.Entities.Squad.GameResultGuess Convert(Models.Squad.CRUD.GameResultGuess source, Database.Entities.Squad.GameResultGuess destination, ResolutionContext context)
		{
			return new Database.Entities.Squad.GameResultGuess
			{
				GameId = source.GameId,
				NumberOfGoalsScoredByDif = source.NumberOfGoalsScoredByDif,
				NumberOfGoalsScoredByOpponent = source.NumberOfGoalsScoredByOpponent,
				GuessedByUserId = source.GuessedByUserId,
				Created = DateTime.Now
			};
		}

		public Models.Squad.GameResultGuess Convert(Database.Entities.Squad.GameResultGuess source, Models.Squad.GameResultGuess destination, ResolutionContext context)
		{
			return new Models.Squad.GameResultGuess
			{
				Game = context.Mapper.Map<Models.Squad.Game>(source.Game),
				NumberOfGoalsScoredByDif = source.NumberOfGoalsScoredByDif,
				NumberOfGoalsScoredByOpponent = source.NumberOfGoalsScoredByOpponent,
				GuessedByUser = new IdAndNickNameUser
				{
					Id = source.GuessedByUserId,
					NickName = source.GuessedByUser.NickNames.Current()
				}
			};
		}
	}
}
