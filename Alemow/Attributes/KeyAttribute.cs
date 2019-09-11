using System;

namespace Alemow.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class KeyAttribute : Attribute
    {
        public KeyAttribute(object key, Type @as)
        {
            Key = key;
            As = @as;
        }

        public object Key { get; }

        public Type As { get; }
    }
}