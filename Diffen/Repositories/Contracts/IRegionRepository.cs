using System.Threading.Tasks;
using System.Collections.Generic;

namespace Diffen.Repositories.Contracts
{
	using Models;
	using Models.Other;

	public interface IRegionRepository
	{
		Task<List<Region>> GetRegionsAsync();
		Task<List<Result>> CreateRegionAsync(Models.Other.CRUD.Region region);
	}
}
