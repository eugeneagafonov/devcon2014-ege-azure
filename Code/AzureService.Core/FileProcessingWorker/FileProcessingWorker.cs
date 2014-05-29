using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using AzureService.Core.Entity;
using AzureService.Core.FileProcessing;
using AzureService.Core.FileStorageService;
using AzureService.Core.QueueService;
using AzureService.Core.TaskService;
using Newtonsoft.Json;
using TaskStatus = AzureService.Core.Entity.TaskStatus;

namespace AzureService.Core.FileProcessingWorker
{
	public class FileProcessingWorker : IFileProcessingWorker
	{
		private readonly ITaskService _taskService;
		private readonly IQueueService _queueService;
		private readonly IFileStorageService _fileStorageService;
		private readonly IIndex<string, IFileProcessor> _fileProcessors;
		public FileProcessingWorker(ITaskService taskService, IQueueService queueService
			, IFileStorageService fileStorageService, IIndex<string, IFileProcessor> fileProcessors)
		{
			if(null == taskService) throw new ArgumentNullException("taskService");
			if(null == queueService) throw new ArgumentNullException("queueService");
			if(null == fileStorageService) throw new ArgumentNullException("fileStorageService");
			if(null == fileProcessors) throw new ArgumentNullException("fileProcessors");

			_taskService = taskService;
			_queueService = queueService;
			_fileStorageService = fileStorageService;
			_fileProcessors = fileProcessors;

		}

		public async Task DoWorkAsync()
		{
			ProcessingTaskQueueMessage msg = await _queueService.DequeueConversionTaskAsync();
			if (null == msg) return;

			ProcessingTask task = await _taskService.GetTaskAsync(msg.Task.Id);
			if (null == task || null == task.SourceFile || string.IsNullOrWhiteSpace(task.SourceFile.BlobFileName)) return;

			IFileProcessor fileProcessor = _fileProcessors[task.Operation];
			if (null == fileProcessor) return;

			Trace.TraceInformation("Обрабатываем задание {0}", JsonConvert.SerializeObject(msg.Task));

			var sourceStream = await _fileStorageService.GetSourceFileStreamAsync(task.SourceFile.BlobFileName);
			var destinationStream = new MemoryStream((int)sourceStream.Length); // плохо, но не хочется делать файлы
			await fileProcessor.ProcessFileAsync(sourceStream, destinationStream);
			destinationStream.Seek(0, SeekOrigin.Begin);
			var destinationFile = await _fileStorageService.SaveDestinationFileStreamAsync(destinationStream, task.SourceFile.MimeContentType);
			destinationStream.Close();

			task.DestinationFile = destinationFile;
			task.Status = TaskStatus.Success;
			task.Message = "Task Processed Succesfully";

			await _taskService.SaveTaskAsync(task);

			await _queueService.DeleteConversionTaskAsync(msg.Id, msg.PopReceipt);



			Trace.TraceInformation("Задание  обработано успешно");
		}
	}
}
