using System.Windows;
using System.Windows.Controls;

namespace CursorTestApp.Services;

/// <summary>
/// 導航服務實作，以 ContentControl 切換 View。
/// </summary>
public sealed class NavigationService : INavigationService
{
    private readonly ContentControl _host;
    private readonly Func<object> _mainViewFactory;
    private readonly Func<object> _loginViewFactory;

    /// <summary>
    /// 建立 NavigationService。
    /// </summary>
    /// <param name="host">承載 View 的 ContentControl</param>
    /// <param name="mainViewFactory">主畫面工廠</param>
    /// <param name="loginViewFactory">登入畫面工廠</param>
    public NavigationService(
        ContentControl host,
        Func<object> mainViewFactory,
        Func<object> loginViewFactory)
    {
        _host = host ?? throw new ArgumentNullException(nameof(host));
        _mainViewFactory = mainViewFactory ?? throw new ArgumentNullException(nameof(mainViewFactory));
        _loginViewFactory = loginViewFactory ?? throw new ArgumentNullException(nameof(loginViewFactory));
    }

    /// <inheritdoc />
    public event EventHandler<bool>? LogPanelVisibilityChanged;

    /// <inheritdoc />
    public void NavigateToMain()
    {
        _host.Content = _mainViewFactory();
        LogPanelVisibilityChanged?.Invoke(this, false);
    }

    /// <inheritdoc />
    public void NavigateToLogin()
    {
        _host.Content = _loginViewFactory();
        LogPanelVisibilityChanged?.Invoke(this, true);
    }
}
