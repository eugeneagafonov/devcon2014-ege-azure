using System.Configuration;
using System.Diagnostics;
using System.Web.Http;
using AzureService.Component;
using AzureService.Configuration;
using AzureService.Core.Configuration;
using AzureService.Web;
using AzureService.Web.Log;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

// - будет использован при запуске OWIN приложения
[assembly: OwinStartup(typeof(Startup))]

namespace AzureService.Web
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			Trace.Listeners.Add(new ElasticSearchTraceListener());
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
			string configurationConnectionString = 
				ConfigurationManager.ConnectionStrings["redisConnectionString"].ConnectionString;
			
			config.DependencyResolver = 
				AppComposition.AssembleWebApiComponents(typeof(Startup).Assembly,
				configurationConnectionString);

			return config;
		}
	}
}