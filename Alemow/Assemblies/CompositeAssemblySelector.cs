using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Alemow.Miscs;
using WildcardMatch;

namespace Alemow.Assemblies
{
    public class CompositeAssemblySelector : IAssemblySelector
    {
        private readonly IList<IAssemblySelector> _assemblySelectors;
        private readonly IList<string> _includes;
        private readonly IList<string> _excludes;

        public CompositeAssemblySelector()
        {
            _assemblySelectors = Enumerables.List<IAssemblySelector>();
            _includes = Enumerables.List<string>();
            _excludes = Enumerables.List<string>();
        }

        public CompositeAssemblySelector Union(IAssemblySelector assemblySelector)
        {
            _assemblySelectors.Add(assemblySelector);
            return this;
        }

        public CompositeAssemblySelector Includes(string pattern)
        {
            _includes.Add(pattern);
            return this;
        }

        public CompositeAssemblySelector Includes(IList<string> patterns)
        {
            foreach (var pattern in patterns)
            {
                _includes.Add(pattern);
            }

            return this;
        }

        public CompositeAssemblySelector Excludes(string pattern)
        {
            _excludes.Add(pattern);
            return this;
        }

        public CompositeAssemblySelector Excludes(IList<string> patterns)
        {
            foreach (var pattern in patterns)
            {
                _excludes.Add(pattern);
            }

            return this;
        }

        public IEnumerable<Assembly> Find()
        {
            foreach (var assembly in _assemblySelectors.SelectMany(it => it.Find()))
            {
                if (_includes.IsNullOrEmpty() || _includes.Any(p => Match(assembly, p)))
                {
                    if (_excludes.Any() && _excludes.Any(p => Match(assembly, p)))
                    {
                        continue;
                    }

                    yield return assembly;
                }
            }
        }

        private bool Match(Assembly assembly, string pattern)
        {
            return StringExtensions.WildcardMatch(pattern, assembly.FullName);
        }
    }
}