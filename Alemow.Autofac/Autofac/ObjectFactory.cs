using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Alemow.Miscs;

namespace Alemow.Autofac
{
    public interface IObjectFactory
    {
        void Register(TypeInfo type, object instance);

        object Resolve(TypeInfo type);

        object CreateFor(TypeInfo type);
    }

    public class SimpleObjectFactory : IObjectFactory
    {
        private readonly IDictionary<TypeInfo, object> _map = new Dictionary<TypeInfo, object>();

        public void Register(TypeInfo type, object instance)
        {
            Assertion.NotNull(instance, $"{nameof(instance)} should not be null");
            Assertion.IsTrue(type.IsInstanceOfType(instance), $"{nameof(instance)} should be assignable to {type.FullName}");
            _map[type] = instance;
        }

        public object Resolve(TypeInfo type)
        {
            Assertion.IsTrue(_map.TryGetValue(type, out var instance), $"type={type.FullName} not registered");
            return instance;
        }

        public object CreateFor(TypeInfo type)
        {
            var ctor = FindBestMatchConstructorFor(type);
            return ctor.Invoke(ctor.GetParameters()
                .Select(p => Resolve(p.ParameterType.GetTypeInfo()))
                .ToArray());
        }

        private ConstructorInfo FindBestMatchConstructorFor(TypeInfo type)
        {
            return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .OrderByDescending(it => it.GetParameters().Length)
                .FirstOrDefault(it => it.GetParameters().IsNullOrEmpty() ||
                                      it.GetParameters().All(p => _map.ContainsKey(p.ParameterType.GetTypeInfo())));
        }
    }

    public static class ObjectFactoryExtensions
    {
        public static void Register<T>(this IObjectFactory objectFactory, T instance)
        {
            objectFactory.Register(typeof(T).GetTypeInfo(), instance);
        }

        public static T Resolve<T>(this IObjectFactory objectFactory)
        {
            return (T)objectFactory.Resolve(typeof(T).GetTypeInfo());
        }

        public static T CreateFor<T>(this IObjectFactory objectFactory)
        {
            return (T)objectFactory.CreateFor(typeof(T).GetTypeInfo());
        }
    }
}