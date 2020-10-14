using System;
using System.Linq;
using System.Reflection;
using Alemow.Attributes;
using Alemow.Autofac.Resolvers;
using Alemow.Config;
using Alemow.Miscs;
using Autofac;
using Autofac.Builder;

namespace Alemow.Autofac.Features
{
    internal class LifecycleAutoRegisterFeature : IAutoRegisterFeature
    {
        private readonly ParameterInfoResolver _parameterInfoResolver;

        public LifecycleAutoRegisterFeature(IConfigResolver configResolver)
        {
            _parameterInfoResolver = new ParameterInfoResolver(new ConfigValueResolver(configResolver), new InjectResolver());
        }

        public IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>
            Apply<TLimit, TActivatorData, TRegistrationStyle>(
                IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder, Type type)
            where TActivatorData : ReflectionActivatorData
        {
            foreach (var methodInfo in type.GetTypeInfo().GetAllMethods().Where(it => !it.IsStatic))
            {
                RegisterInitMethod(builder, methodInfo);
                RegisterDestroyMethod(builder, methodInfo);
            }

            return builder;
        }

        private IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>
            RegisterInitMethod<TLimit, TActivatorData, TRegistrationStyle>(
                IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder, MethodInfo methodInfo)
        {
            var initAttr = methodInfo.GetCustomAttribute<InitAttribute>();
            if (initAttr == null)
            {
                return builder;
            }

            var returnType = methodInfo.ReturnType;
            if (returnType != typeof(void))
            {
                throw Assertion.Fail($"{nameof(InitAttribute)} annotated method should not return value");
            }

            builder.OnActivated(e =>
            {
                var parameters = ResolveParameters(e.Context, methodInfo);
                methodInfo.Invoke(e.Instance, parameters);
            });

            return builder;
        }

        private IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>
            RegisterDestroyMethod<TLimit, TActivatorData, TRegistrationStyle>(
                IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder, MethodInfo methodInfo)
        {
            var destroyAttr = methodInfo.GetCustomAttribute<DestroyAttribute>();
            if (destroyAttr == null)
            {
                return builder;
            }

            var returnType = methodInfo.ReturnType;
            if (returnType != typeof(void))
            {
                throw Assertion.Fail($"{nameof(DestroyAttribute)} annotated method should not return value");
            }

            builder.OnActivated(e =>
            {
                var parameters = ResolveParameters(e.Context, methodInfo);
                var context = e.Context.Resolve<ILifetimeScope>();
                context.Disposer.AddInstanceForDisposal(new ActionDisposer(() =>
                {
                    methodInfo.Invoke(e.Instance, parameters);
                }));
            });

            return builder;
        }

        private object[] ResolveParameters(IComponentContext context, MethodInfo method)
        {
            return method.GetParameters().Select(it => ResolveParameter(context, it)).ToArray();
        }

        private object ResolveParameter(IComponentContext context, ParameterInfo parameter)
        {
            var (success, resolved) = _parameterInfoResolver.Resolve(context, new ParameterInfoResolverParam
            {
                ParameterInfo = parameter,
            });
            Assertion.IsTrue(success, $"failed to resolve parameter type {parameter.ParameterType}");
            return resolved;
        }
    }
}