using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Telemetry_demo_Avalonia;

public class BooleanToSidebarWidthConverter : IValueConverter
{
    public static readonly BooleanToSidebarWidthConverter Instance = new();
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool collapsed && collapsed)
            return 44d;
        return 150d;
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
} 