# Alemow

[![NuGet](https://img.shields.io/nuget/v/Alemow.Autofac.svg)](https://www.nuget.org/packages/Alemow.Autofac)
[![NuGet](https://img.shields.io/nuget/dt/Alemow.Autofac.svg)](https://www.nuget.org/packages/Alemow.Autofac)

[![License MIT](https://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 

[![GitHub stars](https://img.shields.io/github/stars/xkinepa/Alemow.svg?style=social&label=Star)](https://github.com/xkinepa/Alemow)
[![GitHub forks](https://img.shields.io/github/forks/xkinepa/Alemow.svg?style=social&label=Fork)](https://github.com/xkinepa/Alemow)
[![GitHub watchers](https://img.shields.io/github/watchers/xkinepa/Alemow.svg?style=social&label=Watch)](https://github.com/xkinepa/Alemow)

Alemow is an annotation based IoC boostrapper, working with Autofac, and reducing registering complexity heavily.

[Full docs](https://xkinepa.github.io/Alemow)

## Usages

```csharp
    var builder = new ContainerBuilder();
    builder.AutoRegister(new SimpleAssemblySelector(Assembly.GetEntryAssembly()));
    using (var container = builder.Build())
    {
        var a = container.Resolve<IA>();
        a.Dump();
    }

    [Component]
    public class A : IA
    {
        [Inject("B")] private readonly IB _b;

        public void Dump()
        {
            Console.WriteLine(_b);
        }
    }
```

See `Samples` for full usage.

## TODO
