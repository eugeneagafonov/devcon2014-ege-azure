using System;
using System.Linq;
using System.Threading.Tasks;
using AzureService.Core.Configuration;
using AzureService.Core.Entity;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureService.Core.TaskService
{
	public class AzureTableTaskService : ITaskService
	{
		private readonly IApplicationConfiguration _configuration;

		public AzureTableTaskService(IApplicationConfiguration configuration)
		{
			if (null == configuration) throw new ArgumentNullException("configuration");

			_configuration = configuration;
		}

		public async Task SaveTaskAsync(ProcessingTask task)
		{
			if(null == task) throw new ArgumentNullException("task");

			ConfigurationOptions options = await _configuration.GetApplicationConfigurationAsync();
			CloudTable table = await getTableReferenceAsync(options);
			var operation = TableOperation.InsertOrReplace(new ProcessingTaskEntity(task));
			await table.ExecuteAsync(operation);
		}

		public async Task<ProcessingTask> GetTaskAsync(string taskId)
		{
			if(string.IsNullOrWhiteSpace(taskId)) throw new ArgumentNullException("taskId");

			ConfigurationOptions options = await _configuration.GetApplicationConfigurationAsync();
			CloudTable table = await getTableReferenceAsync(options);

			// добавить информацию о пользователе
			var entity = table.CreateQuery<ProcessingTaskEntity>()
				.Where(t => t.RowKey == taskId)
				.SingleOrDefault();

			return null == entity ? null : entity.ToProcessingTask();
		}

		private async Task<CloudTable> getTableReferenceAsync(ConfigurationOptions options)
		{
			CloudStorageAccount account = CloudStorageAccount.Parse(options.StorageConnectionString);
			CloudTableClient tableClient = account.CreateCloudTableClient();
			CloudTable table = tableClient.GetTableReference(options.ProcessingTaskTableName);

			await table.CreateIfNotExistsAsync();
			return table;
		}
	}
}
