using System.Reflection;
using Alemow.Miscs;

namespace Alemow.Config
{
    public interface IConfigResolver
    {
        object Get(string key, TypeInfo type);

        bool TryGet(string key, TypeInfo type, out object value);
    }

    public static class ConfigResolverExtensions
    {
        public static T Get<T>(this IConfigResolver configResolver, string key)
        {
            return (T)configResolver.Get(key, typeof(T).GetTypeInfo());
        }

        public static bool TryGet<T>(this IConfigResolver configResolver, string key, out T value)
        {
            var returnResult = configResolver.TryGet(key, typeof(T).GetTypeInfo(), out var returnValue);
            value = (T)returnValue;
            return returnResult;
        }
    }

    public class FakeConfigResolver : IConfigResolver
    {
        public object Get(string key, TypeInfo type)
        {
            throw Assertion.Fail($"no config resolver configured");
        }

        public bool TryGet(string key, TypeInfo type, out object value)
        {
            throw Assertion.Fail($"no config resolver configured");
        }
    }
}