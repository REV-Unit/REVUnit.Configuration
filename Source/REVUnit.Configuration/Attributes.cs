using System;

namespace REVUnit.Configuration
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultValueAttribute : Attribute
    {
        public DefaultValueAttribute(object? defaultValue) => DefaultValue = defaultValue;

        public object? DefaultValue { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SectionAttribute : Attribute
    {
        public SectionAttribute(string sectionId) => SectionId = sectionId;
        public string SectionId { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class VerifierAttribute : Attribute
    {
        //public VerifierAttribute(Predicate<object?> verifier) => Verifier = verifier;
        public VerifierAttribute(Type verifierType) => VerifierType = verifierType;

        public Type VerifierType { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ConverterAttribute : Attribute
    {
        public ConverterAttribute(Predicate<object?> verifier) => Verifier = verifier;
        public Predicate<object?> Verifier { get; }
    }
}