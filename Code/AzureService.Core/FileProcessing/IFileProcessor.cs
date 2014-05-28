using System.IO;
using System.Threading.Tasks;

namespace AzureService.Core.FileProcessing
{
	public interface IFileProcessor
	{
		string Operation { get; }

		Task ProcessFileAsync(Stream sourceFileStream, Stream destinationFileStream);
	}
}