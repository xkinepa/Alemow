using System;
using System.Runtime.CompilerServices;
using Alemow.Attributes;
using Alemow.Miscs;
using Autofac;

namespace Alemow.Autofac
{
    public class LifetimeScopeAttacher : ILifetimeScopeAttacher
    {
        private static readonly Lazy<LifetimeScopeAttacher> InstanceHolder =
            new Lazy<LifetimeScopeAttacher>(() => new LifetimeScopeAttacher());

        public static LifetimeScopeAttacher Instance => InstanceHolder.Value;

        private readonly ConditionalWeakTable<object, ILifetimeScope> _contextMap = new ConditionalWeakTable<object, ILifetimeScope>();

        public void Attach(object instance, ILifetimeScope scope)
        {
            if (_contextMap.TryGetValue(instance, out var attached))
            {
                Assertion.IsTrue(attached == scope, $"multiple instance with diff scope not allowed attaching");
                return;
            }

            _contextMap.Add(instance, scope);
        }

        public void Detach(object instance)
        {
            _contextMap.Remove(instance);
        }

        public ILifetimeScope GetLifetimeScope(object instance, bool required = true)
        {
            var result = _contextMap.TryGetValue(instance, out var scope);
            if (required)
            {
                Assertion.IsTrue(result, $"scope not found, perhaps component not registered by [{nameof(ComponentAttribute)}]");
            }

            return scope;
        }
    }

    public interface ILifetimeScopeAttacher
    {
        void Attach(object instance, ILifetimeScope scope);
        void Detach(object instance);
        ILifetimeScope GetLifetimeScope(object instance, bool required = true);
    }
}