using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

namespace Diffen.Repositories
{
	using Contracts;
	using Database;
	using Database.Entities.Squad;
	using Helpers.Extensions;

	public class SquadRepository : ISquadRepository
	{
		private readonly DiffenDbContext _dbContext;

		public SquadRepository(DiffenDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<Lineup> GetLineupOnIdAsync(int lineupId)
		{
			return await _dbContext.Lineups.IncludeAll().FirstOrDefaultAsync(l => l.Id == lineupId);
		}

		public async Task<Lineup> GetLineupOnPostIdAsync(int postId)
		{
			return await _dbContext.Lineups.IncludeAll()
				.FirstOrDefaultAsync(l => _dbContext.LineupsOnPosts.Select(x => x.PostId).Contains(postId));
		}

		public async Task<IEnumerable<Player>> GetPlayersAsync()
		{
			return await _dbContext.Players.Where(x => !x.IsSold).OrderBy(x => x.LastName).ToListAsync();
		}

		public async Task<Player> GetPlayerOnIdAsync(int playerId)
		{
			return await _dbContext.Players.FindAsync(playerId);
		}

		public async Task<IEnumerable<Lineup>> GetLineupsOnUserAsync(string userId)
		{
			return await _dbContext.Lineups.IncludeAll().Where(l => l.CreatedByUserId == userId).ToListAsync();
		}

		public async Task<int> GetNumberOfLineupsOnPlayerIdAsync(int playerId)
		{
			return await _dbContext.PlayersToLineups.CountAsync(x => x.PlayerId == playerId);
		}

		public async Task<bool> AddLineupAsync(Lineup lineup)
		{
			lineup.Created = DateTime.Now;
			_dbContext.Lineups.Add(lineup);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<bool> AddPlayerAsync(Player player)
		{
			_dbContext.Players.Add(player);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<bool> UpdatePlayerAsync(Player player)
		{
			_dbContext.Players.Update(player);
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<bool> RemoveAllPlayerToUserForSpecificPlayer(int playerId)
		{
			_dbContext.FavoritePlayers.RemoveRange(_dbContext.FavoritePlayers.Where(x => x.PlayerId == playerId));
			return await _dbContext.SaveChangesAsync() >= 0;
		}

		public async Task<IEnumerable<Formation>> GetFormationsAsync()
		{
			return await _dbContext.Formations.ToListAsync();
		}

		public async Task<IEnumerable<Position>> GetPositionsAsync()
		{
			return await _dbContext.Positions.ToListAsync();
		}
	}
}
