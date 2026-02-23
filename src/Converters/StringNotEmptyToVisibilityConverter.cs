using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CursorTestApp.Converters;

/// <summary>
/// 字串非空時為 Visible，空為 Collapsed。
/// </summary>
public sealed class StringNotEmptyToVisibilityConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string s && !string.IsNullOrWhiteSpace(s)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
