using System;
using System.IO;
using System.Threading.Tasks;
using AzureService.Core.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureService.Core.FileStorage
{
	public class AzureStorage : IFileStorage
	{
		private readonly IApplicationConfiguration _configuration;

		public AzureStorage(IApplicationConfiguration configuration)
		{
			if(null == configuration) throw new ArgumentNullException("configuration");

			_configuration = configuration;
		}

		public async Task<Stream> GetSourceFileStreamAsync(string fileName)
		{
			ConfigurationOptions options = await _configuration.GetApplicationConfigurationAsync();
			return await getBlobStream(fileName, options.DestinationBlobContainerName, options.StorageConnectionString);
		}

		public async Task<Stream> GetDestinationFileStreamAsync(string fileName)
		{
			ConfigurationOptions options = await _configuration.GetApplicationConfigurationAsync();
			return await getBlobStream(fileName, options.DestinationBlobContainerName, options.StorageConnectionString);
		}

		private async Task<Stream> getBlobStream(string fileName, string containerName, string connectionString)
		{
			var container = await getBlobContainerAsync(connectionString, containerName);

			var blob = container.GetBlockBlobReference(fileName);

			if (null == blob) return null;

			await blob.FetchAttributesAsync();
			return await blob.OpenReadAsync();
		}

		private async Task<CloudBlobContainer> getBlobContainerAsync(string connectionString, string containerName)
		{
			CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
			CloudBlobClient blobClient = account.CreateCloudBlobClient();
			CloudBlobContainer container = blobClient.GetContainerReference(containerName);

			await container.CreateIfNotExistsAsync();

			return container;
		}
	}
}