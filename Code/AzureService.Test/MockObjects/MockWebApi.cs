using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Autofac;
using Autofac.Integration.WebApi;
using AzureService.Component;
using AzureService.Configuration;
using AzureService.Core.Configuration;
using AzureService.Core.Entity;
using AzureService.Core.FileProcessing;
using AzureService.Core.FileProcessingWorker;
using AzureService.Core.FileStorageService;
using AzureService.Core.QueueService;
using AzureService.Core.TaskService;
using AzureService.Test.MockObjects.Services;
using AzureService.Web;
using Microsoft.Owin.Testing;
using Owin;

namespace AzureService.Test.MockObjects
{
	public static class MockWebApi
	{
		public static ILifetimeAwareComponent<IFileProcessingWorker> CreateFileProcessingWorker(
	IReadOnlyDictionary<string, IFileProcessor> converters)
		{
			//ContainerBuilder builder = createBuilderAndRegisterServices();
			ContainerBuilder builder = createBuilderAndRegisterMockServices();

			foreach (var entry in converters)
			{
				var service = entry.Value;
				var key = entry.Key;
				builder.Register(c => service).Keyed<IFileProcessor>(key);
			}

			IContainer container = builder.Build();
			var component = new AutofacLifetimeAwareComponent<IFileProcessingWorker>(container);

			return component;
		}

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
			//ContainerBuilder builder = createBuilderAndRegisterServices();
			ContainerBuilder builder = createBuilderAndRegisterMockServices();

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

			builder.RegisterType<AzureFileStorageService>()
				.As<IFileStorageService>();

			builder.RegisterType<AzureQueueService>()
				.As<IQueueService>();

			builder.RegisterType<AzureTableTaskService>()
				.As<ITaskService>();

			builder.RegisterType<FileProcessingWorker>()
				.As<IFileProcessingWorker>();

			return builder;
		}

		private static ContainerBuilder createBuilderAndRegisterMockServices()
		{
			// конфигурирую autofac IoC контейнер
			var builder = new ContainerBuilder();

			builder.RegisterType<DummyAppConfiguration>()
				.As<IApplicationConfiguration>();

			// -- меняем на тестовую реализацию
			builder.RegisterType<MockFileStorageService>()
				.As<IFileStorageService>();

			builder.RegisterType<MockQueueService>()
				.As<IQueueService>();

			builder.RegisterType<MockTaskService>()
				.As<ITaskService>();
			// ------------------------------------


			builder.RegisterType<FileProcessingWorker>()
				.As<IFileProcessingWorker>();

			return builder;
		}

		public static Dictionary<string, ProcessingTask> Tasks = new Dictionary<string, ProcessingTask>();

		public static ConcurrentQueue<ProcessingTaskQueueMessage> QueueMessages = 
			new ConcurrentQueue<ProcessingTaskQueueMessage>();

		public static Dictionary<string, FileData> SrcFiles = new Dictionary<string, FileData>();
		public static Dictionary<string, FileData> DstFiles = new Dictionary<string, FileData>();
	}
}
