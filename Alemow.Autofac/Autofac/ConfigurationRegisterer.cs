using System;
using System.Linq;
using System.Reflection;
using Alemow.Attributes;
using Alemow.Autofac.Resolvers;
using Alemow.Miscs;
using Autofac;
using Autofac.Builder;
using Autofac.Core;

namespace Alemow.Autofac
{
    internal class ConfigurationRegisterer : IRegisterer
    {
        private readonly ContainerBuilder _containerBuilder;
        private readonly AutoRegisterOptions _options;

        private readonly IObjectFactory _instanceFactory;
        private readonly IProfileMatcher _profileMatcher;
        private readonly ParameterInfoResolver _parameterInfoResolver;

        public ConfigurationRegisterer(ContainerBuilder containerBuilder, AutoRegisterOptions options)
        {
            _containerBuilder = containerBuilder;
            _options = options;

            _instanceFactory = options.ObjectFactory;
            _profileMatcher = new ProfileMatcher(options.Profiles);
            _parameterInfoResolver = new ParameterInfoResolver(new ConfigValueResolver(options.ConfigResolver), new InjectResolver());
        }

        public void Register(Type type)
        {
            var con = type.GetCustomAttribute<ConfigurationAttribute>();
            if (con == null)
            {
                return;
            }

            if (con.Profile != null)
            {
                if (!_profileMatcher.Matches(con.Profile))
                {
                    return;
                }
            }

            var instance = _instanceFactory.CreateFor(type.GetTypeInfo());
            if (instance is IModule module)
            {
                _containerBuilder.RegisterModule(module);
            }

            foreach (var method in type.GetRuntimeMethods().Where(it => !it.IsStatic))
            {
                RegisterMethod(method, instance);
            }
        }

        private void RegisterMethod(MethodInfo method, object instance)
        {
            if (typeof(object) == method.DeclaringType)
            {
                return;
            }

            RegisterInitMethod(method, instance);
            RegisterBeanMethod(method, instance);
            RegisterDestroyMethod(method, instance);
        }

        // orderAttribute?
        private void RegisterBeanMethod(MethodInfo method, object instance)
        {
            var bean = method.GetCustomAttribute<BeanAttribute>();
            if (bean == null)
            {
                return;
            }

            if (bean.Profile != null)
            {
                if (!_profileMatcher.Matches(bean.Profile))
                {
                    return;
                }
            }

            var returnType = method.ReturnType;
            if (returnType == typeof(void))
            {
                throw Assertion.Fail($"{nameof(BeanAttribute)} annotated method should return value");
            }

            if (returnType.IsGenericType)
            {
                throw Assertion.Fail($"{nameof(BeanAttribute)} annotated method should return non-generic type");
            }

            _containerBuilder.Register(c => InvokeMethod(c, instance, method))
                .ApplyDefault()
                .ApplyAsSelf(bean, returnType)
                .ApplyAsImplementedInterfaces(bean, returnType)
                .ApplyAs(method.GetCustomAttributes<AsAttribute>())
                .ApplyScope(method.GetCustomAttribute<ScopeAttribute>())
                .ApplyKeys(method.GetCustomAttributes<KeyAttribute>())
                .ApplyMetadata(method.GetCustomAttributes<MetadataAttribute>());
        }

        private void RegisterInitMethod(MethodInfo method, object instance)
        {
            var init = method.GetCustomAttribute<InitAttribute>();
            if (init == null)
            {
                return;
            }

            var returnType = method.ReturnType;
            if (returnType != typeof(void))
            {
                throw Assertion.Fail($"{nameof(InitAttribute)} annotated method should not return value");
            }

            _containerBuilder.RegisterBuildCallback(c =>
            {
                InvokeMethod(c, instance, method);
            });
        }

        private void RegisterDestroyMethod(MethodInfo method, object instance)
        {
            var destroy = method.GetCustomAttribute<DestroyAttribute>();
            if (destroy == null)
            {
                return;
            }

            var returnType = method.ReturnType;
            if (returnType != typeof(void))
            {
                throw Assertion.Fail($"{nameof(DestroyAttribute)} annotated method should not return value");
            }

            _containerBuilder.RegisterBuildCallback(c =>
            {
                var parameters = ResolveParameters(c, method);
                c.Disposer.AddInstanceForDisposal(new ActionDisposer(() =>
                {
                    InvokeMethod(c, instance, method, parameters);
                }));
            });
        }

        private object InvokeMethod(IComponentContext c, object instance, MethodInfo method)
        {
            return method.Invoke(instance, ResolveParameters(c, method));
        }

        private object InvokeMethod(IComponentContext c, object instance, MethodInfo method, object[] parameters)
        {
            return method.Invoke(instance, parameters);
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

    internal static partial class RegistrationBuilderExtensions
    {
        internal static IRegistrationBuilder<TLimit, SimpleActivatorData, SingleRegistrationStyle>
            ApplyDefault<TLimit>(
                this IRegistrationBuilder<TLimit, SimpleActivatorData, SingleRegistrationStyle> builder)
        {
            return builder;
        }

        internal static IRegistrationBuilder<TLimit, SimpleActivatorData, SingleRegistrationStyle>
            ApplyAsSelf<TLimit>(
                this IRegistrationBuilder<TLimit, SimpleActivatorData, SingleRegistrationStyle> builder,
                BeanAttribute attr, Type type)
        {
            if (attr.AsSelf)
            {
                builder = builder.As(type);
            }

            return builder;
        }

        internal static IRegistrationBuilder<TLimit, SimpleActivatorData, SingleRegistrationStyle>
            ApplyAsImplementedInterfaces<TLimit>(
                this IRegistrationBuilder<TLimit, SimpleActivatorData, SingleRegistrationStyle> builder,
                BeanAttribute attr, Type type)
        {
            if (attr.AsImplementedInterfaces)
            {
                builder = builder.As(GetImplementedInterfaces(type));
            }

            return builder;
        }
    }
}