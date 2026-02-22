using System.Globalization;
using System.Resources;
using CursorTestApp.Helpers;

namespace CursorTestApp.Services;

/// <summary>
/// 多國語系服務實作，使用 Resources.resx 與 CultureInfo 切換。
/// </summary>
public sealed class LocalizationService : ILocalizationService
{
    private readonly ResourceManager _resourceManager;
    private CultureInfo _currentCulture;

    /// <inheritdoc />
    public CultureInfo CurrentCulture => _currentCulture;

    /// <inheritdoc />
    public event EventHandler<CultureInfo>? CultureChanged;

    /// <summary>
    /// 建立 LocalizationService 實例。預設使用系統語系對應的支援語系。
    /// </summary>
    /// <param name="initialCulture">可選，指定初始語系；若為 null 則使用系統語系解析</param>
    public LocalizationService(CultureInfo? initialCulture = null)
    {
        _resourceManager = new ResourceManager(
            "CursorTestApp.Resources.Resources",
            typeof(LocalizationService).Assembly);

        CultureInfo culture = initialCulture ?? SupportedCultures.ResolveFromSystem().Culture;
        _currentCulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }

    /// <inheritdoc />
    public void SetCulture(CultureInfo culture)
    {
        if (culture == null)
            throw new ArgumentNullException(nameof(culture));

        _currentCulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        CultureChanged?.Invoke(this, culture);
    }

    /// <inheritdoc />
    public string? GetString(string key)
    {
        if (string.IsNullOrEmpty(key))
            return key;

        return _resourceManager.GetString(key, _currentCulture) ?? key;
    }
}
