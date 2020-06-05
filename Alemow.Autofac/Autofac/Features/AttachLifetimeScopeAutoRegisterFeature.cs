using System;
using Autofac;
using Autofac.Builder;

namespace Alemow.Autofac.Features
{
    [Obsolete]
    internal class AttachLifetimeScopeAutoRegisterFeature : IAutoRegisterFeature
    {
        public IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>
            Apply<TLimit, TActivatorData, TRegistrationStyle>(
                IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder, Type type)
            where TActivatorData : ReflectionActivatorData
        {
            return builder.OnActivated(e =>
            {
                var instance = e.Instance;
                var context = e.Context.Resolve<ILifetimeScope>();
                LifetimeScopeAttacher.Instance.Attach(instance, context);
                context.Disposer.AddInstanceForDisposal(
                    new ActionDisposer<TLimit>(it => LifetimeScopeAttacher.Instance.Detach(it), () => instance));
            });
        }
    }
}