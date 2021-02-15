using System;
using Microsoft.Extensions.Configuration;

namespace REVUnit.Configuration
{
    public static class ConfigurationExtensions
    {
        public static DynConfig DynamicBound(this IConfiguration config, Type structure) =>
            DynConfigBuilder.Default.Build(config, structure);

        public static DynConfig DynamicBound<TStructure>(this IConfiguration config) =>
            DynConfigBuilder.Default.Build<TStructure>(config);
    }
}