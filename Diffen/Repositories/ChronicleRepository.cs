using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

using AutoMapper;

namespace Diffen.Repositories
{
	using Contracts;
	using Models;
	using Models.Other;
	using Helpers.Extensions;
	using Database.Clients.Contracts;

	public class ChronicleRepository : IChronicleRepository
	{
		private readonly IMapper _mapper;
		private readonly IDiffenDbClient _dbClient;

		private readonly IHttpContextAccessor _httpContextAccessor;

		public ChronicleRepository(IMapper mapper, IDiffenDbClient dbClient, IHttpContextAccessor httpContextAccessor)
		{
			_mapper = mapper;
			_dbClient = dbClient;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<List<Chronicle>> GetChroniclesAsync(int amount = 0)
		{
			return _mapper.Map<List<Chronicle>>(await _dbClient.GetChroniclesAsync(amount));
		}

		public async Task<List<Chronicle>> GetAllChroniclesOnUserIdAsync(string userId)
		{
			return _mapper.Map<List<Chronicle>>(await _dbClient.GetChroniclesOnUserIdAsync(userId));
		}

		public async Task<Chronicle> GetChronicleOnIdAsync(int chronicleId)
		{
			return _mapper.Map<Chronicle>(await _dbClient.GetChronicleOnIdAsync(chronicleId));
		}

		public async Task<Chronicle> GetChronicleOnSlugAsync(string slug)
		{
			return _mapper.Map<Chronicle>(await _dbClient.GetChronicleOnSlugAsync(slug));
		}

		public Task<List<Result>> CreateChronicleAsync(Models.Other.CRUD.Chronicle chronicle)
		{
			return new List<Result>().Get(_dbClient.CreateChronicleAsync(_mapper.Map<Database.Entities.Other.Chronicle>(chronicle)), ResultMessages.CreateChronicle);
		}

		public Task<List<Result>> UpdateChronicleAsync(Models.Other.CRUD.Chronicle chronicle)
		{
			return new List<Result>().Get(_dbClient.UpdateChronicleAsync(_mapper.Map<Database.Entities.Other.Chronicle>(chronicle)), ResultMessages.UpdateChronicle);
		}

		public async Task<List<Result>> UpdateHeaderFileNameForLastAddedChronicleAsync(string fileName)
		{
			var chronicle = await _dbClient.GetLastAddedChronicleAsync();
			chronicle.HeaderFileName = fileName;
			return await new List<Result>().Get(_dbClient.SetHeaderFileNameOnChronicleAsync(chronicle), ResultMessages.UpdateChronicleHeaderFile);
		}

		public Task<bool> SelectedChronicleIsCreatedByLoggedInUserAsync(string slug)
		{
			return _dbClient.ChronicleIsCreatedBySelectedUserAsync(slug, _httpContextAccessor.HttpContext.User.GetUserId());
		}
	}
}
