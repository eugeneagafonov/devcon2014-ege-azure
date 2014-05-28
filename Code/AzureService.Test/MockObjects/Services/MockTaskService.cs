using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureService.Core.Entity;
using AzureService.Core.TaskService;

namespace AzureService.Test.MockObjects.Services
{
	public class MockTaskService : ITaskService
	{
		public async Task SaveTaskAsync(ProcessingTask task)
		{
			if (!MockWebApi.Tasks.ContainsKey(task.Id))
				MockWebApi.Tasks.Add(task.Id, task);
			else
				MockWebApi.Tasks[task.Id] = task;
		}

		public async Task<ProcessingTask> GetTaskAsync(string taskId)
		{
			return MockWebApi.Tasks[taskId];
		}
	}
}
