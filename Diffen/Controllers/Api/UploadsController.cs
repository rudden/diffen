using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Serilog;

namespace Diffen.Controllers.Api
{
	using Repositories.Contracts;

	[Route("api/[controller]")]
	public class UploadsController : Controller
	{
		private readonly IUploadRepository _uploadRepository;

		private readonly ILogger _logger = Log.ForContext<UploadsController>();

		public UploadsController(IUploadRepository uploadRepository)
		{
			_uploadRepository = uploadRepository;
		}

		[HttpPost("{directoryName}")]
		[RequestSizeLimit(700_000)]
		public Task<string> Upload(string directoryName, string uniqueId = null)
		{
			_logger.Debug("Requesting to upload a new file to directory {directoryName}", directoryName);
			try
			{
				var file = Request.Form.Files.First();
				return _uploadRepository.UploadFileAsync(directoryName, file, uniqueId);
			}
			catch (Exception e)
			{
				_logger.Warning(e, "Could not upload file");
				throw;
			}
		}
	}
}
