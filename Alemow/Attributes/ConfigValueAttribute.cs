using System;
using JetBrains.Annotations;

namespace Alemow.Attributes
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Field/* | AttributeTargets.Property*/ | AttributeTargets.Parameter)]
    public class ConfigValueAttribute : Attribute
    {
        public ConfigValueAttribute(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}