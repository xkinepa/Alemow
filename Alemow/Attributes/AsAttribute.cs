using System;

namespace Alemow.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AsAttribute : Attribute
    {
        public AsAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
    }
}