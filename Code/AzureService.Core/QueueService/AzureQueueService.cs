using System;
using System.Threading.Tasks;
using AzureService.Core.Configuration;
using AzureService.Core.Entity;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace AzureService.Core.QueueService
{
	public class AzureQueueService : IQueueService
	{
		private readonly IApplicationConfiguration _configuration;

		public AzureQueueService(IApplicationConfiguration configuration)
		{
			if (null == configuration) throw new ArgumentNullException("configuration");

			_configuration = configuration;
		}

		public async Task<ProcessingTaskQueueMessage> EnqueueConversionTaskAsync(ProcessingTask task)
		{
			ConfigurationOptions options = await _configuration.GetApplicationConfigurationAsync();
			CloudQueue queue = await getQueueReferenceAsync(options);

			string message = JsonConvert.SerializeObject(task);
			var msg = new CloudQueueMessage(message);

			await queue.AddMessageAsync(msg);

			return new ProcessingTaskQueueMessage {Id = null, PopReceipt = null, Task = task};
		}

		public async Task<ProcessingTaskQueueMessage> DequeueConversionTaskAsync()
		{
			ConfigurationOptions options = await _configuration.GetApplicationConfigurationAsync();
			CloudQueue queue = await getQueueReferenceAsync(options);

			CloudQueueMessage msg = await queue.GetMessageAsync();
			if (null == msg) return null;

			var task = JsonConvert.DeserializeObject<ProcessingTask>(msg.AsString);
			var result = new ProcessingTaskQueueMessage {Id = msg.Id, PopReceipt = msg.PopReceipt, Task = task};

			return result;
		}

		public async Task DeleteConversionTaskAsync(string id, string popReceipt)
		{
			ConfigurationOptions options = await _configuration.GetApplicationConfigurationAsync();
			CloudQueue queue = await getQueueReferenceAsync(options);
			await queue.DeleteMessageAsync(id, popReceipt);
		}

		private async Task<CloudQueue> getQueueReferenceAsync(ConfigurationOptions options)
		{
			if (null == options) throw new ArgumentNullException("options");

			CloudStorageAccount account = CloudStorageAccount.Parse(options.StorageConnectionString);
			CloudQueueClient queueClient = account.CreateCloudQueueClient();
			CloudQueue queue = queueClient.GetQueueReference(options.ProcessingQueueName);

			await queue.CreateIfNotExistsAsync();

			return queue;
		}
	}
}