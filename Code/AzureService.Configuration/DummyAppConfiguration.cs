using System.Threading.Tasks;
using AzureService.Core.Configuration;

namespace AzureService.Configuration
{
	public class DummyAppConfiguration : IApplicationConfiguration
	{
		public async Task<ConfigurationOptions> GetApplicationConfiguration()
		{
			return new ConfigurationOptions(
				storageConnectionString:	"UseDevelopmentStorage=true;",
				sourceBlobContainerName: "sourceFiles", 
				destinationBlobContainerName: "destinationFiles",
				processingQueueName: "processing",
				processingTaskTableName:	"processing"
				);
		}
	}
}