using System.Diagnostics;
using System.Net.Http;

namespace AzureService.Web.Log
{
	public class ElasticSearchTraceListener : TraceListener
	{
		public override void Write(string message)
		{
			using (var httpClient = new HttpClient())
			{
				var msg = httpClient
					.PostAsync("http://localhost:9200/applog/logentry", new StringContent(message))
					.GetAwaiter()
					.GetResult();
			}
		}

		public override void WriteLine(string message)
		{
			Write(message);
		}
	}
}