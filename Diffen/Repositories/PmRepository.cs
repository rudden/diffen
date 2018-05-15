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

			var markAsRead = personalMessages.Where(pm => pm.ToUserId == fromUserId && !pm.IsReadByToUser).Select(x => x.Id).ToList();
			if (markAsRead.Any())
			{
				await MarkPersonalMessageAsReadByUserOnIdAsync(markAsRead);
			}
			return _mapper.Map<List<PersonalMessage>>(personalMessages);
		}

		public async Task<List<Conversation>> GetUsersWithConversationForUserAsKeyValuePairAsync(string userId)
		{
			var users = await _dbClient.GetUsersThatUserHasOngoingConversationWithAsync(userId);
			var conversations = new List<Conversation>();
			foreach (var user in users)
			{
				var conversation = new Conversation
				{
					User = new IdAndNickNameUser
					{
						Id = user.Id,
						NickName = user.NickNames.Current()
					},
					NumberOfUnReadMessages = await _dbClient.GetNumberOfUnReadPersonalMessagesFromUserToUserAsync(user.Id, userId)
				};
				conversations.Add(conversation);
			}
			return conversations;
		}

		public Task<List<Result>> CreatePersonalMessageAsync(Models.User.CRUD.PersonalMessage pm)
		{
			return new List<Result>().Get(_dbClient.CreatePersonalMessageAsync(_mapper.Map<Database.Entities.User.PersonalMessage>(pm)), ResultMessages.CreatePm);
		}

		public async Task<bool> MarkPersonalMessageAsReadByUserOnIdAsync(IEnumerable<int> pmIds)
		{
			var tasks = pmIds.Select(id => _dbClient.MarkPmAsReadByOnIdAsync(id));
			var results = await Task.WhenAll(tasks);
			return results.Any(result => !result);
		}

		public Task<int> GetAllUnReadMessagesForUserWithIdAsync(string userId)
		{
			return _dbClient.GetNumberOfUnReadPersonalMessagesForUserWithIdAsync(userId);
		}
	}
}
