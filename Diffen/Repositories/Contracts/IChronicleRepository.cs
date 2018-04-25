using System.Threading.Tasks;
using System.Collections.Generic;

namespace Diffen.Repositories.Contracts
{
	using Models;
	using Models.Other;

	public interface IChronicleRepository
	{
		Task<List<Chronicle>> GetAllChroniclesAsync();
		Task<List<Chronicle>> GetAllChroniclesOnUserIdAsync(string userId);
		Task<Chronicle> GetChronicleOnIdAsync(int chronicleId);
		Task<Chronicle> GetChronicleOnSlugAsync(string slug);
		Task<List<Result>> CreateChronicleAsync(Models.Other.CRUD.Chronicle chronicle);
		Task<List<Result>> UpdateChronicleAsync(Models.Other.CRUD.Chronicle chronicle);
		Task<List<Result>> UpdateHeaderFileNameForLastAddedChronicleAsync(string fileName);
	}
}
