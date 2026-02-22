using CommunityToolkit.Mvvm.Input;
using CursorTestApp.Helpers;
using CursorTestApp.Services;

namespace CursorTestApp.ViewModels;

/// <summary>
/// 主畫面 ViewModel。
/// </summary>
public sealed partial class MainViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly ILogService _logService;

    /// <summary>
    /// 主畫面中央標題（動態語系）。
    /// </summary>
    public LocalizedString MainPageTitle { get; }

    /// <summary>
    /// 返回登入按鈕 ToolTip（動態語系）。
    /// </summary>
    public LocalizedString BackToLoginToolTip { get; }

    /// <summary>
    /// 建立 MainViewModel。
    /// </summary>
    public MainViewModel(
        INavigationService navigationService,
        ILogService logService,
        ILocalizationService localizationService)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));

        var loc = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
        MainPageTitle = new LocalizedString(loc, "MainPageTitle");
        BackToLoginToolTip = new LocalizedString(loc, "BackToLoginToolTip");
    }

    [RelayCommand]
    private void NavigateBack()
    {
        _logService.Append("返回登入畫面");
        _navigationService.NavigateToLogin();
    }
}
