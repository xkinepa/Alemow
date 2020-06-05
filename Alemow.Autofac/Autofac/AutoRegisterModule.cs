using System;
using System.Collections.Generic;
using System.Reflection;
using Alemow.Autofac.Features;
using Alemow.Config;
using Alemow.Miscs;
using Autofac;
using Autofac.Core;

namespace Alemow.Autofac
{
    internal class AutoRegisterModule : global::Autofac.Module
    {
        private readonly AutoRegisterOptionsBuilder _optionsBuilder;

        private AutoRegisterOptions _options;

        public AutoRegisterModule(AutoRegisterOptionsBuilder optionsBuilder)
        {
            _optionsBuilder = optionsBuilder;

            _optionsBuilder.Feature<ConfigValueFeature>();
            _optionsBuilder.Feature<InjectionFeature>();

            //_optionsBuilder.Feature<ConfigValueFieldAutoRegisterFeature>();
            //_optionsBuilder.Feature<InjectionFieldAutoRegisterFeature>();
            //_optionsBuilder.Feature<ConfigValueCtorParamAutoRegisterFeature>();
            //_optionsBuilder.Feature<InjectionCtorParamAutoRegisterFeature>();
            _optionsBuilder.Feature<LifecyleAutoRegisterFeature>();
            _optionsBuilder.Feature<AttachLifetimeScopeAutoRegisterFeature>();
        }

        protected override void Load(ContainerBuilder builder)
        {
            _options = _optionsBuilder.Build();

            foreach (var assembly in _options.Assemblies)
            {
                RegisterFromAssembly(builder, assembly);
            }

            builder.RegisterInstance(_options.ConfigResolver).As<IConfigResolver>().ExternallyOwned();

            builder.RegisterSource(new SingleResolverRegistrationSource());
        }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            foreach (var feature in _options.ComponentRegistrationFeatures)
            {
                feature.Configure(componentRegistry, registration);
            }
        }

        protected override void AttachToRegistrationSource(IComponentRegistry componentRegistry, IRegistrationSource registrationSource)
        {
            foreach (var feature in _options.RegistrationSourceFeatures)
            {
                feature.Configure(componentRegistry, registrationSource);
            }
        }

        private void RegisterFromAssembly(ContainerBuilder builder, Assembly assembly)
        {
            var registerers = Enumerables.List<IRegisterer>(
                new ComponentRegisterer(builder, _options),
                new ConfigurationRegisterer(builder, _options)
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