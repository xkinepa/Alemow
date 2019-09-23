using System;
using JetBrains.Annotations;

namespace Alemow.Attributes
{
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Field/* | AttributeTargets.Property*/ | AttributeTargets.Parameter)]
    public class InjectAttribute : Attribute
    {
        public InjectAttribute(object key = null, bool required = true)
        {
            Key = key;
            Required = required;
        }

        public object Key { get; }
        public bool Required { get; }
    }
}