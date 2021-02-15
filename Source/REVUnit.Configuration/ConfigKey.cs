using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace REVUnit.Configuration
{
    public class ConfigKey
    {
        private readonly ConverterManager _converterManager;

        private ConfigKey(string name, bool isRequired, Type valueType, object? defaultValue,
                          IEnumerable<Converter> converters)
        {
            Name = name;
            IsRequired = isRequired;
            ValueType = valueType;
            DefaultValue = defaultValue;
            _converterManager = new ConverterManager(converters);
        }

        public string Name { get; }
        public bool IsRequired { get; }
        public Type ValueType { get; }
        public object? DefaultValue { get; }

        public bool TryGetValue(IConfiguration section, out object? result)
        {
            string? value = section[Name];
            if (value == null)
            {
                if (IsRequired)
                {
                    throw new RequiredKeyEmptyException(Name);
                }

                result = DefaultValue;
                return true;
            }

            result = _converterManager.Convert(value, ValueType);
            return true;
        }

        public bool TrySetValue(IConfiguration section, object? value) => true;

        internal static ConfigKey FromPropertyInfo(PropertyInfo propertyInfo, IEnumerable<Converter> converters)
        {
            string name = propertyInfo.Name;

            bool isRequired = propertyInfo.GetCustomAttribute<RequiredAttribute>() != null;

            Type valueType = propertyInfo.PropertyType;

            var defaultValueAttribute = propertyInfo.GetCustomAttribute<DefaultValueAttribute>();
            object? defaultValue = defaultValueAttribute == null
                ? GetTypeDefaultValue(valueType)
                : defaultValueAttribute.DefaultValue;

            return new ConfigKey(name, isRequired, valueType, defaultValue, converters);
        }

        public static object? GetTypeDefaultValue(Type type) =>
            type.IsValueType && Nullable.GetUnderlyingType(type) == null ? Activator.CreateInstance(type) : null;
    }
}