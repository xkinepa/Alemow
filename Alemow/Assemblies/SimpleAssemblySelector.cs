using System.Collections.Generic;
using System.Reflection;

namespace Alemow.Assemblies
{
    public class SimpleAssemblySelector : IAssemblySelector
    {
        private readonly IEnumerable<Assembly> _assemblies;

        public SimpleAssemblySelector(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        public SimpleAssemblySelector(params Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        public IEnumerable<Assembly> Find()
        {
            foreach (var assembly in _assemblies)
            {
                yield return assembly;
            }
        }
    }
}