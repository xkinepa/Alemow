using Alemow.Autofac;
using Microsoft.Extensions.Configuration;

namespace Alemow.Config
{
    public static class AutoRegisterOptionsBuilderExtensions
    {
        public static AutoRegisterOptionsBuilder Configuration(this AutoRegisterOptionsBuilder optionsBuilder, IConfiguration configuration)
        {
            return optionsBuilder.ConfigResolver(new ConfigurationConfigResolver(configuration));
        }
    }
}