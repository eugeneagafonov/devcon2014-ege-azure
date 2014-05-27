using System.Threading.Tasks;

namespace AzureService.Core.Configuration
{
	public interface IApplicationConfiguration
	{
		Task<ConfigurationOptions> GetApplicationConfiguration();
	}
}