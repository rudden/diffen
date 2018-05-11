using System.Linq;
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

		public async Task<List<ChronicleCategory>> GetChronicleCategoriesAsync()
		{
			return _mapper.Map<List<ChronicleCategory>>(await _dbClient.GetChronicleCategoriesAsync());
		}

		public async Task<List<Result>> CreateChronicleAsync(Models.Other.CRUD.Chronicle chronicle)
		{
			var newChronicle = _mapper.Map<Database.Entities.Other.Chronicle>(chronicle);
			var results = new List<Result>();
			results.Update(await _dbClient.CreateChronicleAsync(newChronicle), ResultMessages.CreateChronicle);
			if (results.Any(r => r.Type == ResultType.Failure))
			{
				return results;
			}

			var nullableNewChronicleId = (await _dbClient.GetChronicleOnSlugAsync(newChronicle.Slug))?.Id;
			if (nullableNewChronicleId == null)
			{
				return results;
			}

			var newChronicleId = (int)nullableNewChronicleId;
			if (chronicle.CategoryIds.Any())
			{
				await _dbClient.AddCategoriesToChronicleAsync(newChronicleId, chronicle.CategoryIds);
			}

			if (chronicle.NewCategoryNames != null && chronicle.NewCategoryNames.Any())
			{
				await _dbClient.CreateNewChronicleCategoriesAndConnectToNewChronicleWithIdAsync(newChronicleId, chronicle.NewCategoryNames.ToList());
			}

			return results;
		}

		public async Task<List<Result>> UpdateChronicleAsync(Models.Other.CRUD.Chronicle chronicle)
		{
			var existingChronicle = _mapper.Map<Database.Entities.Other.Chronicle>(chronicle);
			var results = new List<Result>();
			results.Update(await _dbClient.UpdateChronicleAsync(existingChronicle), ResultMessages.UpdateChronicle);
			if (results.Any(r => r.Type == ResultType.Failure))
			{
				return results;
			}

			await _dbClient.AddOrRemoveCategoriesToExistingChronicleAsync(existingChronicle.Id, chronicle.CategoryIds);

			if (chronicle.NewCategoryNames != null && chronicle.NewCategoryNames.Any())
			{
				await _dbClient.CreateNewChronicleCategoriesAndConnectToNewChronicleWithIdAsync(existingChronicle.Id, chronicle.NewCategoryNames.ToList());
			}

			return results;
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
