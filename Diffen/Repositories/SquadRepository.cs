using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Caching.Memory;

using AutoMapper;

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

		public async Task<bool> CreateGameAsync(Models.Squad.CRUD.Game game)
		{
			var newGame = _mapper.Map<Database.Entities.Squad.Game>(game);
			var result = await _dbClient.CreateGameAsync(newGame);
			if (!result)
			{
				return false;
			}
			return await _dbClient.CreatePlayerEventsAsync(game.Events.Select(x => new Database.Entities.Squad.PlayerEvent
			{
				PlayerId = x.PlayerId,
				Type = x.Type,
				GameId = newGame.Id
			}).ToList());
		}

		public async Task<bool> UpdateGameAsync(Models.Squad.CRUD.Game game)
		{
			var updateGame = _mapper.Map<Database.Entities.Squad.Game>(game);
			var existingGame = await _dbClient.GetGameOnIdAsync(game.Id);

			if (!existingGame.OnDate.Equals(updateGame.OnDate) || !existingGame.Type.Equals(updateGame.Type))
			{
				await _dbClient.UpdateGameAsync(updateGame);
			}

			await _dbClient.DeletePlayerEventsOnGameIdAsync(updateGame.Id);
			return await _dbClient.CreatePlayerEventsAsync(game.Events.Select(x => new Database.Entities.Squad.PlayerEvent
			{
				PlayerId = x.PlayerId,
				Type = x.Type,
				GameId = updateGame.Id
			}).ToList());
		}
	}
}
