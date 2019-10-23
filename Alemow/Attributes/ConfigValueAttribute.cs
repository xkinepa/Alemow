using System;
using JetBrains.Annotations;

namespace Alemow.Attributes
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Field/* | AttributeTargets.Property*/ | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class ConfigValueAttribute : Attribute
    {
        public ConfigValueAttribute(string path, bool required = true)
        {
            Path = path;
            Required = required;
        }

        public string Path { get; }
        public bool Required { get; }
    }
}