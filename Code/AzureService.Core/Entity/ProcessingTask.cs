namespace AzureService.Core.Entity
{
	public class ProcessingTask
	{
		public string Id { get; set; }

		public string UserId { get; set; }

		public TaskStatus Status { get; set; }

		public string OriginalFileName { get; set; }

		public FileMetadata SourceFile { get; set; }

		public FileMetadata DestinationFile { get; set; }
	}
}