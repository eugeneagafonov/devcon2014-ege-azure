using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
