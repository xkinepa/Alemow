using System;
using System.Reflection;
using Alemow.Miscs;
using Microsoft.Extensions.Configuration;

namespace Alemow.Config
{
    public class ConfigurationConfigResolver : IConfigResolver
    {
        private readonly IConfiguration _configuration;

        public ConfigurationConfigResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object Get(string key, TypeInfo type)
        {
            Assertion.IsTrue(TryGet(key, type, out var value),
                $"key={key} not found in configuration");
            return value;
        }

        public bool TryGet(string key, TypeInfo type, out object value)
        {
            Assertion.HasLength(key, $"{nameof(key)} should be specified");

            var section = _configuration.GetSection(key);
            if (section.Exists())
            {
                value = section.Get(type);
                return true;
            }

            value = type.IsValueType ? Activator.CreateInstance(type) : null;
            return false;
        }
    }
}