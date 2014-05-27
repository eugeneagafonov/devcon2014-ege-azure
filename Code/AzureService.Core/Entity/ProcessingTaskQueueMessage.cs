using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureService.Core.Entity
{
	public class ProcessingTaskQueueMessage
	{
		public ProcessingTask Task { get; set; }

		public string PopReceipt { get; set; }

		public string Id { get; set; }
	}
}
