using System.Threading.Tasks;
using System.Collections.Generic;

namespace Diffen.Repositories.Contracts
{
	using Database.Entities.User;

	public interface IPmRepository
	{
		Task<IEnumerable<PersonalMessage>> GetPmsSentFromUserToUserAsync(string fromUserId, string toUserId);
		Task<IEnumerable<AppUser>> GetUsersWithConversationForUserAsync(string userId);
		Task<bool> AddPmAsync(PersonalMessage pm);
		Task<IEnumerable<PersonalMessage>> GetPmsAsync(string userId);
	}
}
