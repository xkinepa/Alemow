using System;

namespace Alemow.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentAttribute : Attribute
    {
        public ComponentAttribute(
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