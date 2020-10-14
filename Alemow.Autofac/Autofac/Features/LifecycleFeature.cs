using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Alemow.Attributes;
using Alemow.Autofac.Resolvers;
using Alemow.Config;
using Alemow.Miscs;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;

namespace Alemow.Autofac.Features
{
    internal class LifecycleFeature : IComponentRegistrationFeature
    {
        private readonly ParameterInfoResolver _parameterInfoResolver;

        private readonly ConcurrentDictionary<TypeInfo, IEnumerable<(MethodInfo MethodInfo, InitAttribute InitAttribute, DestroyAttribute DestroyAttribute)>> _typeMethods =
            new ConcurrentDictionary<TypeInfo, IEnumerable<(MethodInfo, InitAttribute, DestroyAttribute)>>();

        public LifecycleFeature(IConfigResolver configResolver)
        {
            _parameterInfoResolver = new ParameterInfoResolver(new ConfigValueResolver(configResolver), new InjectResolver());
        }

        public void Configure(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            if (registration.Activator is ReflectionActivator)
            {
                RegisterReflection(registration);
            }
            else //
            {
                Register(registration);
            }
        }

        private void RegisterReflection(IComponentRegistration registration)
        {
            var actions = Enumerables.List<Action<object, ActivatedEventArgs<object>>>();

            var type = registration.Activator.LimitType;
            var methods = type.GetTypeInfo().GetAllMethods()
                .Select(it => (MethodInfo: it, InitAttribute: it.GetCustomAttribute<InitAttribute>(), DestroyAttribute: it.GetCustomAttribute<DestroyAttribute>()))
                .Where(it => it.InitAttribute != null || it.DestroyAttribute != null)
                .ToList();
            foreach (var (methodInfo, initAttribute, destroyAttribute) in methods)
            {
                if (initAttribute != null)
                {
                    actions.Add(InitAction(registration, methodInfo, initAttribute));
                }

                if (destroyAttribute != null)
                {
                    actions.Add(DestroyAction(registration, methodInfo, destroyAttribute));
                }
            }

            if (actions.Any())
            {
                registration.Activated += (sender, args) =>
                {
                    foreach (var action in actions)
                    {
                        action.Invoke(sender, args);
                    }
                };
            }
        }

        private void Register(IComponentRegistration registration)
        {
            registration.Activated += (sender, args) =>
            {
                var actions = Enumerables.List<Action<object, ActivatedEventArgs<object>>>();

                var type = args.Instance.GetType();
                var methods = _typeMethods.GetOrAdd(type.GetTypeInfo(), ti => ti
                    .GetAllMethods()
                    .Select(it => (
                        MethodInfo: it,
                        InitAttribute: it.GetCustomAttribute<InitAttribute>(),
                        DestroyAttribute: it.GetCustomAttribute<DestroyAttribute>())
                    ).Where(it => it.InitAttribute != null || it.DestroyAttribute != null)
                    .ToList());
                foreach (var (methodInfo, initAttribute, destroyAttribute) in methods)
                {
                    if (initAttribute != null)
                    {
                        actions.Add(InitAction(registration, methodInfo, initAttribute));
                    }

                    if (destroyAttribute != null)
                    {
                        actions.Add(DestroyAction(registration, methodInfo, destroyAttribute));
                    }
                }

                foreach (var action in actions)
                {
                    action.Invoke(sender, args);
                }
            };
        }

        private Action<object, ActivatedEventArgs<object>> InitAction(IComponentRegistration registration, MethodInfo methodInfo, InitAttribute initAttribute)
        {
            var returnType = methodInfo.ReturnType;
            if (returnType != typeof(void))
            {
                throw Assertion.Fail($"{nameof(InitAttribute)} annotated method should not return value");
            }

            return (sender, args) =>
            {
                var parameters = ResolveParameters(args.Context, methodInfo);
                methodInfo.Invoke(args.Instance, parameters);
            };
        }

        private Action<object, ActivatedEventArgs<object>> DestroyAction(IComponentRegistration registration, MethodInfo methodInfo, DestroyAttribute destroyAttribute)
        {
            var returnType = methodInfo.ReturnType;
            if (returnType != typeof(void))
            {
                throw Assertion.Fail($"{nameof(DestroyAttribute)} annotated method should not return value");
            }

            return (sender, args) =>
            {
                var parameters = ResolveParameters(args.Context, methodInfo);
                var context = args.Context.Resolve<ILifetimeScope>();
                context.Disposer.AddInstanceForDisposal(new ActionDisposer(() =>
                {
                    methodInfo.Invoke(args.Instance, parameters);
                }));
            };
        }

        private object[] ResolveParameters(IComponentContext context, MethodInfo method)
        {
            return method.GetParameters().Select(it => ResolveParameter(context, it)).ToArray();
        }

        private object ResolveParameter(IComponentContext context, ParameterInfo parameter)
        {
            var (success, resolved) = _parameterInfoResolver.Resolve(context, new ParameterInfoResolverParam
            {
                ParameterInfo = parameter,
            });
            Assertion.IsTrue(success, $"failed to resolve parameter type {parameter.ParameterType}");
            return resolved;
        }
    }
}