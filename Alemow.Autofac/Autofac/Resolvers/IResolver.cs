using System.Reflection;
using Autofac;

namespace Alemow.Autofac.Resolvers
{
    public interface IResolver<TP>
    {
        (bool Successful, object Value) Resolve(IComponentContext context, TP param);
    }
}