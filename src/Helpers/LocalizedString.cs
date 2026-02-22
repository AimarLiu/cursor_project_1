using System.ComponentModel;
using System.Globalization;
using CursorTestApp.Services;

namespace CursorTestApp.Helpers;

/// <summary>
/// 可綁定的語系字串包裝類，綁定資源 key，在 CultureChanged 時自動發送 PropertyChanged。
/// 用於實現控制項文字的動態語系切換。
/// </summary>
public sealed class LocalizedString : INotifyPropertyChanged
{
    private readonly ILocalizationService _localizationService;
    private readonly string _key;

    /// <summary>
    /// 取得目前語系下的字串值。
    /// </summary>
    public string Value => _localizationService.GetString(_key) ?? _key;

    /// <summary>
    /// 建立 LocalizedString。
    /// </summary>
    /// <param name="localizationService">語系服務</param>
    /// <param name="key">資源鍵名</param>
    public LocalizedString(ILocalizationService localizationService, string key)
    {
        _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
        _key = key ?? throw new ArgumentNullException(nameof(key));

        _localizationService.CultureChanged += OnCultureChanged;
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnCultureChanged(object? sender, CultureInfo culture)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
    }
}
