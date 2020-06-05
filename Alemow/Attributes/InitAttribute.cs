using System;

namespace Alemow.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class InitAttribute : Attribute
    {
    }
}