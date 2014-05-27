using AzureService.Core.Configuration;

namespace AzureService.Configuration
{
	public static class AppConfiguration
	{
		public static IApplicationConfiguration GetDummyConfiguration()
		{
			return new DummyAppConfiguration();
		}

		public static IApplicationConfiguration GetRedisConfiguration(string server, string connectionString)
		{
			return new DummyAppConfiguration();
		}
	}
}