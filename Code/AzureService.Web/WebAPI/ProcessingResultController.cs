using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AzureService.Core.FileStorageService;
using AzureService.Core.TaskService;
using TaskStatus = AzureService.Core.Entity.TaskStatus;

namespace AzureService.Web.WebAPI
{
	public class ProcessingResultController : ApiController
	{
		private readonly IFileStorageService _fileStorageService;
		private readonly ITaskService _taskService;

		public ProcessingResultController(IFileStorageService fileStorageService, ITaskService taskService)
		{
			if(null == fileStorageService) throw new ArgumentNullException("fileStorageService");
			if(null == taskService) throw new ArgumentNullException("taskService");

			_fileStorageService = fileStorageService;
			_taskService = taskService;
		}

		public async Task<HttpResponseMessage> Get(string taskId)
		{
			var msg = Request.CreateResponse(HttpStatusCode.OK);

			var task = await _taskService.GetTaskAsync(taskId);

			if (null == task || task.Status != TaskStatus.Success 
				|| task.DestinationFile == null || string.IsNullOrWhiteSpace(task.DestinationFile.BlobFileName))
				return Request.CreateResponse(HttpStatusCode.NotFound);

			var fileStream = await _fileStorageService.GetDestinationFileStreamAsync(task.DestinationFile.BlobFileName);

			if (null == fileStream) return Request.CreateResponse(HttpStatusCode.NotFound);
			var pushStream = new PushStreamContent((stream, content, context) =>
			{
				using (fileStream)
				using (stream)
				{
					fileStream.CopyTo(stream);
				}
			});
			msg.Content = pushStream;
			return msg;
		}
	}
}