using System.Reflection;
using Autofac;

namespace Alemow.Autofac.Resolvers
{
    public interface IResolver<TP>
    {
        (bool, object) Resolve(IComponentContext context, TypeInfo type, TP param);
    }
}