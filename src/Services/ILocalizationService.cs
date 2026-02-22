using System.Globalization;

namespace CursorTestApp.Services;

/// <summary>
/// 多國語系服務介面，支援 CultureInfo 切換與字串查詢。
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// 取得目前使用的文化設定。
    /// </summary>
    CultureInfo CurrentCulture { get; }

    /// <summary>
    /// 設定目前文化，並套用至目前執行緒。變更時會觸發 CultureChanged 事件。
    /// </summary>
    /// <param name="culture">欲切換的文化（如 ja、zh-TW、pt、en、th）</param>
    void SetCulture(CultureInfo culture);

    /// <summary>
    /// 依資源鍵名取得對應字串。
    /// </summary>
    /// <param name="key">資源鍵名（如 AppName）</param>
    /// <returns>目前文化下的字串，找不到時回傳 null 或鍵名</returns>
    string? GetString(string key);

    /// <summary>
    /// 文化變更時觸發，供 UI 重新綁定語系字串。
    /// </summary>
    event EventHandler<CultureInfo>? CultureChanged;
}
