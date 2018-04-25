using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using AutoMapper;

namespace Diffen.Repositories
{
	using Contracts;
	using Models;
	using Models.User;
	using Helpers.Extensions;
	using Database.Clients.Contracts;

	public class PmRepository : IPmRepository
	{
		private readonly IMapper _mapper;
		private readonly IDiffenDbClient _dbClient;

		public PmRepository(IMapper mapper, IDiffenDbClient dbClient)
		{
			_mapper = mapper;
			_dbClient = dbClient;
		}

		public async Task<List<PersonalMessage>> GetPmsSentFromUserToUserAsync(string fromUserId, string toUserId)
		{
			var personalMessages = await _dbClient.GetPmsSentFromUserToUserAsync(fromUserId, toUserId);
			return _mapper.Map<List<PersonalMessage>>(personalMessages);
		}

		public async Task<List<KeyValuePair<string, string>>> GetUsersWithConversationForUserAsKeyValuePairAsync(string userId)
		{
			var users = await _dbClient.GetUsersThatUserHasOngoingConversationWithAsync(userId);
			return users.Select(user =>
				new KeyValuePair<string, string>(user.Id,
					user.NickNames.Current())).ToList();
		}

		public Task<List<Result>> CreatePersonalMessageAsync(Models.User.CRUD.PersonalMessage pm)
		{
			return new List<Result>().Get(_dbClient.CreatePersonalMessageAsync(_mapper.Map<Database.Entities.User.PersonalMessage>(pm)), ResultMessages.CreatePm);
		}
	}
}
