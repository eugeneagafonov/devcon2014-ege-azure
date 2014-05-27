using System;

using Autofac;

namespace AzureService.Component
{
	internal class AutofacLifetimeAwareComponent<T> : ILifetimeAwareComponent<T>
	{
		public AutofacLifetimeAwareComponent(IContainer container)
		{
			var scope = container.BeginLifetimeScope();
			LifetimeScope = scope;
			Service = scope.Resolve<T>();
		}

		public IDisposable LifetimeScope { get; private set; }

		public T Service { get; private set; }
	}
}