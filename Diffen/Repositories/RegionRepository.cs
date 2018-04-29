using System;
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

	public class RegionRepository : IRegionRepository
	{
		private readonly IMapper _mapper;
		private readonly IDiffenDbClient _dbClient;

		public RegionRepository(IMapper mapper, IDiffenDbClient dbClient)
		{
			_mapper = mapper;
			_dbClient = dbClient;
		}

		public Task<List<Result>> CreateRegionAsync(Models.Other.CRUD.Region region)
		{
			return new List<Result>().Get(_dbClient.CreateRegionAsync(_mapper.Map<Database.Entities.Other.Region>(region)), ResultMessages.CreateRegion);
		}

		public async Task<List<Region>> GetRegionsAsync()
		{
			var regions = await _dbClient.GetRegionsAsync();
			return _mapper.Map<List<Region>>(regions);
		}
	}
}
