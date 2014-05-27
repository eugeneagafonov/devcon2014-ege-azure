using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
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

			while (true)
			{
				Thread.Sleep(10000);
				Trace.TraceInformation("Working");
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