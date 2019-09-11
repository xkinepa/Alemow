using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Alemow.Assemblies;
using Xunit;

namespace Alemow
{
    public class AssemblySelectorTests
    {
        private readonly Assembly _assembly = typeof(AssemblySelectorTests).Assembly;

        [Fact]
        public void SimpleAssemblySelector()
        {
            var assemblies = new SimpleAssemblySelector(_assembly).Find().ToList();
            Assert.Contains(assemblies, it => it.GetName().Name == _assembly.GetName().Name);
        }

        [Fact]
        public void ReferencedAssemblySelector()
        {
            var assemblies = new ReferencedAssemblySelector(_assembly).Find().ToList();
            Assert.Contains(assemblies, it => it.GetName().Name == _assembly.GetName().Name);
            foreach (var referencedAssembly in _assembly.GetReferencedAssemblies())
            {
                Assert.Contains(assemblies, it => it.GetName().Name == referencedAssembly.Name);
            }
        }

        [Fact]
        public void DirectoryAssemblySelector()
        {
            var assemblies = new DirectoryAssemblySelector(AssemblyLoadContext.Default.LoadFromAssemblyPath).Find().ToList();
            Assert.Contains(assemblies, it => it.GetName().Name == _assembly.GetName().Name);
            Assert.Contains(assemblies, it => it.GetName().Name == "Alemow.Empty");
        }
    }
}
