using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AzureService.Component;
using AzureService.Core.FileProcessing;
using AzureService.Core.FileProcessingWorker;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace AzureService.Worker
{
	public class WorkerRole : RoleEntryPoint
	{
		public override void Run()
		{
			// This is a sample worker implementation. Replace with your logic.
			Trace.TraceInformation("AzureService.Worker entry point called");

			var converters = new Dictionary<string, IFileProcessor>();
			converters["ToUpper"] = new ToUpperFileProcessor();

			ILifetimeAwareComponent<IFileProcessingWorker> app = AppComposition.CreateFileProcessingWorker(
				converters,CloudConfigurationManager.GetSetting("redisConnectionString"));

			using (app.LifetimeScope)
			{
				while (true)
				{
					// нельзя использовать await здесь, метод должен быть блокирующимся
					Task.Delay(10000).GetAwaiter().GetResult();
					app.Service.DoWorkAsync().GetAwaiter().GetResult();
				}
			}
		}

		public override bool OnStart()
		{
			// Set the maximum number of concurrent connections 
			ServicePointManager.DefaultConnectionLimit = 12;

			// For information on handling configuration changes
			// see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

			return base.OnStart();
		}

		private static string getRoleSetting(string name)
		{
			return getRoleSetting(name, string.Empty);
		}

		private static string getRoleSetting(string name, string defaultValue)
		{
			try
			{
				return CloudConfigurationManager.GetSetting(name) ?? defaultValue;
			}
			catch (Exception e)
			{
				Trace.TraceError("No setting '{0}' in worker role configuration.", name);
				Trace.TraceError(e.ToString());
				return defaultValue;
			}
		}
	}
}