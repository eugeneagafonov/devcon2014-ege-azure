using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Dependencies;
using Autofac;
using Autofac.Integration.WebApi;
using AzureService.Configuration;
using AzureService.Core.Configuration;
using AzureService.Core.FileProcessing;
using AzureService.Core.FileProcessingWorker;
using AzureService.Core.FileStorageService;
using AzureService.Core.QueueService;
using AzureService.Core.TaskService;

namespace AzureService.Component
{
	public static class AppComposition
	{
		public static IDependencyResolver AssembleWebApiComponents(Assembly webApiAssembly,
			string configurationConnectionString)
		{
			ContainerBuilder builder = createBuilderAndRegisterServices(configurationConnectionString);

			builder.RegisterApiControllers(webApiAssembly);
			IContainer container = builder.Build();
			return new AutofacWebApiDependencyResolver(container);
		}

		public static ILifetimeAwareComponent<IFileProcessingWorker> CreateFileProcessingWorker(
			IReadOnlyDictionary<string, IFileProcessor> converters,
			string configurationConnectionString)
		{
			ContainerBuilder builder = createBuilderAndRegisterServices(configurationConnectionString);

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

		private static ContainerBuilder createBuilderAndRegisterServices(string configurationConnectionString)
		{
			// конфигурирую autofac IoC контейнер
			var builder = new ContainerBuilder();

			//builder.RegisterType<DummyAppConfiguration>()
			//	.As<IApplicationConfiguration>();

			builder.RegisterType<RedisApplicationConfiguration>()
				.WithParameter("redisConnectionString", configurationConnectionString)
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
	}
}