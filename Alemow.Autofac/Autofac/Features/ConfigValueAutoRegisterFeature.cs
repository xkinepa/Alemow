using System;
using System.Linq;
using System.Reflection;
using Alemow.Attributes;
using Alemow.Autofac.Resolvers;
using Alemow.Miscs;
using Autofac.Builder;

namespace Alemow.Autofac.Features
{
    internal class ConfigValueAutoRegisterFeature : IAutoRegisterFeature
    {
        private readonly ConfigValueResolver _resolver = new ConfigValueResolver();

        public IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>
            Apply<TLimit, TActivatorData, TRegistrationStyle>(
                IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder, Type type)
        {
            var fieldList = type.GetTypeInfo().GetAllFields().Where(it => !it.IsStatic).ToList();
            if (fieldList.SelectMany(it => it.GetCustomAttributes<ConfigValueAttribute>()).Any())
            {
                return builder.OnActivating(e =>
                {
                    var instance = e.Instance;
                    foreach (var fieldInfo in fieldList)
                    {
                        var valueAttr = fieldInfo.GetCustomAttribute<ConfigValueAttribute>();
                        if (valueAttr == null)
                        {
                            continue;
                        }

                        var valueType = fieldInfo.FieldType;
                        var (success, value) = _resolver.Resolve(
                            e.Context,
                            valueType.GetTypeInfo(),
                            new ConfigValueResolver.ConfigValueResolverParam
                            {
                                Attribute = valueAttr,
                            });
                        if (success)
                        {
                            fieldInfo.SetValue(instance, value);
                        }
                    }
                });
            }

            return builder;
        }
    }
}