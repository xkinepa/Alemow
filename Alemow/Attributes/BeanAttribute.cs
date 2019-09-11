using System;

namespace Alemow.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
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