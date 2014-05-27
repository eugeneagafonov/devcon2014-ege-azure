﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using Autofac;
using Autofac.Core;
using Autofac.Integration.WebApi;
using AzureService.Core.Configuration;

namespace AzureService.Component
{
	public static class AppComposition
	{
		public static IDependencyResolver AssembleWebApiComponents(Assembly webApiAssembly,
	ConfigurationOptions options)
		{
			ContainerBuilder builder = createBuilderAndRegisterServices(options);

			builder.RegisterApiControllers(webApiAssembly);
			IContainer container = builder.Build();
			return new AutofacWebApiDependencyResolver(container);
		}

		private static ContainerBuilder createBuilderAndRegisterServices(ConfigurationOptions options)
		{
			// конфигурирую autofac IoC контейнер
			var builder = new ContainerBuilder();

			//// в качестве реализации IStorageService нужно брать AzureStorageService
			//builder.RegisterType<AzureStorageService>()
			//	.WithParameters(new Parameter[]
			//	{
			//		// в конструктор передаем два параметра. Так сложно нужно, чтобы
			//		// сделать контроллеры независимыми от реализации сервисов
			//		new NamedParameter("storageConnectionString", options.StorageConnectionString),
			//		new NamedParameter("containerName", options.BlobContainerName),
			//		new NamedParameter("pdfContainerName", options.BlobContainerNamePdf),
			//		new NamedParameter("conversionTaskTableName", options.ConversionTaskTableName)
			//	})
			//	.As<IStorageService>();

			//builder.RegisterType<AzureQueueService>()
			//	.WithParameters(new Parameter[]
			//	{
			//		new NamedParameter("storageConnectionString", options.StorageConnectionString),
			//		new NamedParameter("conversionQueueName", options.ConversionQueueName)
			//	})
			//	.As<IQueueService>();

			return builder;
		}
	}
}
