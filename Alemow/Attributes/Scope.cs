using System;

namespace Alemow.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ScopeAttribute : Attribute
    {
        public ScopeAttribute(Scope scope)
        {
            Scope = scope;
        }

        public Scope Scope { get; }
    }

    public enum Scope
    {
        PerDependency = 0,
        Singleton,
        PerLifetimeScope,
        //PerMatchingLifetime,
        //PerOwned,
    }
}