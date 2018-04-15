using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

using Serilog;
using AutoMapper;

namespace Diffen.Controllers.Api
{
	using Models;
	using Models.Squad;
	using Helpers.Extensions;
	using Repositories.Contracts;
	using Database.Entities.User;

	[Route("api/[controller]")]
	public class SquadsController : Controller
	{
		private readonly IMemoryCache _cache;
		private readonly IMapper _mapper;
		private readonly ISquadRepository _squadRepository;
		private readonly UserManager<AppUser> _userManager;

		private readonly ILogger _logger = Log.ForContext<SquadsController>();

		public SquadsController(IMemoryCache cache, IMapper mapper, ISquadRepository squadRepository, UserManager<AppUser> userManager)
		{
			_cache = cache;
			_mapper = mapper;
			_squadRepository = squadRepository;
			_userManager = userManager;
		}

		[HttpGet("players")]
		public async Task<IActionResult> GetPlayers()
		{
			try
			{
				return Json(_mapper.Map<List<Player>>(await _squadRepository.GetPlayersAsync()));

			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "GetPlayers: An unexpected error occured when trying to fetch players");
				return BadRequest();
			}
		}

		[HttpGet("players/{id}")]
		public async Task<IActionResult> GetPlayers(int id)
		{
			try
			{
				return Json(_mapper.Map<Player>(await _squadRepository.GetPlayerOnIdAsync(id)));
			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "GetPlayers: An unexpected error occured when trying to fetch player with id {playerId}", id);
				return BadRequest();
			}
		}

		[HttpPost("players/create")]
		public async Task<IActionResult> CreatePlayer([FromBody] Models.Squad.CRUD.Player player)
		{
			try
			{
				if (player == null)
					return BadRequest();

				if (!IsValid(player))
					return BadRequest("not valid");

				var results = new List<Result>();
				await _squadRepository.AddPlayerAsync(_mapper.Map<Database.Entities.Squad.Player>(player))
					.ContinueWith(task => task.UpdateResults(ResultMessages.CreatePlayer, results));
				return Json(results);
			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "CreatePlayer: An unexpected error occured when trying add a player");
				return BadRequest();
			}
		}

		[HttpPost("players/update")]
		public async Task<IActionResult> UpdatePlayer([FromBody] Models.Squad.CRUD.Player player)
		{
			try
			{
				if (player == null)
					return BadRequest();

				if (player.IsSold)
				{
					await _squadRepository.RemoveAllPlayerToUserForSpecificPlayer(player.Id);
				}
				if (!IsValid(player))
					return BadRequest("not valid");

				var results = new List<Result>();
				await _squadRepository.UpdatePlayerAsync(_mapper.Map<Database.Entities.Squad.Player>(player))
					.ContinueWith(task => task.UpdateResults(ResultMessages.UpdatePlayer, results));
				return Json(results);
			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "UpdatePlayer: An unexpected error occured when trying update a player");
				return BadRequest();
			}
		}

		[HttpGet("lineups/{id}")]
		public async Task<IActionResult> GetLineupOnId(int id)
		{
			try
			{
				return Json(_mapper.Map<Lineup>(await _squadRepository.GetLineupOnIdAsync(id)));
			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "GetLineupOnId: An unexpected error occured when trying to fetch lineup with id {lineupId}", id);
				return BadRequest();
			}
		}

		[HttpGet("lineups/post/{postId}")]
		public async Task<IActionResult> GetLineupOnPostId(int postId)
		{
			try
			{
				return Json(_mapper.Map<Lineup>(await _squadRepository.GetLineupOnPostIdAsync(postId)));
			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "GetLineupOnPostId: An unexpected error occured when trying to fetch lineup for post with id {postId}", postId);
				return BadRequest();
			}
		}

		[HttpGet("lineups/user/{userId}")]
		public async Task<IActionResult> GetLineupsOnUser(string userId)
		{
			try
			{
				return Json(_mapper.Map<List<Lineup>>(await _squadRepository.GetLineupsOnUserAsync(userId)));
			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "GetLineupsOnUser: An unexpected error occured when trying to fetch lineups for user with id {userId}", userId);
				return BadRequest();
			}
		}

		[HttpGet("formations")]
		public async Task<IActionResult> GetFormations()
		{
			try
			{
				if (_cache.TryGetValue("formations", out var formations))
				{
					return Json(formations);
				}

				var allFormations = await _squadRepository.GetFormationsAsync();
				_cache.Set("formations", allFormations.OrderBy(x => x.Name));
				return Json(allFormations);
			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "GetFormations: An unexpected error occured when trying to fetch formations");
				return BadRequest();
			}
		}

		[HttpGet("positions")]
		public async Task<IActionResult> GetPositions()
		{
			try
			{
				if (_cache.TryGetValue("positions", out var positions))
				{
					return Json(positions);
				}

				var allPositions = await _squadRepository.GetPositionsAsync();
				_cache.Set("positions", allPositions.OrderBy(x => x.Name));
				return Json(allPositions);
			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "GetPositions: An unexpected error occured when trying to fetch positions");
				return BadRequest();
			}
		}

		[HttpPost, Route("lineups/create")]
		public async Task<IActionResult> CreateLineup([FromBody] Models.Squad.CRUD.Lineup lineup)
		{
			try
			{
				if (lineup == null)
					return BadRequest();

				if (!lineup.CreatedByUserId.Equals(_userManager.GetUserId(User)))
				{
					return Forbid();
				}

				var results = new List<Result>();
				await _squadRepository.AddLineupAsync(_mapper.Map<Database.Entities.Squad.Lineup>(lineup))
					.ContinueWith(task => task.UpdateResults(ResultMessages.CreateLineup, results));

				return Json(results);
			}
			catch (Exception e)
			{
				_logger.Warning(e.Message, "CreateLineup: An unexpected error occured when trying to create a lineup for user with id {userId}", lineup?.CreatedByUserId);
				return BadRequest();
			}
		}

		private static bool IsValid(Models.Squad.CRUD.Player player)
		{
			return !string.IsNullOrEmpty(player.FirstName) && !string.IsNullOrEmpty(player.LastName) && player.KitNumber >= 0 && player.KitNumber < 100;
		}
	}
}
