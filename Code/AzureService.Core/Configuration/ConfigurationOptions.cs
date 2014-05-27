namespace AzureService.Core.Configuration
{
	public class ConfigurationOptions
	{
		public ConfigurationOptions(string storageConnectionString, string sourceBlobContainerName,
			string destinationBlobContainerName, string processingQueueName, string processingTaskTableName)
		{
			StorageConnectionString = storageConnectionString;
			SourceSourceBlobContainerName = sourceBlobContainerName;
			DestinationDestinationBlobContainerName = destinationBlobContainerName;
			ProcessingQueueName = processingQueueName;
			ProcessingTaskTableName = processingTaskTableName;
		}

		public string StorageConnectionString { get; private set; }

		public string SourceSourceBlobContainerName { get; private set; }

		public string DestinationDestinationBlobContainerName { get; private set; }

		public string ProcessingQueueName { get; private set; }

		public string ProcessingTaskTableName { get; private set; }
	}
}