using System.Threading.Tasks;
using System.Collections.Generic;

using AutoMapper;

namespace Diffen.Repositories
{
	using Contracts;
	using Models;
	using Models.Other;
	using Database.Clients.Contracts;

	public class PollRepository : IPollRepository
	{
		private readonly IMapper _mapper;
		private readonly IDiffenDbClient _dbClient;

		public PollRepository(IMapper mapper, IDiffenDbClient dbClient)
		{
			_mapper = mapper;
			_dbClient = dbClient;
		}

		public async Task<List<Poll>> GetAllPollsAsync()
		{
			return _mapper.Map<List<Poll>>(await _dbClient.GetPollsAsync());
		}

		public async Task<List<Poll>> GetActivePollsAsync()
		{
			return _mapper.Map<List<Poll>>(await _dbClient.GetActivePollsAsync());
		}

		public async Task<List<Poll>> GetLastNthActivePollsAsync(int amount = 5)
		{
			return _mapper.Map<List<Poll>>(await _dbClient.GetLastNthActivePollsAsync(amount));
		}

		public async Task<List<Poll>> GetPollsForUserWithIdAsync(string userId)
		{
			return _mapper.Map<List<Poll>>(await _dbClient.GetPollsOnUserIdAsync(userId));
		}

		public async Task<Poll> GetPollOnIdAsync(int pollId)
		{
			return _mapper.Map<Poll>(await _dbClient.GetPollOnIdAsync(pollId));
		}

		public async Task<List<Result>> CreateNewPollAsync(Models.Other.CRUD.Poll poll)
		{
			var newPoll = _mapper.Map<Database.Entities.Other.Poll>(poll);

			var isCreated = _dbClient.CreatePollAsync(newPoll);
			var results = await new List<Result>().Get(isCreated, ResultMessages.CreatePoll);

			if (!await isCreated)
			{
				return results;
			}

			var selections = _mapper.Map<List<Database.Entities.Other.PollSelection>>(poll.Selections);
			foreach (var selection in selections)
			{
				selection.PollId = newPoll.Id;
			}
			await _dbClient.CreatePollSelectionsAsync(selections);

			return results;
		}

		public Task<List<Result>> CreateVoteOnPollAsync(Models.Other.CRUD.PollVote pollVote)
		{
			return new List<Result>().Get(_dbClient.CreateVoteOnPollAsync(_mapper.Map<Database.Entities.Other.PollVote>(pollVote)), ResultMessages.CreatePollVote);
		}
	}
}
