using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

using Serilog;

namespace Diffen.Controllers.Api
{
	using Models;
	using Models.Other;
	using Helpers.Authorize;
	using Repositories.Contracts;

	[Authorize]
	[Route("api/[controller]")]
	public class ChroniclesController : Controller
	{
		private readonly IChronicleRepository _chronicleRepository;

		private readonly ILogger _logger = Log.ForContext<ChroniclesController>();

		private const string DirectoryForChronicleHeaderUploads = "uploads/chronicles";

		public ChroniclesController(IChronicleRepository chronicleRepository, IHostingEnvironment environment)
		{
			_chronicleRepository = chronicleRepository;
		}

		[HttpGet]
		public Task<List<Chronicle>> Get(int amount = 0)
		{
			_logger.Debug("Requesting chronicles");
			return _chronicleRepository.GetChroniclesAsync(amount);
		}

		[HttpGet("user/{userId}")]
		public Task<List<Chronicle>> GetOnUserId(string userId)
		{
			_logger.Debug("Requesting all chronicles created by user with id {userId}", userId);
			return _chronicleRepository.GetAllChroniclesOnUserIdAsync(userId);
		}

		[HttpGet("{slug}")]
		public Task<Chronicle> Get(string slug)
		{
			_logger.Debug("Requesting chronicle with slug {slug}", slug);
			return _chronicleRepository.GetChronicleOnSlugAsync(slug);
		}

		[HttpGet("categories")]
		public Task<List<ChronicleCategory>> GetAllCategories()
		{
			_logger.Debug("Requesting all chronicle categories");
			return _chronicleRepository.GetChronicleCategoriesAsync();
		}

		[Authorize(Policy = "IsAuthor")]
		[VerifyInputToLoggedInUserId("chronicle", "WrittenByUserId")]
		[HttpPost("create")]
		public Task<List<Result>> Create([FromBody] Models.Other.CRUD.Chronicle chronicle)
		{
			_logger.Debug("Requesting to create a new chronicle");
			return _chronicleRepository.CreateChronicleAsync(chronicle);
		}

		[Authorize(Policy = "IsAuthor")]
		[VerifyInputToLoggedInUserId("chronicle", "WrittenByUserId")]
		[HttpPost("update")]
		public Task<List<Result>> Update([FromBody] Models.Other.CRUD.Chronicle chronicle)
		{
			_logger.Debug("Requesting to update an existing chronicle");
			return _chronicleRepository.UpdateChronicleAsync(chronicle);
		}

		[Authorize(Policy = "IsAuthor")]
		[HttpPost("image/header/update/{fileName}")]
		public async Task<bool> UpdateHeaderFileName(string fileName)
		{
			_logger.Debug("Requesting to update an existing chronicles header filename");
			try
			{
				var isUpdated = await _chronicleRepository.UpdateHeaderFileNameForLastAddedChronicleAsync($"{DirectoryForChronicleHeaderUploads}/{fileName}");
				return isUpdated.Any(x => x.Type == ResultType.Success);
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
