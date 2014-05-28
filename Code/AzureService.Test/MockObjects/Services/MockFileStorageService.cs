using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.Metadata;
using AzureService.Core.Entity;
using AzureService.Core.Extension;
using AzureService.Core.FileStorageService;

namespace AzureService.Test.MockObjects.Services
{
	public class MockFileStorageService : IFileStorageService
	{
		public async Task<FileMetadata> SaveSourceFileStreamAsync(Stream stream, string contentType)
		{
			var data = new FileData();
			var metadata = new FileMetadata();
			metadata.MimeContentType = contentType;
			metadata.BlobFileName = Guid.NewGuid().ToShortString();
			metadata.UploadedDateUtc = DateTime.UtcNow;

			data.Content = readFully(stream);
			data.Metadata = metadata;

			MockWebApi.SrcFiles.Add(metadata.BlobFileName, data);
			return metadata;
		}

		public async Task<FileMetadata> SaveDestinationFileStreamAsync(Stream stream, string contentType)
		{
			var data = new FileData();
			var metadata = new FileMetadata();
			metadata.MimeContentType = contentType;
			metadata.BlobFileName = Guid.NewGuid().ToShortString();
			metadata.UploadedDateUtc = DateTime.UtcNow;

			data.Content = readFully(stream);
			data.Metadata = metadata;

			MockWebApi.DstFiles.Add(metadata.BlobFileName, data);
			return metadata;
		}

		public async Task<Stream> GetSourceFileStreamAsync(string fileName)
		{
			var file = MockWebApi.SrcFiles[fileName];
			return new MemoryStream(file.Content);
		}

		public async Task<Stream> GetDestinationFileStreamAsync(string fileName)
		{
			var file = MockWebApi.DstFiles[fileName];
			return new MemoryStream(file.Content);
		}

		private static byte[] readFully(Stream input)
		{
			byte[] buffer = new byte[16 * 1024];
			using (MemoryStream ms = new MemoryStream())
			{
				int read;
				while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
				{
					ms.Write(buffer, 0, read);
				}
				return ms.ToArray();
			}
		}
	}
}
