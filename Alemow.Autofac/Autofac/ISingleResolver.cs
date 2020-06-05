using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Alemow.Miscs;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;

namespace Alemow.Autofac
{
    public interface ISingleResolver<out T>
    {
        T Get();

        T GetOptional();
    }

    internal class SingleResolver<T> : ISingleResolver<T>
    {
        private readonly bool _exists;
        private readonly T _instance;

        public SingleResolver(bool exists, T instance)
        {
            _exists = exists;
            _instance = instance;
        }

        public T Get()
        {
            Assertion.IsTrue(_exists, $"instance of type={typeof(T).FullName} not found");
            return _instance;
        }

        public T GetOptional()
        {
            return _exists ? _instance : default(T);
        }
    }

    internal class SingleResolverRegistrationSource : IRegistrationSource
    {
        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            if (!(service is IServiceWithType swt) || !IsGenericSingleType(swt.ServiceType))
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            var serviceType = swt.ServiceType;
            var elementType = serviceType.GetTypeInfo().GenericTypeArguments.First();
            var singleResolverType = typeof(SingleResolver<>).MakeGenericType(elementType);
            var elementTypeService = swt.ChangeType(elementType);

            var activator = new DelegateActivator(
                serviceType,
                (c, p) =>
                {
                    var elements = c.ComponentRegistry.RegistrationsFor(elementTypeService).ToList();
                    if (elements.IsNullOrEmpty())
                    {
                        return Activator.CreateInstance(singleResolverType, false, null);
                    }

                    var item = elements.Select(cr => c.ResolveComponent(cr, p)).Single();
                    return Activator.CreateInstance(singleResolverType, true, item);
                });

            var registration = new ComponentRegistration(
                Guid.NewGuid(),
                activator,
                new CurrentScopeLifetime(),
                InstanceSharing.None,
                InstanceOwnership.ExternallyOwned,
                new[] { service },
                new Dictionary<string, object>());

            return new IComponentRegistration[] { registration };
        }

        public bool IsAdapterForIndividualComponents => false;

        private bool IsGenericSingleType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ISingleResolver<>);
        }
    }
}