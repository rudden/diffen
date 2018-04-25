using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace Diffen.Repositories
{
	using Contracts;
	using Models;
	using Models.Other;
	using Database.Clients.Contracts;

	public class ChronicleRepository : IChronicleRepository
	{
		private readonly IMapper _mapper;
		private readonly IDiffenDbClient _dbClient;
		private readonly IUploadRepository _uploadRepository;

		public ChronicleRepository(IMapper mapper, IDiffenDbClient dbClient, IUploadRepository uploadRepository)
		{
			_mapper = mapper;
			_dbClient = dbClient;
			_uploadRepository = uploadRepository;
		}

		public async Task<List<Chronicle>> GetAllChroniclesAsync()
		{
			return _mapper.Map<List<Chronicle>>(await _dbClient.GetChroniclesAsync());
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
	}
}
