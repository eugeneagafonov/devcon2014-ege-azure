using System.Threading.Tasks;
using AzureService.Core.Configuration;

namespace AzureService.Configuration
{
	public class DummyAppConfiguration : IApplicationConfiguration
	{
		public async Task<ConfigurationOptions> GetApplicationConfigurationAsync()
		{
			return new ConfigurationOptions(
				storageConnectionString: "DefaultEndpointsProtocol=https;AccountName=devcon2014;AccountKey=apaYha7glRpWv0lg9KWgQyF0TNbty9Oy2rPz0CKwCq4m2JTA0q/K/GByLn7GyyLM+dlXXU8RBVtMtA/mahCr0A==",
				sourceBlobContainerName: "sourcefiles", // имя контейнера должно быть в нижнем регистре!!
				destinationBlobContainerName: "destinationfiles", // имя контейнера должно быть в нижнем регистре!!
				processingQueueName: "processing",
				processingTaskTableName:	"processing"
				);
		}
	}
}