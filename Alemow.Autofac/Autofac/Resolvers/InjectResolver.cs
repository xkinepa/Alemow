﻿using System.Reflection;
using Alemow.Attributes;
using Alemow.Miscs;
using Autofac;

namespace Alemow.Autofac.Resolvers
{
    public class InjectResolver : IResolver<InjectResolver.InjectResolverParam>
    {
        public (bool Successful, object Value) Resolve(IComponentContext context, InjectResolverParam param)
        {
            var type = param.Type;
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
            public TypeInfo Type { get; set; }
            public InjectAttribute Attribute { get; set; }
        }
    }
}