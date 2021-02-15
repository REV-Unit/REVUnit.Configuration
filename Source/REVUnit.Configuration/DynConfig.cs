using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace REVUnit.Configuration
{
    public class DynConfig : DynamicObject
    {
        private readonly IConfiguration _config;
        private readonly Dictionary<string, ConfigKey> _keyDictionary;
        private readonly Dictionary<string, DynConfig> _sectionDictionary;

        internal DynConfig(IConfiguration config, string name, IEnumerable<DynConfig> sections,
                           IEnumerable<ConfigKey> keys)
        {
            _config = config;
            Name = name;
            _sectionDictionary = sections.ToDictionary(s => s.Name);
            _keyDictionary = keys.ToDictionary(k => k.Inner);
        }

        public string Name { get; }
        public IEnumerable<DynConfig> Sections => _sectionDictionary.Values;
        public IEnumerable<ConfigKey> Keys => _keyDictionary.Values;

        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            string configEntryName = binder.Name;

            if (!_sectionDictionary.TryGetValue(configEntryName, out DynConfig? configSection))
            {
                return _keyDictionary[configEntryName].TryGetValue(_config, out result);
            }

            result = configSection;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object? value)
        {
            string keyName = binder.Name;

            if (!_keyDictionary.TryGetValue(keyName, out ConfigKey? key))
            {
                throw new NotImplementedException();
            }

            return false;
        }
    }
}