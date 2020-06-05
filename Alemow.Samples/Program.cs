using System;
using System.Reflection;
using Alemow.Assemblies;
using Alemow.Attributes;
using Alemow.Autofac;
using Alemow.Config;
using Autofac;
using Microsoft.Extensions.Configuration;
using Module = Autofac.Module;

namespace Alemow
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var conf = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new ContainerBuilder();

            builder.RegisterInstance(conf).ExternallyOwned().AsImplementedInterfaces();

            builder.AutoRegister()
                .AssemblySelectors(new SimpleAssemblySelector(Assembly.GetEntryAssembly()))
                .ConfigResolver(new ConfigurationConfigResolver(conf))
                .Profile("default");

            using (var container = builder.Build())
            {
                var a = container.Resolve<IA>();
                a.Dump();
            }
        }
    }

    [Configuration]
    public class BModule : Module
    {
        [Bean(false, false),
         Scope(Scope.PerDependency),
         As(typeof(IB)),
         Key("B", typeof(IB))]
        public IB DefineB([ConfigValue("foo")] string foo, [Inject] C c)
        {
            return new B();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<C>();
        }
    }

    [Component(true, false),
     Scope(Scope.Singleton),
     As(typeof(IA)),
     Key("A", typeof(IA))]
    public class A : IA
    {
        [ConfigValue("foo")] private readonly string _foo;
        [Inject("B")] private readonly IB _b;

        [Init]
        public void Init()
        {
            Console.WriteLine("Init");
        }

        [Destroy]
        public void Destroy()
        {
            Console.WriteLine("Destroy");
        }

        public void Dump()
        {
            Console.WriteLine(_foo);
            Console.WriteLine(_b != null);
        }
    }

    public interface IA
    {
        void Dump();
    }

    public class B : IB
    {
    }

    public interface IB
    {
    }

    public class C
    {
    }
}
