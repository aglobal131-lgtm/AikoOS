using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AikoOS.App.Converters;

public sealed class BooleanToHorizontalAlignmentConverter
    : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        return value is true
            ? HorizontalAlignment.Right
            : HorizontalAlignment.Left;
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}