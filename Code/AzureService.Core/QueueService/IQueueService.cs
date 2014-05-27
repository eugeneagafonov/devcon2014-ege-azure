using System.Threading.Tasks;
using AzureService.Core.Entity;

namespace AzureService.Core.QueueService
{
	public interface IQueueService
	{
		Task<ProcessingTaskQueueMessage> EnqueueConversionTaskAsync(ProcessingTask task);
		Task<ProcessingTaskQueueMessage> DequeueConversionTaskAsync();
		Task DeleteConversionTaskAsync(string id, string popReceipt);
	}
}