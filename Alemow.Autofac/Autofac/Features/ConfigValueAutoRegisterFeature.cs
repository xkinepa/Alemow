using System;
using System.Linq;
using System.Reflection;
using Alemow.Attributes;
using Alemow.Miscs;
using Autofac;
using Autofac.Builder;
using Microsoft.Extensions.Configuration;

namespace Alemow.Autofac.Features
{
    internal class ConfigValueAutoRegisterFeature : IAutoRegisterFeature
    {
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

                        var valuePath = valueAttr.Path;
                        var config = e.Context.Resolve<IConfiguration>();
                        var valueType = fieldInfo.FieldType;
                        fieldInfo.SetValue(instance, config.GetSection(valuePath).Get(valueType));
                    }
                });
            }

            return builder;
        }
    }
}