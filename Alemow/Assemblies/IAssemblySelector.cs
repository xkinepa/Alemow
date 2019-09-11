using System.Collections.Generic;
using System.Reflection;

namespace Alemow.Assemblies
{
    public interface IAssemblySelector
    {
        IEnumerable<Assembly> Find();
    }
}