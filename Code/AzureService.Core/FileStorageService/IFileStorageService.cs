using System.IO;
using System.Threading.Tasks;
using AzureService.Core.Entity;

namespace AzureService.Core.FileStorageService
{
	public interface IFileStorageService
	{
		Task<FileMetadata> SaveSourceFileStreamAsync(Stream stream, string contentType);
		Task<FileMetadata> SaveDestinationFileStreamAsync(Stream stream, string contentType);
		Task<Stream> GetSourceFileStreamAsync(string fileName);
		Task<Stream> GetDestinationFileStreamAsync(string fileName);
	}
}