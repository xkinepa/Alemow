using System.Reflection;
using Alemow.Attributes;
using Alemow.Miscs;
using Autofac;

namespace Alemow.Autofac.Resolvers
{
    public class InjectResolver : IResolver<InjectResolver.InjectResolverParam>
    {
        public (bool, object) Resolve(IComponentContext context, TypeInfo type, InjectResolverParam param)
        {
            var attr = param.Attribute;
            var key = attr.Key;
            if (key != null)
            {
                if (context.TryResolveKeyed(key, type, out var value))
                {
                    return (true, value);
                }

                Assertion.IsTrue(!attr.Required, $"required inject type {type.Name} with key {key} not resolved.");
                return (false, null);
            }

            {
                if (context.TryResolve(type, out var value))
                {
                    return (true, value);
                }

                Assertion.IsTrue(!attr.Required, $"required inject type {type.Name} not resolved.");
                return (false, null);
            }
        }

        public class InjectResolverParam
        {
            public InjectAttribute Attribute { get; set; }
        }
    }
}