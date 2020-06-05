using System;
using System.Linq;
using System.Reflection;
using Alemow.Attributes;
using Alemow.Autofac.Resolvers;
using Alemow.Config;
using Alemow.Miscs;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;

namespace Alemow.Autofac.Features
{
    internal class ConfigValueFeature : IComponentRegistrationFeature
    {
        private readonly ConfigValueResolver _resolver;

        public ConfigValueFeature(IConfigResolver configResolver)
        {
            _resolver = new ConfigValueResolver(configResolver);
        }

        public void Configure(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            if (registration.Activator is ReflectionActivator)
            {
                ProcessCtorParam(registration);
                ProcessField(registration);
            }
        }

        private void ProcessCtorParam(IComponentRegistration registration)
        {
            var type = registration.Activator.LimitType;
            var @params = type
                .GetConstructors()
                .SelectMany(it => it.GetParameters())
                .Select(it => (ParameterInfo: it, ConfigValueAttribute: it.GetCustomAttribute<ConfigValueAttribute>()))
                .Where(it => it.ConfigValueAttribute != null)
                .ToList();
            if (@params.IsNullOrEmpty())
            {
                return;
            }

            registration.Preparing += (sender, args) =>
            {
                args.Parameters = args.Parameters.Concat(@params.Select(it => new ResolvedParameter(
                    (info, context) => info == it.ParameterInfo,
                    (info, context) =>
                    {
                        var (success, value) = _resolver.Resolve(
                            context,
                            new ConfigValueResolver.ConfigValueResolverParam
                            {
                                Type = info.ParameterType.GetTypeInfo(),
                                Attribute = it.ConfigValueAttribute,
                            });
                        if (success)
                        {
                            return value;
                        }

                        if (info.HasDefaultValue)
                        {
                            return info.DefaultValue;
                        }

                        return null;
                    }
                ))).ToList();
            };
        }

        private void ProcessField(IComponentRegistration registration)
        {
            var type = registration.Activator.LimitType;
            var fields = type.GetTypeInfo().GetAllFields().Where(it => !it.IsStatic)
                .Select(it => (FieldInfo: it, ConfigValueAttribute: it.GetCustomAttribute<ConfigValueAttribute>()))
                .Where(it => it.ConfigValueAttribute != null)
                .ToList();
            if (fields.IsNullOrEmpty())
            {
                return;
            }

            registration.Activating += (sender, args) =>
            {
                foreach (var (fieldInfo, valueAttr) in fields)
                {
                    var (success, value) = _resolver.Resolve(
                        args.Context,
                        new ConfigValueResolver.ConfigValueResolverParam
                        {
                            Type = fieldInfo.FieldType.GetTypeInfo(),
                            Attribute = valueAttr,
                        });
                    if (success)
                    {
                        fieldInfo.SetValue(args.Instance, value);
                    }
                }
            };
        }
    }
}