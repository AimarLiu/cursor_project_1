using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace CursorTestApp.Converters;

/// <summary>
/// 將字串拆成直向標題用的一字一列（每個字元維持水平正向，由上而下排列；不使用 RotateTransform）。
/// </summary>
public sealed class StringToVerticalUprightElementsConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string s || string.IsNullOrEmpty(s))
            return Array.Empty<string>();

        var si = new StringInfo(s);
        var list = new List<string>(si.LengthInTextElements);
        for (var i = 0; i < si.LengthInTextElements; i++)
            list.Add(si.SubstringByTextElements(i, 1));

        return list;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
