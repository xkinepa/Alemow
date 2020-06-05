using System.Reflection;
using Alemow.Attributes;
using Alemow.Config;
using Alemow.Miscs;
using Autofac;

namespace Alemow.Autofac.Resolvers
{
    public class ConfigValueResolver : IResolver<ConfigValueResolver.ConfigValueResolverParam>
    {
        private readonly IConfigResolver _configResolver;

        public ConfigValueResolver(IConfigResolver configResolver)
        {
            _configResolver = configResolver;
        }

        public (bool Successful, object Value) Resolve(IComponentContext context, ConfigValueResolverParam param)
        {
            var type = param.Type;
            var attr = param.Attribute;
            var path = attr.Path;
            var exists = _configResolver.TryGet(path, type, out var val);
            if (!exists)
            {
                Assertion.IsTrue(!attr.Required, $"required config path {path} not resolved.");
                return (false, null);
            }

            return (true, val);
        }

        public class ConfigValueResolverParam
        {
            public TypeInfo Type { get; set; }
            public ConfigValueAttribute Attribute { get; set; }
        }
    }
}