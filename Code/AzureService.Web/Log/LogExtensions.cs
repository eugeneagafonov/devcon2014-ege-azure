using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;

namespace AzureService.Web.Log
{
	public static class LogExtensions
	{
		public static string CreateLogEntry(this ApiController controller, string message)
		{
			var user = new {Name = "eugene.agafonov@devcon2014.ru"};
			var env =
				new
				{
					Url = controller.Request.RequestUri,
					Headers = controller.Request.Headers.ToString()
				};

			var result = new {Timestamp = DateTime.UtcNow, User = user, Environment = env, Message = message};
			return JsonConvert.SerializeObject(result);
		}
	}
}