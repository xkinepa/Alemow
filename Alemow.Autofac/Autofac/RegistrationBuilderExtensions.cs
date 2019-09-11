using System.Collections.Generic;
using Alemow.Attributes;
using Alemow.Miscs;
using Autofac;
using Autofac.Builder;

namespace Alemow.Autofac
{
    internal static partial class RegistrationBuilderExtensions
    {
        internal static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>
            ApplyAs<TLimit, TActivatorData, TRegistrationStyle>(
                this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder, IEnumerable<AsAttribute> attrs)
        {
            foreach (var attr in attrs)
            {
                builder = builder.As(attr.Type);
            }

            return builder;
        }

        internal static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>
            ApplyScope<TLimit, TActivatorData, TRegistrationStyle>(
                this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder, ScopeAttribute attr)
        {
            var scope = attr?.Scope ?? default(Scope);
            switch (scope)
            {
                case Scope.PerDependency:
                    return builder.InstancePerDependency();
                case Scope.Singleton:
                    return builder.SingleInstance();
                case Scope.PerLifetimeScope:
                    return builder.InstancePerLifetimeScope();
                case Scope.PerRequest:
                    return builder.InstancePerRequest();
                default:
                    throw Assertion.Fail();
            }
        }

        internal static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>
            ApplyKeys<TLimit, TActivatorData, TRegistrationStyle>(
                this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder, IEnumerable<KeyAttribute> attrs
            )
        {
            foreach (var attr in attrs)
            {
                builder = builder.Keyed(attr.Key, attr.As);
            }

            return builder;
        }

        internal static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>
            ApplyMetadata<TLimit, TActivatorData, TRegistrationStyle>(
                this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder, IEnumerable<MetadataAttribute> attrs
            )
        {
            foreach (var attr in attrs)
            {
                builder = builder.WithMetadata(attr.Key, attr.Value);
            }

            return builder;
        }
    }
}