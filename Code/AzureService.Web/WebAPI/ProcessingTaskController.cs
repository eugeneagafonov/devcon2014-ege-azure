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
	public class ProcessingTaskController : ApiController
	{
		private readonly IFileStorageService _fileStorageService;
		private readonly IQueueService _queueService;
		private readonly ITaskService _taskService;

		public ProcessingTaskController(IFileStorageService fileStorageService,
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
			string operation = null;
			HttpContent filePart = null;

			MultipartMemoryStreamProvider bodyParts = await Request.Content.ReadAsMultipartAsync();
			ProcessingTask task = null;

			foreach (HttpContent part in bodyParts.Contents)
			{
				var contentDisposition = part.Headers.ContentDisposition;
				if (contentDisposition == null || string.IsNullOrWhiteSpace(contentDisposition.Name))
				{
					continue;
				}

				string partName = contentDisposition.Name.Trim(new[] { ' ', '\t', '\"', '\'' });
				if (string.Equals(partName, "operation", StringComparison.OrdinalIgnoreCase))
				{
					operation = await part.ReadAsStringAsync();
				}
				else if (null != part.Headers.ContentType
					&& string.Equals(partName, "file", StringComparison.OrdinalIgnoreCase))
				{
					filePart = part;
				}

				if (!string.IsNullOrWhiteSpace(operation) && null != filePart)
				{
					break;
				}
			}

			if (string.IsNullOrWhiteSpace(operation) || null == filePart)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}

			string contentType = filePart.Headers.ContentType.MediaType;
			string fileName = filePart.Headers.ContentDisposition.FileName;
			fileName = fileName.Trim(new[] {' ', '\t', '\"', '\''});
			FileMetadata fileMetadata = null;
			using (Stream requestStream = await filePart.ReadAsStreamAsync())
			{
				fileMetadata = await _fileStorageService.SaveSourceFileStreamAsync(requestStream, contentType);
			}

			task = new ProcessingTask
			{
				Status = TaskStatus.Created,
				UserId = "eugene.agafonov@devcon2014.ru", // аутентификация - не в этом демо
				Id = Guid.NewGuid().ToShortString(),
				OriginalFileName = fileName,
				Operation = operation,
				Message = "Created processing task",
				SourceFile = fileMetadata
			};

			// создали задачу
			await _taskService.SaveTaskAsync(task);

			// поставили ссылку в очередь
			ProcessingTaskQueueMessage msg = await _queueService.EnqueueConversionTaskAsync(task);

			task.Status = TaskStatus.Enqueued;
			task.Message = "Enqueued task for processing";

			// изменили статус задачи
			await _taskService.SaveTaskAsync(task);

			return Request.CreateResponse(HttpStatusCode.OK, task);
		}
	}
}