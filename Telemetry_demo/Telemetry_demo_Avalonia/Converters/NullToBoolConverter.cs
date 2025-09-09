using System;
using Avalonia.Data.Converters;
using System.Globalization;

namespace Telemetry_demo_Avalonia.Converters
{
    public class NullToBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool invert = parameter != null && bool.TryParse(parameter.ToString(), out var b) && b;
            bool isNull = value == null;
            return invert ? isNull : !isNull;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 