using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Alemow.Assemblies
{
    public class DirectoryAssembliesSelector : IAssemblySelector
    {
        private readonly string _directory;
        private readonly Func<string, Assembly> _assemblyLoader;

        public DirectoryAssembliesSelector(Func<string, Assembly> assemblyLoader)
            : this(AppContext.BaseDirectory, assemblyLoader)
        {
        }

        public DirectoryAssembliesSelector(string directory, Func<string, Assembly> assemblyLoader)
        {
            _directory = directory;
            _assemblyLoader = assemblyLoader;
        }

        public IEnumerable<Assembly> Find()
        {
            return Directory.GetFiles(_directory, "*.dll", SearchOption.TopDirectoryOnly)
                .Select(_assemblyLoader);
        }
    }
}