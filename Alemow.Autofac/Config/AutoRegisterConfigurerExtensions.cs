using Alemow.Autofac;
using Microsoft.Extensions.Configuration;

namespace Alemow.Config
{
    public static class AutoRegisterConfigurerExtensions
    {
        public static AutoRegisterOptionsBuilder Configuration(this AutoRegisterOptionsBuilder optionsBuilder, IConfiguration configuration)
        {
            return optionsBuilder.ConfigResolver(new ConfigurationConfigResolver(configuration));
        }
    }
}