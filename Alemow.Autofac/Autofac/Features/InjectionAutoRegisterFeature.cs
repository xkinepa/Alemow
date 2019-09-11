using System;
using System.Linq;
using System.Reflection;
using Alemow.Attributes;
using Alemow.Miscs;
using Autofac;
using Autofac.Builder;

namespace Alemow.Autofac.Features
{
    internal class InjectionAutoRegisterFeature : IAutoRegisterFeature
    {
        public IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>
            Apply<TLimit, TActivatorData, TRegistrationStyle>(
                IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder, Type type)
        {
            var fieldList = type.GetTypeInfo().GetAllFields().Where(it => !it.IsStatic).ToList();
            if (fieldList.SelectMany(it => it.GetCustomAttributes<InjectAttribute>()).Any())
            {
                return builder.OnActivating(e =>
                {
                    var instance = e.Instance;
                    foreach (var fieldInfo in fieldList)
                    {
                        var injectAttr = fieldInfo.GetCustomAttribute<InjectAttribute>();
                        if (injectAttr == null)
                        {
                            continue;
                        }

                        var key = injectAttr.Key;
                        var valueType = fieldInfo.FieldType;
                        var value = key != null ? e.Context.ResolveKeyed(key, valueType) : e.Context.Resolve(valueType);
                        fieldInfo.SetValue(instance, value);
                    }
                });
            }

            return builder;
        }
    }
}