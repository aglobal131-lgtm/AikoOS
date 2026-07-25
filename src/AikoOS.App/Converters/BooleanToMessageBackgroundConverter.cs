using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AikoOS.App.Converters;

public sealed class BooleanToMessageBackgroundConverter
    : IValueConverter
{
    private static readonly Brush UserBrush =
        new SolidColorBrush(
            Color.FromRgb(48, 91, 168));

    private static readonly Brush AssistantBrush =
        new SolidColorBrush(
            Color.FromRgb(32, 37, 49));

    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        return value is true
            ? UserBrush
            : AssistantBrush;
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