using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CursorTestApp.Converters;

/// <summary>
/// 用於 OPT（紅/灰）、PLC（綠/灰）LED。Parameter 傳 "Opt" 或 "Plc"。
/// </summary>
public sealed class BoolToLedBrushConverter : IValueConverter
{
    private static readonly Brush Red = new SolidColorBrush(Color.FromRgb(0xE5, 0x39, 0x35));
    private static readonly Brush Green = new SolidColorBrush(Color.FromRgb(0x43, 0xA0, 0x47));
    private static readonly Brush Gray = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool on = value is true;
        string? p = parameter?.ToString();
        if (string.Equals(p, "Opt", StringComparison.OrdinalIgnoreCase))
            return on ? Red : Gray;
        if (string.Equals(p, "Plc", StringComparison.OrdinalIgnoreCase))
            return on ? Green : Gray;
        return Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
