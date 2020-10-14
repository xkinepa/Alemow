using System;
using Autofac;
using Autofac.Builder;
using Autofac.Core;

namespace Alemow.Autofac
{
    public interface IFeature
    {
    }

    public interface IAutoRegisterFeature : IFeature
    {
        IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>
            Apply<TLimit, TActivatorData, TRegistrationStyle>(IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder, Type type)
            where TActivatorData : ReflectionActivatorData;
    }

    public interface IContainerBuilderFeature : IFeature
    {
        void Configure(ContainerBuilder containerBuilder);
    }

    public interface IComponentRegistrationFeature : IFeature
    {
        void Configure(IComponentRegistry componentRegistry, IComponentRegistration registration);
    }

    public interface IRegistrationSourceFeature : IFeature
    {
        void Configure(IComponentRegistry componentRegistry, IRegistrationSource registrationSource);
    }
}