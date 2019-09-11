using System;
using System.Linq;
using System.Reflection;
using Alemow.Attributes;
using Alemow.Miscs;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Microsoft.Extensions.Configuration;

namespace Alemow.Autofac
{
    internal class ConfigurationRegisterer : IRegisterer
    {
        private readonly ContainerBuilder _containerBuilder;

        public ConfigurationRegisterer(ContainerBuilder containerBuilder)
        {
            _containerBuilder = containerBuilder;
        }

        public void Register(Type type)
        {
            var con = type.GetCustomAttribute<ConfigurationAttribute>();
            if (con == null)
            {
                return;
            }

            if (typeof(IModule).IsAssignableFrom(type))
            {
                _containerBuilder.RegisterModule((IModule) Activator.CreateInstance(type));
            }

            foreach (var method in type.GetRuntimeMethods())
            {
                RegisterMethod(method, Activator.CreateInstance(type));
            }
        }

        private void RegisterMethod(MethodInfo method, object instance)
        {
            if (typeof(object).Equals(method.DeclaringType))
            {
                return;
            }

            var bean = method.GetCustomAttribute<BeanAttribute>();
            if (bean == null)
            {
                return;
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

            var parameters = method.GetParameters();
            _containerBuilder.Register(c =>
                {
                    return method.Invoke(instance, parameters.Select(it => ResolveParameter(c, it)).ToArray());
                })
                .ApplyDefault()
                .ApplyAsSelf(bean, returnType)
                .ApplyAsImplementedInterfaces(bean, returnType)
                .ApplyAs(method.GetCustomAttributes<AsAttribute>())
                .ApplyScope(method.GetCustomAttribute<ScopeAttribute>())
                .ApplyKeys(method.GetCustomAttributes<KeyAttribute>())
                .ApplyMetadata(method.GetCustomAttributes<MetadataAttribute>());
        }

        private object ResolveParameter(IComponentContext context, ParameterInfo parameter)
        {
            var valueAttr = parameter.GetCustomAttribute<ConfigValueAttribute>();
            if (valueAttr != null)
            {
                var valuePath = valueAttr.Path;
                var config = context.Resolve<IConfiguration>();
                var valueType = parameter.ParameterType;
                var value = config.GetSection(valuePath).Get(valueType);
                return value;
            }

            var inject = parameter.GetCustomAttribute<InjectAttribute>();
            if (inject != null)
            {
                var key = inject.Key;
                var valueType = parameter.ParameterType;
                var value = key != null ? context.ResolveKeyed(key, valueType) : context.Resolve(valueType);
                return value;
            }

            if (parameter.ParameterType == typeof(IComponentContext))
            {
                return context;
            }

            return context.Resolve(parameter.ParameterType);
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
                builder = builder.As(RegistrationBuilderExtensions.GetImplementedInterfaces(type));
            }

            return builder;
        }
    }
}