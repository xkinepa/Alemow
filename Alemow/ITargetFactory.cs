using System;

namespace Alemow
{
    public interface ITargetFactory
    {
        object Instance { get; }
    }

    public class InstanceTargetFactory : ITargetFactory
    {
        public InstanceTargetFactory(object instance)
        {
            Instance = instance;
        }

        public object Instance { get; }
    }

    public class FactoryTargetFactory : ITargetFactory
    {
        private readonly Func<object> _factory;

        public FactoryTargetFactory(Func<object> factory)
        {
            _factory = factory;
        }

        public object Instance => _factory.Invoke();
    }
}