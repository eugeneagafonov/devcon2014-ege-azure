using System.Threading.Tasks;

namespace AzureService.Core.FileProcessingWorker
{
	public interface IFileProcessingWorker
	{
		Task DoWorkAsync();
	}
}