using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AzureService.Core.TaskService;
using AzureService.Web.Log;
using TaskStatus = AzureService.Core.Entity.TaskStatus;

namespace AzureService.Web.WebAPI
{
	public class ProcessingTaskStatusController : ApiController
	{
		private readonly ITaskService _taskService;
		public ProcessingTaskStatusController(ITaskService taskService)
		{
			if(null == taskService) throw new ArgumentNullException("taskService");

			_taskService = taskService;
		}
		public async Task<HttpResponseMessage> Get(string taskId)
		{
			Trace.TraceInformation(this.CreateLogEntry(string.Format("Checkig task id {0} status", taskId)));

			var task = await _taskService.GetTaskAsync(taskId);
			if (null == task)
			{
				return Request.CreateResponse(HttpStatusCode.NotFound);
			}

			if(TaskStatus.Error == task.Status)
			{
				return Request.CreateResponse(HttpStatusCode.OK,
				new
				{
					TaskId = taskId,
					Status = task.Status,
					Message = task.Message
				});
			}

			if (TaskStatus.Created == task.Status || TaskStatus.Enqueued == task.Status)
			{
				return Request.CreateResponse(HttpStatusCode.OK,
					new
					{
						TaskId = taskId,
						Status = task.Status,
					});
			}

			return Request.CreateResponse(HttpStatusCode.OK, task);
		}
	}
}
