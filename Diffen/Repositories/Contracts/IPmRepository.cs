using System.Threading.Tasks;
using System.Collections.Generic;

namespace Diffen.Repositories.Contracts
{
	using Models;
	using Models.User;

	public interface IPmRepository
	{
		Task<List<PersonalMessage>> GetPmsSentFromUserToUserAsync(string fromUserId, string toUserId);
		Task<List<KeyValuePair<string, string>>> GetUsersWithConversationForUserAsKeyValuePairAsync(string userId);
		Task<List<Result>> CreatePersonalMessageAsync(Models.User.CRUD.PersonalMessage pm);
	}
}
