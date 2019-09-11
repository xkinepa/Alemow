using System;

namespace Alemow.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class MetadataAttribute : Attribute
    {
        public MetadataAttribute(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }

        public object Value { get; }
    }
}