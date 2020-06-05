using System.Reflection;
using Alemow.Attributes;
using Alemow.Miscs;
using Autofac;

namespace Alemow.Autofac.Resolvers
{
    public class ParameterInfoResolver : IResolver<ParameterInfoResolverParam>
    {
        private readonly ConfigValueResolver _configValueResolver;
        private readonly InjectResolver _injectResolver;

        public ParameterInfoResolver(ConfigValueResolver configValueResolver, InjectResolver injectResolver)
        {
            _configValueResolver = configValueResolver;
            _injectResolver = injectResolver;
        }

        public (bool, object) Resolve(IComponentContext context, ParameterInfoResolverParam param)
        {
            var parameter = param.ParameterInfo;

            if (parameter.ParameterType == typeof(IComponentContext))
            {
                return (true, context);
            }

            var configValueAttr = parameter.GetCustomAttribute<ConfigValueAttribute>();
            if (configValueAttr != null)
            {
                var valueType = parameter.ParameterType;
                var (success, value) = _configValueResolver.Resolve(
                    context,
                    new ConfigValueResolver.ConfigValueResolverParam
                    {
                        Type = valueType.GetTypeInfo(),
                        Attribute = configValueAttr,
                    });

                if (success)
                {
                    return (true, value);
                }

                Assertion.IsTrue(parameter.HasDefaultValue, $"optional configValue parameter {parameter.Name} should have default value");
                return (true, parameter.DefaultValue);
            }

            var injectAttr = parameter.GetCustomAttribute<InjectAttribute>();
            if (injectAttr != null)
            {
                var valueType = parameter.ParameterType;
                var (success, value) = _injectResolver.Resolve(
                    context,
                    new InjectResolver.InjectResolverParam
                    {
                        Type = valueType.GetTypeInfo(),
                        Attribute = injectAttr,
                    });
                if (success)
                {
                    return (true, value);
                }

                Assertion.IsTrue(parameter.HasDefaultValue, $"optional inject parameter {parameter.Name} should have default value");
                return (true, parameter.DefaultValue);
            }

            return (true, context.Resolve(parameter.ParameterType));
        }
    }

    public class ParameterInfoResolverParam
    {
        public ParameterInfo ParameterInfo { get; set; }
    }
}