using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Alemow.Assemblies;
using Alemow.Config;
using Autofac;

namespace Alemow.Autofac
{
    public class AutoRegisterOptionsBuilder
    {
        private AutoRegisterOptions Options { get; }

        public AutoRegisterOptionsBuilder()
        {
            Options = new AutoRegisterOptions();
        }

        public AutoRegisterOptionsBuilder Use<T>(T instance)
        {
            Options.ObjectFactory.Register<T>(instance);
            return this;
        }

        public AutoRegisterOptionsBuilder Assembly(Assembly assembly)
        {
            Options.Assemblies.Add(assembly);
            return this;
        }

        public AutoRegisterOptionsBuilder Assemblies(IEnumerable<Assembly> assemblies)
        {
            Options.Assemblies.AddRange(assemblies);
            return this;
        }

        public AutoRegisterOptionsBuilder Profile(string profile)
        {
            Options.Profiles.Add(profile);
            return this;
        }

        public AutoRegisterOptionsBuilder Profiles(IEnumerable<string> profiles)
        {
            Options.Profiles.AddRange(profiles);
            return this;
        }

        public AutoRegisterOptionsBuilder Feature<TFeature>() where TFeature : IFeature
        {
            Options.FeatureDefinitions.Add(new ObjectFactoryTargetFactory(Options.ObjectFactory, typeof(TFeature).GetTypeInfo()));
            return this;
        }

        public AutoRegisterOptionsBuilder Feature(IFeature feature)
        {
            Options.FeatureDefinitions.Add(new InstanceTargetFactory(feature));
            return this;
        }

        public AutoRegisterOptionsBuilder ConfigResolver(IConfigResolver configResolver)
        {
            Options.ConfigResolver = configResolver;
            return this;
        }

        internal AutoRegisterOptions Build()
        {
            return Options;
        }
    }

    public static class AutoRegisterOptionsBuilderExtensions
    {
        public static AutoRegisterOptionsBuilder AssemblySelector(this AutoRegisterOptionsBuilder optionsBuilder, IAssemblySelector assemblySelector)
        {
            return optionsBuilder.Assemblies(assemblySelector.Find());
        }

        public static AutoRegisterOptionsBuilder AssemblySelectors(this AutoRegisterOptionsBuilder optionsBuilder, params IAssemblySelector[] assemblySelectors)
        {
            return optionsBuilder.Assemblies(assemblySelectors.SelectMany(it => it.Find()));
        }

        public static AutoRegisterOptionsBuilder AssembliesFromBaseDirectory(this AutoRegisterOptionsBuilder optionsBuilder,
            Func<string, Assembly> assemblyLoader,
            IList<string> includes = null,
            IList<string> excludes = null)
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

            return AssemblySelector(optionsBuilder, assemblySelector);
        }

        public static AutoRegisterOptionsBuilder Enable(this AutoRegisterOptionsBuilder optionsBuilder, IAutoRegisterFeature feature)
        {
            return optionsBuilder.Feature(feature);
        }
    }

    internal class AutoRegisterOptions
    {
        public IObjectFactory ObjectFactory { get; } = new SimpleObjectFactory();

        public List<ITargetFactory> FeatureDefinitions { get; } = new List<ITargetFactory>();

        public List<string> Profiles { get; } = new List<string>();

        public List<Assembly> Assemblies { get; } = new List<Assembly>();

        public IConfigResolver ConfigResolver
        {
            get => ObjectFactory.Resolve<IConfigResolver>();
            set => ObjectFactory.Register<IConfigResolver>(value);
        }

        public IEnumerable<IAutoRegisterFeature> Features =>
            FeatureDefinitions.Select(it => it.Instance).OfType<IAutoRegisterFeature>();

        public IEnumerable<IContainerBuilderFeature> ContainerBuilderFeatures =>
            FeatureDefinitions.Select(it => it.Instance).OfType<IContainerBuilderFeature>();

        public IEnumerable<IComponentRegistrationFeature> ComponentRegistrationFeatures =>
            FeatureDefinitions.Select(it => it.Instance).OfType<IComponentRegistrationFeature>();

        public IEnumerable<IRegistrationSourceFeature> RegistrationSourceFeatures =>
            FeatureDefinitions.Select(it => it.Instance).OfType<IRegistrationSourceFeature>();

        internal AutoRegisterOptions()
        {
            ConfigResolver = new FakeConfigResolver();
        }
    }
}