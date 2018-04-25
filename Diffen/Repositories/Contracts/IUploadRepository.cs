using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Diffen.Repositories.Contracts
{
	public interface IUploadRepository
	{
		Task<string> UploadFileAsync(string directoryName, IFormFile file, string uniqueId = "");
		bool DeleteFilesInDirectory(string directoryName, Func<FileInfo, bool> predicate = null);
	}
}
