using System.Web.Http;
using AzureService.Web.WebAPI;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

// - будет использован при запуске OWIN приложения
[assembly: OwinStartup(typeof(Startup))]

namespace AzureService.Web.WebAPI
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.UseCors(CorsOptions.AllowAll);
			app.UseWebApi(createConfig());
		}

		private HttpConfiguration createConfig()
		{
			var config = new HttpConfiguration();

			config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}",
				new {id = RouteParameter.Optional});

			config.Routes.MapHttpRoute("VersionedApi", "api/1.0/{controller}/{id}",
				new {id = RouteParameter.Optional});

			return registerServices(config);
		}

		private HttpConfiguration registerServices(HttpConfiguration config)
		{
			//ConfigurationOptions options = AppConfiguration.GetConfigurationOptions();

			//config.DependencyResolver = AppComposition.CreateWebApiComponents(typeof(Startup).Assembly,
			//	options);

			return config;
		}
	}
}