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
        public static AutoRegisterOptionsBuilder AutoRegister(this ContainerBuilder containerBuilder,
            Action<AutoRegisterOptionsBuilder> setupAction,
            params IAssemblySelector[] assemblySelectors)
        {
            var builder = new AutoRegisterOptionsBuilder();

            if (!assemblySelectors.IsNullOrEmpty())
            {
                builder.Assemblies(assemblySelectors.SelectMany(s => s.Find()));
            }

            setupAction?.Invoke(builder);

            containerBuilder.RegisterModule(new AutoRegisterModule(builder));

            return builder;
        }

        public static AutoRegisterOptionsBuilder AutoRegister(this ContainerBuilder containerBuilder, params IAssemblySelector[] assemblySelectors)
            => AutoRegister(containerBuilder, _ => { }, assemblySelectors);

        public static AutoRegisterOptionsBuilder AutoRegister(this ContainerBuilder containerBuilder)
            => AutoRegister(containerBuilder, new SimpleAssemblySelector());

        public static AutoRegisterOptionsBuilder AutoRegisterBaseDirectory(this ContainerBuilder containerBuilder, Func<string, Assembly> assemblyLoader,
            IList<string> includes = null,
            IList<string> excludes = null
        )
        {
            var assemblySelector = new CompositeAssemblySelector();
            assemblySelector.Union(new DirectoryAssembliesSelector(assemblyLoader));
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