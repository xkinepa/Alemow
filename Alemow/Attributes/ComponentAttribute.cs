using System;

namespace Alemow.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
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