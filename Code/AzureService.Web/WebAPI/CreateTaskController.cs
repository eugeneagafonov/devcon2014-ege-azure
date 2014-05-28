using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AzureService.Core.Entity;
using AzureService.Core.Extension;
using AzureService.Core.FileStorageService;
using AzureService.Core.QueueService;
using AzureService.Core.TaskService;
using TaskStatus = AzureService.Core.Entity.TaskStatus;

namespace AzureService.Web.WebAPI
{
	public class CreateTaskController : ApiController
	{
		private readonly IFileStorageService _fileStorageService;
		private readonly IQueueService _queueService;
		private readonly ITaskService _taskService;

		public CreateTaskController(IFileStorageService fileStorageService,
			ITaskService taskService, IQueueService queueService)
		{
			_fileStorageService = fileStorageService;
			_taskService = taskService;
			_queueService = queueService;
		}

		public async Task<HttpResponseMessage> Post()
		{
			if (!Request.Content.IsMimeMultipartContent())
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}

			MultipartMemoryStreamProvider bodyParts = await Request.Content.ReadAsMultipartAsync();
			ProcessingTask task = null;

			foreach (HttpContent part in bodyParts.Contents)
			{
				// загружаем только один файл
				if (null == part.Headers.ContentType) continue;

				string contentType = part.Headers.ContentType.MediaType;
				string fileName = part.Headers.ContentDisposition.FileName;
				fileName = fileName.Trim(new[] {' ', '\t', '\"', '\''});
				FileMetadata fileMetadata = null;
				using (Stream requestStream = await part.ReadAsStreamAsync())
				{
					fileMetadata = await _fileStorageService.SaveSourceFileStreamAsync(requestStream, contentType);
				}

				task = new ProcessingTask
				{
					Status = TaskStatus.Created,
					UserId = "eugene.agafonov@devcon2014.ru",
					Id = Guid.NewGuid().ToShortString(),
					OriginalFileName = fileName,
					SourceFile = fileMetadata
				};

				// создали задачу
				await _taskService.SaveTaskAsync(task);

				// поставили ссылку в очередь
				ProcessingTaskQueueMessage msg = await _queueService.EnqueueConversionTaskAsync(task);

				task.Status = TaskStatus.Enqueued;

				// изменили статус задачи
				await _taskService.SaveTaskAsync(task);

				break;
			}

			return Request.CreateResponse(HttpStatusCode.OK, task);
		}
	}
}