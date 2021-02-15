using System;
using System.Collections.Generic;

namespace REVUnit.Configuration
{
    public abstract class Converter
    {
        public abstract bool TryConvert(string? s, Type targetType, out object? result);
    }

    public class ConvertibleConverter : Converter
    {
        public ConvertibleConverter(IFormatProvider? formatProvider = null) => FormatProvider = formatProvider;

        public IFormatProvider? FormatProvider { get; }

        public override bool TryConvert(string? s, Type targetType, out object? result)
        {
            result = null;
            try
            {
                if (s == null)
                {
                    return false;
                }

                result = ((IConvertible) s).ToType(targetType, FormatProvider);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class ClosureConverter : Converter
    {
        private readonly ConvertFunction _convertFunction;

        public ClosureConverter(ConvertFunction convertFunction) => _convertFunction = convertFunction;

        public override bool TryConvert(string? s, Type targetType, out object? result) =>
            _convertFunction(s, targetType, out result);
    }

    public delegate bool ConvertFunction(string? s, Type targetType, out object? result);

    internal class ConverterManager
    {
        public ConverterManager(IEnumerable<Converter> binders) => Binders = binders;

        public IEnumerable<Converter> Binders { get; }

        public object? Convert(string? s, Type targetType)
        {
            foreach (Converter binder in Binders)
            {
                if (binder.TryConvert(s, targetType, out object? result))
                {
                    return result;
                }
            }

            throw new Exception();
        }
    }
}