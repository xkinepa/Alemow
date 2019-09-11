using System;
using System.Collections.Generic;
using System.Reflection;
using Alemow.Autofac.Features;
using Alemow.Miscs;
using Autofac;

namespace Alemow.Autofac
{
    internal class AutoRegisterModule : global::Autofac.Module
    {
        private readonly IEnumerable<Assembly> _assemblies;
        private readonly IList<IAutoRegisterFeature> _features;

        public AutoRegisterModule(Assembly assembly)
            : this(Enumerables.List(assembly))
        {
        }

        public AutoRegisterModule(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
            _features = Enumerables.List<IAutoRegisterFeature>(
                new InjectionAutoRegisterFeature(),
                new ConfigValueAutoRegisterFeature(),
                new AttachLifetimeScopeAutoRegisterFeature()
            );
        }

        public void Enable(IAutoRegisterFeature feature)
        {
            _features.Add(feature);
        }

        protected override void Load(ContainerBuilder builder)
        {
            foreach (var assembly in _assemblies)
            {
                RegisterFromAssembly(builder, assembly);
            }
        }

        private void RegisterFromAssembly(ContainerBuilder builder, Assembly assembly)
        {
            var registerers = Enumerables.List<IRegisterer>(
                new ComponentRegisterer(builder, _features),
                new ConfigurationRegisterer(builder)
                );

            foreach (var type in assembly.ExportedTypes)
            {
                foreach (var registerer in registerers)
                {
                    registerer.Register(type);
                }
            }
        }
    }

    internal interface IRegisterer
    {
        void Register(Type type);
    }
}