using System;

namespace Alemow.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ConfigurationAttribute : Attribute
    {
        public ConfigurationAttribute(
            string profile = null
        )
        {
            Profile = profile;
        }

        public string Profile { get; }
    }
}