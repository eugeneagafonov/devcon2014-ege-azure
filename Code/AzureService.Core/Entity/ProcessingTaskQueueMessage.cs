namespace AzureService.Core.Entity
{
	public class ProcessingTaskQueueMessage
	{
		public ProcessingTask Task { get; set; }

		public string PopReceipt { get; set; }

		public string Id { get; set; }
	}
}