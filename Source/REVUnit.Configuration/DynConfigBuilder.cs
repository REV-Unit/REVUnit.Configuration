using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace REVUnit.Configuration
{
    internal class DynConfigParseSettings
    {
        public DynConfigParseSettings(Converter[] converters) => Converters = converters;

        public Converter[] Converters { get; }
    }

    internal class DynConfigBuilder
    {
        public DynConfigBuilder(DynConfigParseSettings settings) => Settings = settings;

        public DynConfigParseSettings Settings { get; }

        public static DynConfigBuilder Default { get; } =
            new(new DynConfigParseSettings(new Converter[] { new ConvertibleConverter() }));

        public DynConfig Build<TStructure>(IConfiguration config) => Build(config, typeof(TStructure));

        public DynConfig Build(IConfiguration config, Type structure)
        {
            string name;
            Type? declaringType = structure.DeclaringType;
            if (declaringType != null)
            {
                PropertyInfo[] sectionProperties = declaringType
                                                  .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                  .Where(p => p.PropertyType == structure).ToArray();
                if (sectionProperties.Length != 1)
                {
                    throw new Exception();
                }

                name = sectionProperties[0].Name;
            }
            else
            {
                name = "root";
            }

            Type[] subSectionTypes = structure.GetNestedTypes(BindingFlags.Public).Where(t => t.IsInterface).ToArray();
            DynConfig[] subSections = subSectionTypes.Select(s => Build(config, s)).ToArray();

            PropertyInfo[] properties = structure.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            ConfigKey[] keys = properties
                              .Where(p => p.PropertyType != structure && p.CanRead &&
                                          p.CanWrite)
                              .Select(p => ConfigKey.FromPropertyInfo(p, Settings.Converters)).ToArray();

            return new DynConfig(config, name, subSections, keys);
        }
    }
}