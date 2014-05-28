using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureService.Core.Entity;
using AzureService.Core.QueueService;

namespace AzureService.Test.MockObjects.Services
{
	public class MockQueueService : IQueueService
	{
		public async Task<ProcessingTaskQueueMessage> EnqueueConversionTaskAsync(ProcessingTask task)
		{
			var msg = new ProcessingTaskQueueMessage
			{
				Id = Guid.NewGuid().ToString(),
				PopReceipt = "",
				Task = task
			};

			MockWebApi.QueueMessages.Enqueue(msg);
			return msg;
		}

		public async Task<ProcessingTaskQueueMessage> DequeueConversionTaskAsync()
		{
			ProcessingTaskQueueMessage msg;
			while(!MockWebApi.QueueMessages.TryDequeue(out msg));
			return msg;
		}

		public async Task DeleteConversionTaskAsync(string id, string popReceipt)
		{
			// ничего делать не надо
			return;
		}
	}
}
