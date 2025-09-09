using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Telemetry_demo_Avalonia;

public class BooleanToSidebarOpacityConverter : IValueConverter
{
    public static readonly BooleanToSidebarOpacityConverter Instance = new();
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool collapsed && collapsed)
            return 0d;
        return 1d;
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
} 