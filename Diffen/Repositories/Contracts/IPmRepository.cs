using System.Threading.Tasks;
using System.Collections.Generic;

namespace Diffen.Repositories.Contracts
{
	using Models;
	using Models.User;

	public interface IPmRepository
	{
		Task<List<PersonalMessage>> GetPmsSentFromUserToUserAsync(string fromUserId, string toUserId);
		Task<List<Conversation>> GetUsersWithConversationForUserAsKeyValuePairAsync(string userId);
		Task<List<Result>> CreatePersonalMessageAsync(Models.User.CRUD.PersonalMessage pm);
		Task<bool> MarkPersonalMessageAsReadByUserOnIdAsync(IEnumerable<int> pmIds);
		Task<int> GetAllUnReadMessagesForUserWithIdAsync(string userId);
	}
}
