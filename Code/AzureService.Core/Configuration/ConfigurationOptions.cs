namespace AzureService.Core.Configuration
{
	public class ConfigurationOptions
	{
		public ConfigurationOptions(string storageConnectionString, string sourceBlobContainerName,
			string destinationBlobContainerName, string conversionQueueName, string conversionTaskTableName)
		{
			StorageConnectionString = storageConnectionString;
			SourceSourceBlobContainerName = sourceBlobContainerName;
			DestinationDestinationBlobContainerName = destinationBlobContainerName;
			ConversionQueueName = conversionQueueName;
			ConversionTaskTableName = conversionTaskTableName;
		}

		public string StorageConnectionString { get; private set; }

		public string SourceSourceBlobContainerName { get; private set; }

		public string DestinationDestinationBlobContainerName { get; private set; }

		public string ConversionQueueName { get; private set; }

		public string ConversionTaskTableName { get; private set; }
	}
}