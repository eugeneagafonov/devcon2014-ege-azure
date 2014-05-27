using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using Autofac;
using Autofac.Core;
using Autofac.Integration.WebApi;
using AzureService.Configuration;
using AzureService.Core.Configuration;
using AzureService.Core.FileStorageService;
using AzureService.Core.QueueService;

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

		private static ContainerBuilder createBuilderAndRegisterServices(string configurationConnectionString)
		{
			// конфигурирую autofac IoC контейнер
			var builder = new ContainerBuilder();

			builder.RegisterType<RedisApplicationConfiguration>()
				.WithParameter("redisConnectionString", configurationConnectionString)
				.As<IApplicationConfiguration>();

			builder.RegisterType<AzureFileStorageService>()
				.As<IFileStorageService>();

			builder.RegisterType<AzureQueueService>()
				.As<IQueueService>();

			//builder.RegisterType<AzureQueueService>()
			//	.WithParameters(new Parameter[]
			//	{
			//		new NamedParameter("storageConnectionString", options.StorageConnectionString),
			//		new NamedParameter("conversionQueueName", options.ProcessingQueueName)
			//	})
			//	.As<IQueueService>();

			return builder;
		}
	}
}
