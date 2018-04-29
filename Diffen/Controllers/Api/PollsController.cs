using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Serilog;

namespace Diffen.Controllers.Api
{
	using Models;
	using Models.Other;
	using Repositories.Contracts;

	[Route("api/[controller]")]
	public class PollsController : Controller
	{
		private readonly IPollRepository _pollRepository;

		private readonly ILogger _logger = Log.ForContext<PollsController>();

		public PollsController(IPollRepository pollRepository)
		{
			_pollRepository = pollRepository;
		}

		[HttpGet]
		public Task<List<Poll>> Get()
		{
			_logger.Debug("Requesting all polls");
			return _pollRepository.GetAllPollsAsync();
		}

		[HttpGet("{slug}")]
		public Task<Poll> Get(string slug)
		{
			_logger.Debug("Requesting poll with slug {slug}", slug);
			return _pollRepository.GetPollOnSlugAsync(slug);
		}

		[HttpGet("active")]
		public Task<List<Poll>> GetActive(int amount)
		{
			_logger.Debug("Requesting all active polls");
			return amount == 0
				? _pollRepository.GetActivePollsAsync()
				: _pollRepository.GetLastNthActivePollsAsync(amount);
		}

		[HttpGet("user/{userId}")]
		public Task<List<Poll>> GetOnUserId(string userId)
		{
			_logger.Debug("Requesting all polls created by user with id {userId}", userId);
			return _pollRepository.GetPollsForUserWithIdAsync(userId);
		}

		[HttpPost("create")]
		public Task<List<Result>> Create([FromBody] Models.Other.CRUD.Poll poll)
		{
			_logger.Debug("Requesting to create a new poll");
			return _pollRepository.CreateNewPollAsync(poll);
		}

		[HttpPost("{pollId}/vote")]
		public Task<List<Result>> Vote(int pollId, [FromBody] Models.Other.CRUD.PollVote pollVote)
		{
			_logger.Debug("Requesting to create a new vote on poll with id {pollId}", pollId);
			return _pollRepository.CreateVoteOnPollAsync(pollVote);
		}
	}
}
