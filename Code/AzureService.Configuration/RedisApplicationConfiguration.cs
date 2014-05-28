using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureService.Core.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using ConfigurationOptions = AzureService.Core.Configuration.ConfigurationOptions;

namespace AzureService.Configuration
{
	public class RedisApplicationConfiguration : IApplicationConfiguration
	{
		private readonly string _redisConnectionString;
		public RedisApplicationConfiguration(string redisConnectionString)
		{
			_redisConnectionString = redisConnectionString;
		}
		public async Task<ConfigurationOptions> GetApplicationConfigurationAsync()
		{
			using (ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(_redisConnectionString))
			{
				IDatabase cache = connection.GetDatabase();
				string config = await cache.StringGetAsync("configuration");

				if (null == config)
				{
					var options = getDefaultConfigurationOptions();
					string optionsString = JsonConvert.SerializeObject(options);
					await cache.StringSetAsync("configuration", optionsString);
					return options;
				}

				return JsonConvert.DeserializeObject<ConfigurationOptions>(config);
			}
		}

		private ConfigurationOptions getDefaultConfigurationOptions()
		{
			return new ConfigurationOptions(
				storageConnectionString: "UseDevelopmentStorage=true;",
				sourceBlobContainerName: "sourcefiles",
				destinationBlobContainerName: "destinationfiles",
				processingQueueName: "processing",
				processingTaskTableName: "processing"
				);
		}
	}
}
