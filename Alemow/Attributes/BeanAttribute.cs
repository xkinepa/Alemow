using System;

namespace Alemow.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class BeanAttribute : Attribute
    {
        public BeanAttribute(
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