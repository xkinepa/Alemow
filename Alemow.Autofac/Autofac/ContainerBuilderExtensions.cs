using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Alemow.Assemblies;
using Alemow.Miscs;
using Autofac;

namespace Alemow.Autofac
{
    public static class ContainerBuilderExtensions
    {
        public static AutoRegisterConfigurer AutoRegister(this ContainerBuilder containerBuilder, params IAssemblySelector[] assemblySelectors)
        {
            Assertion.IsTrue(assemblySelectors.Any(), $"{nameof(assemblySelectors)} should contain at least one value");
            return new AutoRegisterConfigurer(containerBuilder, assemblySelectors.SelectMany(s => s.Find()));
        }

        public static AutoRegisterConfigurer AutoRegisterAssembly(this ContainerBuilder containerBuilder, Assembly assembly)
        {
            return AutoRegister(containerBuilder, new SimpleAssemblySelector(assembly));
        }

        public static AutoRegisterConfigurer AutoRegisterBaseDirectory(this ContainerBuilder containerBuilder, Func<string, Assembly> assemblyLoader,
            IList<string> includes = null,
            IList<string> excludes = null
        )
        {
            var assemblySelector = new CompositeAssemblySelector();
            assemblySelector.Union(new DirectoryAssemblySelector(assemblyLoader));
            if (includes != null)
            {
                assemblySelector.Includes(includes);
            }

            if (excludes != null)
            {
                assemblySelector.Excludes(excludes);
            }

            return AutoRegister(containerBuilder, assemblySelector);
        }
    }
}