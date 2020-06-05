using System;

namespace Alemow.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ComponentAttribute : Attribute
    {
        public ComponentAttribute(
            bool asSelf = true,
            bool asImplementedInterfaces = true,
            string profile = null
        )
        {
            AsSelf = asSelf;
            AsImplementedInterfaces = asImplementedInterfaces;
            Profile = profile;
        }

        public bool AsSelf { get; }

        public bool AsImplementedInterfaces { get; }

        public string Profile { get; }
    }
}