using System.Threading.Tasks;
using System.Collections.Generic;

namespace Diffen.Repositories.Contracts
{
	using Database.Entities.Squad;

	public interface ISquadRepository
	{
		Task<Lineup> GetLineupOnIdAsync(int lineupId);
		Task<Lineup> GetLineupOnPostIdAsync(int postId);
		Task<IEnumerable<Player>> GetPlayersAsync();
		Task<Player> GetPlayerOnIdAsync(int playerId);
		Task<IEnumerable<Lineup>> GetLineupsOnUserAsync(string userId);
		Task<int> GetNumberOfLineupsOnPlayerIdAsync(int playerId);
		Task<bool> AddLineupAsync(Lineup lineup);
		Task<bool> AddPlayerAsync(Player player);
		Task<bool> UpdatePlayerAsync(Player player);
		Task<bool> RemoveAllPlayerToUserForSpecificPlayer(int playerId);
		Task<IEnumerable<Formation>> GetFormationsAsync();
		Task<IEnumerable<Position>> GetPositionsAsync();
	}
}
