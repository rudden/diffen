using System.Threading.Tasks;
using System.Collections.Generic;

namespace Diffen.Repositories.Contracts
{
	using Models;
	using Models.Other;

	public interface IPollRepository
	{
		Task<List<Poll>> GetAllPollsAsync();
		Task<List<Poll>> GetActivePollsAsync();
		Task<List<Poll>> GetLastNthActivePollsAsync(int amount = 5);
		Task<List<Poll>> GetPollsForUserWithIdAsync(string userId);
		Task<Poll> GetPollOnIdAsync(int pollId);
		Task<List<Result>> CreateNewPollAsync(Models.Other.CRUD.Poll poll);
		Task<List<Result>> CreateVoteOnPollAsync(Models.Other.CRUD.PollVote pollVote);
	}
}
