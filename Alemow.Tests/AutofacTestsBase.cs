using System;
using Autofac;

namespace Alemow
{
    public abstract class AutofacTestsBase : IDisposable
    {
        protected IContainer Container { get; }

        public AutofacTestsBase(Action<ContainerBuilder> configurer)
        {
            var containerBuilder = new ContainerBuilder();
            configurer.Invoke(containerBuilder);
            Container = containerBuilder.Build();
        }

        public void Dispose()
        {
            Container.Dispose();
        }
    }
}