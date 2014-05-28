using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Autofac;
using Autofac.Integration.WebApi;
using AzureService.Configuration;
using AzureService.Core.Configuration;
using AzureService.Core.FileProcessingWorker;
using AzureService.Core.FileStorageService;
using AzureService.Core.QueueService;
using AzureService.Core.TaskService;
using AzureService.Web;
using Microsoft.Owin.Testing;
using Owin;

namespace AzureService.Test.MockObjects
{
	public static class MockWebApi
	{
		public static TestServer CreateServer()
		{
			return TestServer.Create(app => app.UseWebApi(createConfig()));
		}

		private static HttpConfiguration createConfig()
		{
			var config = new HttpConfiguration();

			config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}",
				new { id = RouteParameter.Optional });

			config.Routes.MapHttpRoute("VersionedApi", "api/1.0/{controller}/{id}",
				new { id = RouteParameter.Optional });

			config.DependencyResolver = assembleWebApiComponents(typeof(Startup).Assembly);

			return config;
		}

		private static IDependencyResolver assembleWebApiComponents(Assembly webApiAssembly)
		{
			ContainerBuilder builder = createBuilderAndRegisterServices();

			builder.RegisterApiControllers(webApiAssembly);
			IContainer container = builder.Build();
			return new AutofacWebApiDependencyResolver(container);
		}

		private static ContainerBuilder createBuilderAndRegisterServices()
		{
			// конфигурирую autofac IoC контейнер
			var builder = new ContainerBuilder();

			builder.RegisterType<DummyAppConfiguration>()
				.As<IApplicationConfiguration>();

			// -- меняем на тестовую реализацию
			builder.RegisterType<AzureFileStorageService>()
				.As<IFileStorageService>();

			builder.RegisterType<AzureQueueService>()
				.As<IQueueService>();

			builder.RegisterType<AzureTableTaskService>()
				.As<ITaskService>();
			// ------------------------------------


			builder.RegisterType<FileProcessingWorker>()
				.As<IFileProcessingWorker>();

			return builder;
		}
	}
}
