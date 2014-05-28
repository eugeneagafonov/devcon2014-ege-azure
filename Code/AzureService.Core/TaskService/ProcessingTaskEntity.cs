using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureService.Core.Entity;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace AzureService.Core.TaskService
{
	internal class ProcessingTaskEntity : TableEntity
	{
		/// <summary>
		/// Должен быть конструктор без параметров
		/// </summary>
		public ProcessingTaskEntity()
		{
		}

		public ProcessingTaskEntity(ProcessingTask task)
		{
			PartitionKey = task.UserId;
			RowKey = task.Id;

			TaskString = JsonConvert.SerializeObject(task);
		}

		public ProcessingTask ToProcessingTask()
		{
			return JsonConvert.DeserializeObject<ProcessingTask>(TaskString);
		}

		public string TaskString { get; set; }
	}
}
