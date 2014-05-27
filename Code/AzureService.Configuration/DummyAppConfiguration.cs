using AzureService.Core.Configuration;

namespace AzureService.Configuration
{
	internal class DummyAppConfiguration : IApplicationConfiguration
	{
		public ConfigurationOptions GetApplicationConfiguration()
		{
			return new ConfigurationOptions("UseDevelopmentStorage=true;", "sourceFiles", "destinationFiles", "processing",
				"processing"
				);
		}
	}
}