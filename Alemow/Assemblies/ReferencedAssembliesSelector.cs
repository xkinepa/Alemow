using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Alemow.Assemblies
{
    public class ReferencedAssembliesSelector : IAssemblySelector
    {
        private readonly Assembly _entryAssembly;

        public ReferencedAssembliesSelector()
            : this(Assembly.GetEntryAssembly())
        {
        }

        public ReferencedAssembliesSelector(Assembly entryAssembly)
        {
            _entryAssembly = entryAssembly;
        }

        public IEnumerable<Assembly> Find()
        {
            yield return _entryAssembly;
            foreach (var assembly in _entryAssembly.GetReferencedAssemblies().Select(Assembly.Load))
            {
                yield return assembly;
            }
        }
    }
}