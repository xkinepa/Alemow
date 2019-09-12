# Alemow

[![NuGet](https://img.shields.io/nuget/v/Alemow.Autofac.svg)](https://www.nuget.org/packages/Alemow.Autofac)
[![NuGet](https://img.shields.io/nuget/dt/Alemow.Autofac.svg)](https://www.nuget.org/packages/Alemow.Autofac)

[![License MIT](https://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 

[![GitHub stars](https://img.shields.io/github/stars/xkinepa/Alemow.svg?style=social&label=Star)](https://github.com/xkinepa/Alemow)
[![GitHub forks](https://img.shields.io/github/forks/xkinepa/Alemow.svg?style=social&label=Fork)](https://github.com/xkinepa/Alemow)
[![GitHub watchers](https://img.shields.io/github/watchers/xkinepa/Alemow.svg?style=social&label=Watch)](https://github.com/xkinepa/Alemow)

Alemow is an annotation based IoC boostrapper, working with Autofac.

## Basics

Alemow currently only targets netstandard2.0, and only `Autofac` is integrated for now.

## Usages

### NuGet
`Install-Package Alemow.Autofac`

### Register

```csharp
    // var builder = new ContainerBuilder();
    builder.AutoRegister(new SimpleAssemblySelector(Assembly.GetEntryAssembly()));
```

or

```csharp
    // var builder = new ContainerBuilder();
    builder.AutoRegisterBaseDirectory(AssemblyLoadContext.Default.LoadFromAssemblyPath, excludes: Enumerables.List(@"System.*", @"Microsoft.*")); // use Assembly.LoadFile for dotnetfx
```

### ComponentAttribute

`AutoRegister` will load all defined types from `IAssemblySelector.Find` with `[ComponentAttribute]` annotated.

```csharp
    [Component(asSelf: true, asImplementedInterfaces: false),
     Scope(Scope.Singleton),
     As(typeof(IService)),
     Key("A", typeof(IService))]
    public class Service : IService
    {
        [Inject] private readonly IOtherService _otherService;

        [ConfigValue("api:key")] private readonly string _apiKey;

        public Service(IRepository repository)
        {
            //
        }
    }
```

* `[ComponentAttribute]` indicates this type will be registered automatically;
* Scope, As, Keyed, WithMetadata from Autofac are supported by different attributes;
* `[InjectAttribute]` annotated fields are resolved when `OnActivating`;
* `[ConfigValueAttribute]` annotated fields are resolved by `IConfiguration` when `OnActivating`, different types are supported such as `int`, `IList<string>`, etc;
* The `ILifetimeScope` is attached to the resolved instance by `LifetimeScopeAttacher.Instance`.

### ConfigurationAttribute

```csharp
    [Configuration]
    public class DataSourceConfiguration : Module
    {
        [Bean(false, false),
         Scope(Scope.PerDependency),
         As(typeof(IDataSource)),
         Key("DataSource", typeof(IDataSource))]
        public IDataSource DefineDataSource([ConfigValue("foo")] string foo, [Inject] IConfiguration configuration, IComponentContext context)
        {
            return new DataSource();
        }

        protected override void Load(ContainerBuilder builder)
        {
            //
        }
    }
```

* `[ConfigurationAttribute]` indicates this type will be parsed automatically, if this type is derived from `Autofac.Core.IModule`, this type will be registered as a module;
* `[BeanAttribute]` indicates this method will be used as the creator and the result will be returned as the resolved instance;
* `[InjectAttribute]` and `[ConfigValueAttribute]` are supported when resolving parameters;
* Specific type as `IComponentContext` is also supported;
* Method should be non-generic.

### Feature

You can define any features from `IAutoRegisterFeature` and enable with `builder.AutoRegister(/**/).Enable(feature);`, features will be applied to all `[ComponentAttribute]` types.
