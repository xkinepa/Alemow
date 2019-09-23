using Alemow.Attributes;
using Autofac;

namespace Alemow
{
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

        public string Foo => _foo;
        public IB B => _b;
    }

    public interface IA
    {
        string Foo { get; }
        IB B { get; }
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