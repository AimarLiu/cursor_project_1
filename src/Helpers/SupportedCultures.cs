using System.Globalization;
using CursorTestApp.Models;

namespace CursorTestApp.Helpers;

/// <summary>
/// 支援的語系清單與對應邏輯。
/// </summary>
public static class SupportedCultures
{
    private static readonly LanguageOption[] AllOptions =
    [
        new LanguageOption { CultureName = "ja", DisplayName = "日本語" },
        new LanguageOption { CultureName = "zh-TW", DisplayName = "繁體中文" },
        new LanguageOption { CultureName = "pt", DisplayName = "Português" },
        new LanguageOption { CultureName = "en", DisplayName = "English" },
        new LanguageOption { CultureName = "th", DisplayName = "ไทย" }
    ];

    /// <summary>
    /// 取得所有支援的語系選項。
    /// </summary>
    public static IReadOnlyList<LanguageOption> Options => AllOptions;

    /// <summary>
    /// 從系統目前語系解析出最接近的支援語系。
    /// 若無法對應則回傳英文。
    /// </summary>
    public static LanguageOption ResolveFromSystem()
    {
        string name = CultureInfo.CurrentUICulture.Name;
        if (name.StartsWith("ja", StringComparison.OrdinalIgnoreCase))
            return AllOptions[0];
        if (name.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
            return name.StartsWith("zh-TW", StringComparison.OrdinalIgnoreCase) ? AllOptions[1] : AllOptions[1]; // zh-CN 也給繁中或可加 zh-CN
        if (name.StartsWith("pt", StringComparison.OrdinalIgnoreCase))
            return AllOptions[2];
        if (name.StartsWith("th", StringComparison.OrdinalIgnoreCase))
            return AllOptions[4];
        return AllOptions[3]; // en
    }
}
