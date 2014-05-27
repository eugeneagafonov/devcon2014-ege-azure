using System;
using System.IO;
using System.Threading.Tasks;
using AzureService.Core.Configuration;
using AzureService.Core.Entity;
using AzureService.Core.Extension;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureService.Core.FileStorage
{
	public class AzureFileStorage : IFileStorage
	{
		private readonly IApplicationConfiguration _configuration;

		public AzureFileStorage(IApplicationConfiguration configuration)
		{
			if(null == configuration) throw new ArgumentNullException("configuration");

			_configuration = configuration;
		}

		public async Task<FileMetadata> SaveSourceFileStreamAsync(Stream stream, string contentType)
		{
			ConfigurationOptions options = await _configuration.GetApplicationConfigurationAsync();
			return await saveFileStreamAsync(
				stream, contentType, options.SourceBlobContainerName, options.StorageConnectionString);
		}

		public async Task<FileMetadata> SaveDestinationFileStreamAsync(Stream stream, string contentType)
		{
			ConfigurationOptions options = await _configuration.GetApplicationConfigurationAsync();
			return await saveFileStreamAsync(
				stream, contentType, options.DestinationBlobContainerName, options.StorageConnectionString);
		}

		public async Task<Stream> GetSourceFileStreamAsync(string fileName)
		{
			ConfigurationOptions options = await _configuration.GetApplicationConfigurationAsync();
			return await getBlobStreamAsync(fileName, options.SourceBlobContainerName, options.StorageConnectionString);
		}

		public async Task<Stream> GetDestinationFileStreamAsync(string fileName)
		{
			ConfigurationOptions options = await _configuration.GetApplicationConfigurationAsync();
			return await getBlobStreamAsync(fileName, options.DestinationBlobContainerName, options.StorageConnectionString);
		}

		private async Task<FileMetadata> saveFileStreamAsync(
			Stream stream, string contentType, string containerName, string connectionString)
		{
			var container = await getBlobContainerAsync(connectionString, containerName);

			string blobName = Guid.NewGuid().ToShortString();

			var blob = container.GetBlockBlobReference(blobName);
			await blob.UploadFromStreamAsync(stream);
			blob.Properties.ContentType = contentType;
			blob.SetProperties();
			blob.SetMetadata();

			return new FileMetadata
			{
				BlobFileName = blobName,
				MimeContentType = contentType,
				UploadedDateUtc = blob.Properties.LastModified.GetValueOrDefault().UtcDateTime,
				ETag = blob.Properties.ETag
			};
		}

		private async Task<Stream> getBlobStreamAsync(string fileName, string containerName, string connectionString)
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