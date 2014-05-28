using System;

namespace AzureService.Core.Entity
{
	public class FileMetadata
	{
		public string BlobFileName { get; set; }

		public string MimeContentType { get; set; }

		public DateTime UploadedDateUtc { get; set; }

		public string ETag { get; set; }
	}
}