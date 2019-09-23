using System.Reflection;
using Alemow.Attributes;
using Alemow.Miscs;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace Alemow.Autofac.Resolvers
{
    public class ConfigValueResolver : IResolver<ConfigValueResolver.ConfigValueResolverParam>
    {
        public (bool, object) Resolve(IComponentContext context, TypeInfo type, ConfigValueResolverParam param)
        {
            var attr = param.Attribute;
            var valuePath = attr.Path;
            var config = context.Resolve<IConfiguration>();
            var configSection = config.GetSection(valuePath);
            if (!configSection.Exists())
            {
                Assertion.IsTrue(!attr.Required, $"required config path {valuePath} not resolved.");
                return (false, null);
            }

            var value = configSection.Get(type);
            return (true, value);
        }

        public class ConfigValueResolverParam
        {
            public ConfigValueAttribute Attribute { get; set; }
        }
    }
}