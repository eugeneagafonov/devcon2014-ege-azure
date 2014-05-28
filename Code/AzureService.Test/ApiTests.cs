﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using AzureService.Test.MockObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
				Assert.Inconclusive(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
			}
		}
	}
}
