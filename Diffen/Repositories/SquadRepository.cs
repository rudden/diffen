using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Caching.Memory;

using AutoMapper;
using Diffen.Helpers.Extensions;
using Serilog;

namespace Diffen.Repositories
{
	using Contracts;
	using Models;
	using Models.Squad;
	using Database.Clients.Contracts;

	public class SquadRepository : ISquadRepository
	{
		private readonly IMapper _mapper;
		private readonly IMemoryCache _cache;
		private readonly IDiffenDbClient _dbClient;

		private readonly ILogger _logger = Log.ForContext<SquadRepository>();

		public SquadRepository(IMapper mapper, IMemoryCache cache, IDiffenDbClient dbClient)
		{
			_mapper = mapper;
			_cache = cache;
			_dbClient = dbClient;
		}

		public async Task<Lineup> GetLineupOnIdAsync(int lineupId)
		{
			var lineup = await _dbClient.GetLineupOnIdAsync(lineupId);
			return _mapper.Map<Lineup>(lineup);
		}

		public async Task<Lineup> GetLineupOnPostIdAsync(int postId)
		{
			var lineup = await _dbClient.GetLineupOnPostIdAsync(postId);
			return _mapper.Map<Lineup>(lineup);
		}

		public async Task<List<Player>> GetPlayersAsync()
		{
			var players = await _dbClient.GetPlayersAsync();
			return _mapper.Map<List<Player>>(players);
		}

		public async Task<Player> GetPlayerOnIdAsync(int playerId)
		{
			var player = await _dbClient.GetPlayerOnIdAsync(playerId);
			return _mapper.Map<Player>(player);
		}

		public async Task<List<Lineup>> GetLineupsOnUserAsync(string userId)
		{
			var lineups = await _dbClient.GetLineupsCreatedByUserIdAsync(userId);
			return _mapper.Map<List<Lineup>>(lineups);
		}

		public Task<List<Result>> CreateLineupAsync(Models.Squad.CRUD.Lineup lineup)
		{
			var newLineup = _mapper.Map<Database.Entities.Squad.Lineup>(lineup);
			return new List<Result>().Get(_dbClient.CreateLineupAsync(newLineup), ResultMessages.CreateLineup);
		}

		public async Task<List<Result>> CreatePlayerAsync(Models.Squad.CRUD.Player player)
		{
			var newPlayer = _mapper.Map<Database.Entities.Squad.Player>(player);
			await _dbClient.UpdateAvailablePositionsForPlayerAsync(player.Id, player.AvailablePositionsIds);
			return await new List<Result>().Get(_dbClient.CreatePlayerAsync(newPlayer), ResultMessages.CreatePlayer);
		}

		public async Task<List<Result>> UpdatePlayerAsync(Models.Squad.CRUD.Player player)
		{
			var updatePlayer = _mapper.Map<Database.Entities.Squad.Player>(player);
			if (updatePlayer.IsSold)
			{
				updatePlayer.KitNumber = 0;
				await _dbClient.DeleteFavoritePlayerRelationToUserForPlayerAsync(player.Id);
			}
			await _dbClient.UpdateAvailablePositionsForPlayerAsync(player.Id, player.AvailablePositionsIds);
			return await new List<Result>().Get(_dbClient.UpdatePlayerAsync(updatePlayer), ResultMessages.UpdatePlayer);
		}

		public async Task<List<Formation>> GetFormationsAsync()
		{
			if (_cache.TryGetValue("formations", out List<Formation> formations))
			{
				return formations;
			}

			var allFormations = await _dbClient.GetFormationsAsync();
			_cache.Set("formations", _mapper.Map<List<Formation>>(allFormations.OrderBy(x => x.Name)));
			return _cache.Get<List<Formation>>("formations");
		}

		public async Task<List<Position>> GetPositionsAsync()
		{
			if (_cache.TryGetValue("positions", out List<Position> positions))
			{
				return positions;
			}

			var allPositions = await _dbClient.GetPositionsAsync();
			_cache.Set("positions", _mapper.Map<List<Position>>(allPositions.OrderBy(x => x.Name)));
			return _cache.Get<List<Position>>("positions");
		}

		public async Task<List<Game>> GetGamesAsync()
		{
			var games = await _dbClient.GetGamesAsync();
			return _mapper.Map<List<Game>>(games);
		}

		public async Task<Game> GetUpcomingGameAsync()
		{
			var game = await _dbClient.GetUpcomingGameAsync();
			return _mapper.Map<Game>(game);
		}

		public async Task<bool> CreateGameAsync(Models.Squad.CRUD.Game game)
		{
			var newGame = _mapper.Map<Database.Entities.Squad.Game>(game);
			var result = await _dbClient.CreateGameAsync(newGame);
			if (!result)
			{
				return false;
			}
			await ComplementGameWithPotentialLineupAndEventsAsync(newGame.Id, game);
			return true;
		}

		public async Task<bool> UpdateGameAsync(Models.Squad.CRUD.Game game)
		{
			var updateGame = _mapper.Map<Database.Entities.Squad.Game>(game);
			var existingGame = await _dbClient.GetGameOnIdAsync(game.Id);

			try
			{
				var result = false;
				if (
					!existingGame.OnDate.Equals(updateGame.OnDate) ||
					!existingGame.Type.Equals(updateGame.Type) ||
					!existingGame.ArenaType.Equals(updateGame.ArenaType) ||
					!existingGame.OpponentTeamName.Equals(updateGame.OpponentTeamName) ||
					!existingGame.NumberOfGoalsScoredByOpponent.Equals(updateGame.NumberOfGoalsScoredByOpponent))
				{
					result = await _dbClient.UpdateGameAsync(updateGame);
				}

				await _dbClient.DeletePlayerEventsOnGameIdAsync(updateGame.Id);
				await ComplementGameWithPotentialLineupAndEventsAsync(updateGame.Id, game, existingGame);
				return result;
			}
			catch (Exception e)
			{
				_logger.Warning(e, "An unexpected error occured when updating game");
				return false;
			}
		}

		private async Task ComplementGameWithPotentialLineupAndEventsAsync(int gameId, Models.Squad.CRUD.Game game, Database.Entities.Squad.Game existingGame = null)
		{
			try
			{
				if (game.Lineup != null) {
					if (existingGame != null && existingGame.Lineup == null)
					{
						var lineup = _mapper.Map<Database.Entities.Squad.Lineup>(game.Lineup);
						await _dbClient.CreateLineupAsync(lineup);
						await _dbClient.UpdateGameWithLineupAsync(gameId, lineup.Id);
					}
				}
				if (game.Events.Any()) {
					await _dbClient.CreatePlayerEventsAsync(game.Events.Select(x => new Database.Entities.Squad.PlayerEvent
					{
						PlayerId = x.PlayerId,
						Type = x.Type,
						GameId = gameId,
						InMinuteOfGame = x.InMinute
					}).ToList());
				}
			}
			catch (Exception e)
			{
				_logger.Warning(e, "An unexpected error occured when complementing game");
			}
	}

		public async Task<List<Title>> GetTitlesAsync()
		{
			var titles = await _dbClient.GetTitlesAsync();
			return _mapper.Map<List<Title>>(titles);
		}

		public Task<bool> CreateGameResultGuessAsync(Models.Squad.CRUD.GameResultGuess guess)
		{
			var newGuess = _mapper.Map<Database.Entities.Squad.GameResultGuess>(guess);
			return _dbClient.CreateGameResultGuessAsync(newGuess);
		}

		public async Task<List<GameResultGuessLeagueItem>> GetFinishedGameResultGuessesAsync()
		{
			var guesses = await _dbClient.GetFinishedGameResultGuessesAsync();
			return guesses.Select(x => x.GuessedByUser)
				.Distinct()
				.Select(user => new GameResultGuessLeagueItem
				{
					User = new IdAndNickNameUser
					{
						Id = user.Id,
						NickName = user.NickNames.Current()
					},
					Guesses = _mapper.Map<List<GameResultGuess>>(guesses.Where(x => x.GuessedByUserId.Equals(user.Id) && x.Game.OnDate < DateTime.Now))
				})
				.ToList();
		}
	}
}
