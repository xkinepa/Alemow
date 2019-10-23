using System;

namespace Alemow.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class BeanAttribute : Attribute
    {
        public BeanAttribute(
            bool asSelf = true,
            bool asImplementedInterfaces = true
        )
        {
            AsSelf = asSelf;
            AsImplementedInterfaces = asImplementedInterfaces;
        }

        public bool AsSelf { get; }

        public bool AsImplementedInterfaces { get; }
    }
}