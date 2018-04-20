using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Serilog;

namespace Diffen.Controllers.Api
{
	using Models;
	using Models.Squad;
	using Repositories.Contracts;

	[Route("api/[controller]")]
	public class SquadsController : Controller
	{
		private readonly ISquadRepository _squadRepository;

		private readonly ILogger _logger = Log.ForContext<SquadsController>();

		public SquadsController(ISquadRepository squadRepository)
		{
			_squadRepository = squadRepository;
		}

		[HttpGet("players")]
		public Task<List<Player>> GetPlayers()
		{
			_logger.Debug("Requesting all players");
			return _squadRepository.GetPlayersAsync();
		}

		[HttpPost("players/create")]
		public Task<List<Result>> CreatePlayer([FromBody] Models.Squad.CRUD.Player player)
		{
			_logger.Debug("Requesting to create a new player");
			return _squadRepository.CreatePlayerAsync(player);
		}

		[HttpPost("players/update")]
		public Task<List<Result>> UpdatePlayer([FromBody] Models.Squad.CRUD.Player player)
		{
			_logger.Debug("Requesting to update an existing player");
			return _squadRepository.UpdatePlayerAsync(player);
		}

		[HttpGet("lineups/{id}")]
		public Task<Lineup> GetLineupOnId(int id)
		{
			_logger.Debug("Requesting a lineup with id {lineupId}", id);
			return _squadRepository.GetLineupOnIdAsync(id);
		}

		[HttpGet("lineups/post/{postId}")]
		public Task<Lineup> GetLineupOnPostId(int postId)
		{
			_logger.Debug("Requesting a lineup connected to a post with id {postId}", postId);
			return _squadRepository.GetLineupOnPostIdAsync(postId);
		}

		[HttpGet("lineups/user/{userId}")]
		public Task<List<Lineup>> GetLineupsOnUser(string userId)
		{
			_logger.Debug("Requesting all lineups created by user with id {userId}", userId);
			return _squadRepository.GetLineupsOnUserAsync(userId);
		}

		[HttpGet("formations")]
		public Task<List<Formation>> GetFormations()
		{
			_logger.Debug("Requesting all formations");
			return _squadRepository.GetFormationsAsync();
		}

		[HttpGet("positions")]
		public Task<List<Position>> GetPositions()
		{
			_logger.Debug("Requesting all positions");
			return _squadRepository.GetPositionsAsync();
		}

		[HttpPost, Route("lineups/create")]
		public Task<List<Result>> CreateLineup([FromBody] Models.Squad.CRUD.Lineup lineup)
		{
			_logger.Debug("Requesting to create a new lineup");
			return _squadRepository.CreateLineupAsync(lineup);
		}
	}
}
