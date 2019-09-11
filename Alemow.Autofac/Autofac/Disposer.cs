using System;
using Autofac.Util;

namespace Alemow.Autofac
{
    public class ActionDisposer : Disposable
    {
        private readonly Action _action;

        public ActionDisposer(Action action)
        {
            _action = action;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _action();
            }

            base.Dispose(disposing);
        }
    }

    public class ActionDisposer<TLimit> : Disposable
    {
        private readonly Action<TLimit> _action;
        private readonly Func<TLimit> _factory;

        public ActionDisposer(Action<TLimit> action, TLimit instance)
            : this(action, () => instance)
        {
        }

        public ActionDisposer(Action<TLimit> action, Func<TLimit> factory)
        {
            _action = action;
            _factory = factory;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _action(_factory());
            }

            base.Dispose(disposing);
        }
    }

    public class InstanceDisposer<TLimit> : ActionDisposer<TLimit>
    {
        private static readonly Action<TLimit> Action = it => (it as IDisposable)?.Dispose();

        public InstanceDisposer(TLimit instance)
            : base(Action, instance)
        {
        }

        public InstanceDisposer(Func<TLimit> factory)
            : base(Action, factory)
        {
        }
    }
}