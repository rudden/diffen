using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Serilog;

namespace Diffen.Controllers.Api
{
	using Models;
	using Models.Other;
	using Repositories.Contracts;

	[Authorize]
	[Route("api/[controller]")]
	public class RegionsController : ControllerBase
	{
		private readonly IRegionRepository _regionRepository;

		private readonly ILogger _logger = Log.ForContext<RegionsController>();

		public RegionsController(IRegionRepository regionRepository)
		{
			_regionRepository = regionRepository;
		}

		[HttpGet]
		public Task<List<Region>> Get()
		{
			_logger.Debug("Requesting all regions");
			return _regionRepository.GetRegionsAsync();
		}

		[Authorize(Policy = "IsManager")]
		[HttpPost("create")]
		public Task<List<Result>> Create([FromBody] Models.Other.CRUD.Region region)
		{
			_logger.Debug("Requesting to create a new region");
			return _regionRepository.CreateRegionAsync(region);
		}
	}
}
