using System.Globalization;

namespace CursorTestApp.Models;

/// <summary>
/// 語系選項，用於語系切換下拉選單。
/// </summary>
public sealed class LanguageOption
{
    /// <summary>
    /// 文化代碼（如 ja、zh-TW、en）。
    /// </summary>
    public string CultureName { get; init; } = string.Empty;

    /// <summary>
    /// 該語系下的顯示名稱（如 日本語、繁體中文、English）。
    /// </summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>
    /// 對應的 CultureInfo。
    /// </summary>
    public CultureInfo Culture => new(CultureName);

    public override string ToString() => DisplayName;
}
