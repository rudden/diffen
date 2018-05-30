using System.Threading.Tasks;
using System.Collections.Generic;

namespace Diffen.Repositories.Contracts
{
	using Models;
	using Models.Squad;

	public interface ISquadRepository
	{
		Task<Lineup> GetLineupOnIdAsync(int lineupId);
		Task<Lineup> GetLineupOnPostIdAsync(int postId);
		Task<List<Player>> GetPlayersAsync();
		Task<Player> GetPlayerOnIdAsync(int playerId);
		Task<List<Lineup>> GetLineupsOnUserAsync(string userId);
		Task<List<Result>> CreateLineupAsync(Models.Squad.CRUD.Lineup lineup);
		Task<List<Result>> CreatePlayerAsync(Models.Squad.CRUD.Player player);
		Task<List<Result>> UpdatePlayerAsync(Models.Squad.CRUD.Player player);
		Task<List<Position>> GetPositionsAsync();
		Task<List<Formation>> GetFormationsAsync();
		Task<List<Game>> GetGamesAsync();
		Task<Game> GetUpcomingGameAsync();
		Task<bool> CreateGameAsync(Models.Squad.CRUD.Game game);
		Task<bool> UpdateGameAsync(Models.Squad.CRUD.Game game);
		Task<List<Title>> GetTitlesAsync();
		Task<bool> CreateGameResultGuessAsync(Models.Squad.CRUD.GameResultGuess guess);
		Task<List<GameResultGuessLeagueItem>> GetFinishedGameResultGuessesAsync();
		Task<List<Season>> GetSeasons();
	}
}
