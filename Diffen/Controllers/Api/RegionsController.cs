using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Serilog;

namespace Diffen.Controllers.Api
{
	using Models;
	using Models.Other;
	using Repositories.Contracts;

	[Route("api/[controller]")]
	public class RegionsController : Controller
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

		[HttpPost("create")]
		public Task<List<Result>> Create([FromBody] Models.Other.CRUD.Region region)
		{
			_logger.Debug("Requesting to create a new region");
			return _regionRepository.CreateRegionAsync(region);
		}
	}
}
