using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using AzureService.Component;
using AzureService.Core.Entity;
using AzureService.Core.FileProcessing;
using AzureService.Core.FileProcessingWorker;
using AzureService.Test.MockObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TaskStatus = AzureService.Core.Entity.TaskStatus;

namespace AzureService.Test
{
	[TestClass]
	public class ApiTests
	{
		[TestMethod]
		public void TaskPipelineTest()
		{
			var operationContent = new StringContent("ToUpper");
			var cd = new ContentDispositionHeaderValue("inline");
			cd.Name = "operation";
			operationContent.Headers.ContentDisposition = cd;

			var fileContent = new ByteArrayContent(File.ReadAllBytes(@"..\..\Source.txt"));
			cd = new ContentDispositionHeaderValue("attachment");
			cd.Name = "file";
			cd.FileName = "Source.txt";
			fileContent.Headers.ContentDisposition = cd;
			var md = new MediaTypeHeaderValue("text/plain");
			fileContent.Headers.ContentType = md;

			var multipartContent = new MultipartContent();
			multipartContent.Add(operationContent);
			multipartContent.Add(fileContent);

			using (var server = MockWebApi.CreateServer())
			{
				var response = server.CreateRequest("/api/ProcessingTask")
					.And(r => r.Content = multipartContent)
					.PostAsync()
					.GetAwaiter()
					.GetResult();
				
				Assert.IsTrue(HttpStatusCode.OK == response.StatusCode, "StatusCode should be OK");

				ProcessingTask task = response.Content.ReadAsAsync<ProcessingTask>().GetAwaiter().GetResult();

				string taskId = task.Id;
				Assert.IsTrue(TaskStatus.Enqueued == task.Status, "Task should be enqueued");

				response = server.CreateRequest("/api/ProcessingTaskStatus?taskId=" + taskId)
					.GetAsync()
					.GetAwaiter()
					.GetResult();

				Assert.IsTrue(HttpStatusCode.OK == response.StatusCode, "StatusCode should be OK");
				task = response.Content.ReadAsAsync<ProcessingTask>().GetAwaiter().GetResult();

				Assert.IsTrue(TaskStatus.Enqueued == task.Status, "Task should be still enqueued");

				var converters = new Dictionary<string, IFileProcessor>();
				converters["ToUpper"] = new ToUpperFileProcessor();

				ILifetimeAwareComponent<IFileProcessingWorker> app = 
					MockWebApi.CreateFileProcessingWorker(converters);
				using (app.LifetimeScope)
				{
						app.Service.DoWorkAsync().GetAwaiter().GetResult();
				}

				response = server.CreateRequest("/api/ProcessingTaskStatus?taskId=" + taskId)
					.GetAsync()
					.GetAwaiter()
					.GetResult();

				Assert.IsTrue(HttpStatusCode.OK == response.StatusCode, "StatusCode should be OK");
				task = response.Content.ReadAsAsync<ProcessingTask>().GetAwaiter().GetResult();

				Assert.IsTrue(TaskStatus.Success == task.Status, "Task should be succeeded");

				response = server.CreateRequest("/api/ProcessingResult?taskId=" + taskId)
					.GetAsync()
					.GetAwaiter()
					.GetResult();

				Assert.IsTrue(HttpStatusCode.OK == response.StatusCode, "StatusCode should be OK");
			}
		}
	}
}
