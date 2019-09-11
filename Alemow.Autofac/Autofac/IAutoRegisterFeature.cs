using System;
using Autofac.Builder;

namespace Alemow.Autofac
{
    public interface IAutoRegisterFeature
    {
        IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>
            Apply<TLimit, TActivatorData, TRegistrationStyle>(IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder, Type type);
    }
}