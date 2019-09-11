using System;
using JetBrains.Annotations;

namespace Alemow.Attributes
{
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Field/* | AttributeTargets.Property*/ | AttributeTargets.Parameter)]
    public class InjectAttribute : Attribute
    {
        public InjectAttribute(object key = null)
        {
            Key = key;
        }

        public object Key { get; }
    }
}