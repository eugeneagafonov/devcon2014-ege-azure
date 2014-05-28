using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureService.Core.Entity;

namespace AzureService.Test
{
	public class FileData
	{
		public FileMetadata Metadata { get; set; }

		public byte[] Content { get; set; }
	}
}
