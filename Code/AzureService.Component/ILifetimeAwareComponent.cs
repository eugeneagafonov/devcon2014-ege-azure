using System;

namespace AzureService.Component
{
	public interface ILifetimeAwareComponent<out T>
	{
		IDisposable LifetimeScope { get; }

		T Service { get; }
	}
}