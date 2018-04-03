using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

namespace Diffen.Repositories
{
	using Database;
	using Contracts;
	using Helpers.Extensions;
	using Database.Entities.User;

	public class PmRepository : IPmRepository
	{
		private readonly DiffenDbContext _dbContext;

		public PmRepository(DiffenDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IEnumerable<PersonalMessage>> GetPmsSentFromUserToUserAsync(string fromUserId, string toUserId)
		{
			return await _dbContext.PersonalMessages.IncludeAll()
				.Where(x => (x.FromUserId == fromUserId || x.FromUserId == toUserId) && (x.ToUserId == toUserId || x.ToUserId == fromUserId))
				.OrderByDescending(x => x.Created).ToListAsync();
		}

		public async Task<IEnumerable<AppUser>> GetUsersWithConversationForUserAsync(string userId)
		{
			var all = await _dbContext.PersonalMessages.IncludeAll()
				.Where(x => x.FromUserId == userId || x.ToUserId == userId).ToListAsync();
			var from = all.Select(x => x.FromUser);
			var to = all.Select(x => x.ToUser);

			var users = from.Distinct().Union(to.Distinct()).ToList();
			users.RemoveAll(x => x.Id == userId);
			return users;
		}

		public async Task<bool> AddPmAsync(PersonalMessage pm)
		{
			_dbContext.PersonalMessages.Add(pm);
			return await _dbContext.SaveChangesAsync() >= 0;
		}
	}
}
