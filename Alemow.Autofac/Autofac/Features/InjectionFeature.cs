using System;
using System.Linq;
using System.Reflection;
using Alemow.Attributes;
using Alemow.Autofac.Resolvers;
using Alemow.Miscs;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;

namespace Alemow.Autofac.Features
{
    internal class InjectionFeature : IComponentRegistrationFeature
    {
        private readonly InjectResolver _resolver;

        public InjectionFeature()
        {
            _resolver = new InjectResolver();
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
                .Select(it => (ParameterInfo: it, InjectAttribute: it.GetCustomAttribute<InjectAttribute>()))
                .Where(it => it.InjectAttribute != null)
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
                            new InjectResolver.InjectResolverParam
                            {
                                Type = info.ParameterType.GetTypeInfo(),
                                Attribute = it.InjectAttribute,
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
                .Select(it => (FieldInfo: it, InjectAttribute: it.GetCustomAttribute<InjectAttribute>()))
                .Where(it => it.InjectAttribute != null)
                .ToList();
            if (fields.IsNullOrEmpty())
            {
                return;
            }

            registration.Activating += (sender, args) =>
            {
                foreach (var (fieldInfo, injectAttr) in fields)
                {
                    var (success, value) = _resolver.Resolve(
                        args.Context,
                        new InjectResolver.InjectResolverParam
                        {
                            Type = fieldInfo.FieldType.GetTypeInfo(),
                            Attribute = injectAttr,
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