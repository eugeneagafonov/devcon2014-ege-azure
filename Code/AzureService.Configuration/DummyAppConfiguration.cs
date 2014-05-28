using System.Threading.Tasks;
using AzureService.Core.Configuration;

namespace AzureService.Configuration
{
	public class DummyAppConfiguration : IApplicationConfiguration
	{
		public async Task<ConfigurationOptions> GetApplicationConfigurationAsync()
		{
			return new ConfigurationOptions(
				storageConnectionString:	"UseDevelopmentStorage=true;",
				sourceBlobContainerName: "sourcefiles", // имя контейнера должно быть в нижнем регистре!!
				destinationBlobContainerName: "destinationfiles", // имя контейнера должно быть в нижнем регистре!!
				processingQueueName: "processing",
				processingTaskTableName:	"processing"
				);
		}
	}
}