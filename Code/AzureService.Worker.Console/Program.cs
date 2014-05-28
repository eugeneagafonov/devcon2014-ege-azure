using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using AzureService.Component;
using AzureService.Core.FileProcessing;
using AzureService.Core.FileProcessingWorker;
using Newtonsoft.Json;

namespace AzureService.Worker.Console
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Trace.Listeners.Add(new ConsoleTraceListener()); // можно в конфиг файле

			Trace.TraceInformation("Console Worker entry point called");

			var converters = new Dictionary<string, IFileProcessor>();
			converters["ToUpper"] = new ToUpperFileProcessor();

			ILifetimeAwareComponent<IFileProcessingWorker> app = AppComposition.CreateFileProcessingWorker(
				converters, ConfigurationManager.ConnectionStrings["redisConnectionString"].ConnectionString);

			using (app.LifetimeScope)
			{
				while (true)
				{
					try
					{
						// нельзя использовать await здесь, метод должен быть блокирующимся
						Task.Delay(10000).GetAwaiter().GetResult();
						app.Service.DoWorkAsync().GetAwaiter().GetResult();
					}
					catch (Exception e)
					{
						Trace.TraceError("Ошибка обработки {0}", JsonConvert.SerializeObject(e));
					}
				}
			}
		}
	}
}