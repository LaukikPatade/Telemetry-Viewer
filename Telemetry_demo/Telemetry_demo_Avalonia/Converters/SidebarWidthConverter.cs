using System;
using Avalonia.Data.Converters;
using System.Globalization;

namespace Telemetry_demo_Avalonia.Converters
{
    public class SidebarWidthConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool collapsed && collapsed)
                return 40d;
            return 260d;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 