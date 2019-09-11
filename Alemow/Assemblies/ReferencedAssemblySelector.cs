using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Alemow.Assemblies
{
    public class ReferencedAssemblySelector : IAssemblySelector
    {
        private readonly Assembly _entryAssembly;

        public ReferencedAssemblySelector()
            : this(Assembly.GetEntryAssembly())
        {
        }

        public ReferencedAssemblySelector(Assembly entryAssembly)
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