using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Alemow.Autofac
{
    public class SingleResolver<T> : ISingleResolver<T>
    {
        private readonly ILifetimeScope _scope;

        public SingleResolver(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public T Get()
        {
            return _scope.Resolve<IEnumerable<T>>().Single();
        }

        public T GetOptional()
        {
            return _scope.Resolve<IEnumerable<T>>().SingleOrDefault();
        }
    }

    public interface ISingleResolver<out T>
    {
        T Get();

        T GetOptional();
    }
}