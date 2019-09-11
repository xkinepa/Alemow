using System.Collections.Generic;
using System.Reflection;
using Autofac;

namespace Alemow.Autofac
{
    public class AutoRegisterConfigurer
    {
        private readonly AutoRegisterModule _module;

        public ContainerBuilder ContainerBuilder { get; }

        public AutoRegisterConfigurer(ContainerBuilder containerBuilder, IEnumerable<Assembly> assemblies)
        {
            ContainerBuilder = containerBuilder;

            _module = new AutoRegisterModule(assemblies);
            containerBuilder.RegisterModule(_module);
        }

        public AutoRegisterConfigurer Enable(IAutoRegisterFeature feature)
        {
            _module.Enable(feature);
            return this;
        }
    }
}