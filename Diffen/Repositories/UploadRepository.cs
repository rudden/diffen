using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace Diffen.Repositories
{
	using Contracts;

	public class UploadRepository : IUploadRepository
	{
		private const string UniqueIdAndFileNameDivider = "_____";

		private readonly string _basePathForFileUploads;

		public UploadRepository(IHostingEnvironment environment)
		{
			_basePathForFileUploads = Path.Combine(environment.WebRootPath, "uploads");
		}

		public async Task<string> UploadFileAsync(string directoryName, IFormFile file, string uniqueId = "")
		{
			try
			{
				if (file != null)
				{
					var id = string.IsNullOrEmpty(uniqueId) ? Guid.NewGuid().ToString() : uniqueId;
					var fileName = string.Concat(id, UniqueIdAndFileNameDivider, file.FileName);
					var basePath = Path.Combine(_basePathForFileUploads, directoryName);
					var path = Path.Combine(basePath, fileName);
					using (var fileStream = new FileStream(path, FileMode.Create))
					{
						await file.CopyToAsync(fileStream);
						return fileName;
					}
				}
				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public bool DeleteFilesInDirectory(string directoryName, Func<FileInfo, bool> predicate = null)
		{
			try
			{
				var files = new DirectoryInfo(Path.Combine(_basePathForFileUploads, directoryName)).GetFiles();
				if (predicate != null)
				{
					files = files.Where(predicate).ToArray();
				}
				foreach (var file in files)
				{
					file.Delete();
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
