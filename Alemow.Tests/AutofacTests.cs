using Alemow.Autofac;
using Autofac;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Alemow
{
    public class AutofacTests : AutofacTestsBase
    {
        public AutofacTests() : base(b =>
        {
            b.RegisterInstance(new ConfigurationBuilder().Build()).As<IConfiguration>();
            b.AutoRegister(opts => opts.Assembly(typeof(AutofacTests).Assembly));
        })
        {
        }

        [Fact]
        public void Component()
        {
            Container.IsRegistered(typeof(A));
            Container.IsRegistered(typeof(IA));
            Container.IsRegisteredWithKey("A", typeof(IA));

            IA a1, a2;
            using (var context = Container.BeginLifetimeScope())
            {
                a1 = context.Resolve<IA>();
            }

            using (var context = Container.BeginLifetimeScope())
            {
                a2 = context.Resolve<IA>();
            }

            Assert.Same(a1, a2);
        }
    }
}