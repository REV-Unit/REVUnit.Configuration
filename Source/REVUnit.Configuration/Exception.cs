using System;

namespace REVUnit.Configuration
{
    public class ConfigException : Exception { }

    public class RequiredKeyEmptyException : ConfigException
    {
        public RequiredKeyEmptyException(string keyName) => KeyName = keyName;
        public string KeyName { get; }
        public override string Message => $"Config key \"{KeyName}\" is required";
    }
}