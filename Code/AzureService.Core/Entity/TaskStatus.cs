using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureService.Core.Entity
{
	public enum TaskStatus
	{
		Created,

		Enqueued,

		Processing,

		Success,

		Error
	}
}
