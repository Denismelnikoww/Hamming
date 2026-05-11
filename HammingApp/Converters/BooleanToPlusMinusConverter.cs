using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace HammingApp.Converters;

public class BooleanToPlusMinusConverter : IValueConverter
{
    public static readonly BooleanToPlusMinusConverter Instance = new();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? "+" : "-";
        }

        return "-";
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}