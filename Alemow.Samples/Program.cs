using System;
using System.Reflection;
using Alemow.Assemblies;
using Alemow.Attributes;
using Alemow.Autofac;
using Autofac;
using Microsoft.Extensions.Configuration;
using Module = Autofac.Module;

namespace Alemow.Samples
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

            builder.AutoRegister(new SimpleAssemblySelector(Assembly.GetEntryAssembly()));

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
