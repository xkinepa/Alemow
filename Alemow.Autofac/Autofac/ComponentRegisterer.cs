using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Alemow.Attributes;
using Autofac;
using Autofac.Builder;
using Autofac.Features.AttributeFilters;

namespace Alemow.Autofac
{
    internal class ComponentRegisterer : IRegisterer
    {
        private readonly ContainerBuilder _containerBuilder;
        private readonly IList<IAutoRegisterFeature> _features;

        public ComponentRegisterer(ContainerBuilder containerBuilder, IList<IAutoRegisterFeature> features)
        {
            _containerBuilder = containerBuilder;
            _features = features;
        }

        public void Register(Type type)
        {
            var com = type.GetCustomAttribute<ComponentAttribute>();
            if (com == null)
            {
                return;
            }

            var isGeneric = type.IsGenericType;
            if (isGeneric)
            {
                _containerBuilder.RegisterGeneric(type)
                    .ApplyDefault()
                    .ApplyAsSelf(com)
                    .ApplyAsImplementedInterfaces(com)
                    .ApplyAs(type.GetCustomAttributes<AsAttribute>())
                    .ApplyScope(type.GetCustomAttribute<ScopeAttribute>())
                    .ApplyKeys(type.GetCustomAttributes<KeyAttribute>())
                    .ApplyMetadata(type.GetCustomAttributes<MetadataAttribute>())
                    .ApplyFeatures(type, _features)
                    ;
                return;
            }

            _containerBuilder.RegisterType(type)
                .ApplyDefault()
                .ApplyAsSelf(com)
                .ApplyAsImplementedInterfaces(com)
                .ApplyAs(type.GetCustomAttributes<AsAttribute>())
                .ApplyScope(type.GetCustomAttribute<ScopeAttribute>())
                .ApplyKeys(type.GetCustomAttributes<KeyAttribute>())
                .ApplyMetadata(type.GetCustomAttributes<MetadataAttribute>())
                .ApplyFeatures(type, _features)
                ;
        }
    }

    internal static partial class RegistrationBuilderExtensions
    {
        internal static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>
            ApplyDefault<TLimit, TActivatorData, TRegistrationStyle>(
                this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder)
            where TActivatorData : ReflectionActivatorData
        {
            return builder.WithAttributeFiltering();
        }

        internal static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>
            ApplyFeatures<TLimit, TActivatorData, TRegistrationStyle>(
                this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder, Type type, IList<IAutoRegisterFeature> features
            )
        {
            foreach (var feature in features)
            {
                feature.Apply(builder, type);
            }

            return builder;
        }

        private static Type[] GetImplementedInterfaces(Type type)
        {
            var interfaces = type.GetTypeInfo().ImplementedInterfaces.Where(i => i != typeof(IDisposable));
            return type.GetTypeInfo().IsInterface ? interfaces.AppendItem(type).ToArray() : interfaces.ToArray();
        }

        private static IEnumerable<T> AppendItem<T>(this IEnumerable<T> sequence, T trailingItem)
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));

            foreach (var t in sequence)
                yield return t;

            yield return trailingItem;
        }
    }

    internal static partial class RegistrationBuilderExtensions
    {
        internal static IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            ApplyAsSelf(
                this IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> builder,
                ComponentAttribute attr)
        {
            if (attr.AsSelf)
            {
                builder = builder.AsSelf();
            }

            return builder;
        }

        internal static IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            ApplyAsImplementedInterfaces(
                this IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> builder,
                ComponentAttribute attr)
        {
            if (attr.AsImplementedInterfaces)
            {
                builder = builder.AsImplementedInterfaces();
            }

            return builder;
        }
    }

    internal static partial class RegistrationBuilderExtensions
    {
        internal static IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle>
            ApplyAsSelf(
                this IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle> builder,
                ComponentAttribute attr)
        {
            if (attr.AsSelf)
            {
                builder = builder.AsSelf();
            }

            return builder;
        }

        internal static IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle>
            ApplyAsImplementedInterfaces(
                this IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle> builder,
                ComponentAttribute attr)
        {
            if (attr.AsImplementedInterfaces)
            {
                builder = builder.AsImplementedInterfaces();
            }

            return builder;
        }
    }
}