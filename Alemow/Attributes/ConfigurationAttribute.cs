using System;

namespace Alemow.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ConfigurationAttribute : Attribute
    {
    }
}