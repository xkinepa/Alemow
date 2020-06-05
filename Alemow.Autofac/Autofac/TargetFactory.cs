using System.Reflection;
using Autofac;

namespace Alemow.Autofac
{
    public class ObjectFactoryTargetFactory : ITargetFactory
    {
        private readonly IObjectFactory _objectFactory;
        private readonly TypeInfo _type;

        public ObjectFactoryTargetFactory(IObjectFactory objectFactory, TypeInfo type)
        {
            _objectFactory = objectFactory;
            _type = type;
        }

        public object Instance => _objectFactory.CreateFor(_type);
    }

    public class AutofacTargetFactory : ITargetFactory
    {
        private readonly IComponentContext _context;
        private readonly TypeInfo _type;
        private readonly object _key;

        public AutofacTargetFactory(IComponentContext context, TypeInfo type) : this(context, type, null)
        {
        }

        public AutofacTargetFactory(IComponentContext context, TypeInfo type, object key)
        {
            _context = context;
            _type = type;
            _key = key;
        }

        public object Instance => _key == null ? _context.Resolve(_type) : _context.ResolveKeyed(_key, _type);
    }
}