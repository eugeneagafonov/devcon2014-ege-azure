using System.Threading.Tasks;
using AzureService.Core.Entity;

namespace AzureService.Core.TaskService
{
	public interface ITaskService
	{
		Task SaveTaskAsync(ProcessingTask task);
		Task<ProcessingTask> GetTaskAsync(string taskId);
	}
}