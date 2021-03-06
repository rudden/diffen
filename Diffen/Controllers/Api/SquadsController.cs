﻿using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Serilog;

namespace Diffen.Controllers.Api
{
	using Models;
	using Models.Squad;
	using Helpers.Authorize;
	using Repositories.Contracts;

	[Authorize]
	[Route("api/[controller]")]
	public class SquadsController : ControllerBase
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

		[Authorize(Policy = "IsGameAdmin")]
		[HttpPost("players/create")]
		public Task<List<Result>> CreatePlayer([FromBody] Models.Squad.CRUD.Player player)
		{
			_logger.Debug("Requesting to create a new player");
			return _squadRepository.CreatePlayerAsync(player);
		}

		[Authorize(Policy = "IsGameAdmin")]
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

		[VerifyInputToLoggedInUserId("lineup", "CreatedByUserId")]
		[HttpPost, Route("lineups/create")]
		public Task<List<Result>> CreateLineup([FromBody] Models.Squad.CRUD.Lineup lineup)
		{
			_logger.Debug("Requesting to create a new lineup");
			return _squadRepository.CreateLineupAsync(lineup);
		}

		[HttpGet("games")]
		public Task<List<Game>> GetGames()
		{
			_logger.Debug("Requesting all games");
			return _squadRepository.GetGamesAsync();
		}

		[HttpGet("games/upcoming")]
		public Task<Game> GetUpcomingGame()
		{
			_logger.Debug("Requesting upcoming game");
			return _squadRepository.GetUpcomingGameAsync();
		}

		[Authorize(Policy = "IsGameAdmin")]
		[HttpPost, Route("games/create")]
		public Task<bool> CreateGame([FromBody] Models.Squad.CRUD.Game game)
		{
			_logger.Debug("Requesting to create a new game");
			return _squadRepository.CreateGameAsync(game);
		}

		[Authorize(Policy = "IsGameAdmin")]
		[HttpPost, Route("games/update")]
		public Task<bool> UpdateGame([FromBody] Models.Squad.CRUD.Game game)
		{
			_logger.Debug("Requesting to update a game {@game}", game);
			return _squadRepository.UpdateGameAsync(game);
		}

		[HttpGet("games/result/finished")]
		public Task<List<GameResultGuessLeagueItem>> GetFinishedGameResultGuesses()
		{
			_logger.Debug("Requesting all finished game result guesses");
			return _squadRepository.GetFinishedGameResultGuessesAsync();
		}

		[VerifyInputToLoggedInUserId("guess", "GuessedByUserId")]
		[HttpPost, Route("games/result/guess")]
		public Task<bool> GuessGameResult([FromBody] Models.Squad.CRUD.GameResultGuess guess)
		{
			_logger.Debug("Requesting to guess a result for a game");
			return _squadRepository.CreateGameResultGuessAsync(guess);
		}

		[HttpGet("titles")]
		public Task<List<Title>> GetTitles()
		{
			_logger.Debug("Requesting all titles");
			return _squadRepository.GetTitlesAsync();
		}

		[HttpGet("seasons")]
		public Task<List<Season>> GetSeasons()
		{
			_logger.Debug("Requesting all seasons including games");
			return _squadRepository.GetSeasons();
		}
	}
}
