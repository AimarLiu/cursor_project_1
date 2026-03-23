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
    private readonly Func<object> _layout2ViewFactory;
    private readonly Func<object> _orderMakingViewFactory;
    private readonly Func<object> _settingsViewFactory;

    /// <summary>
    /// 建立 NavigationService。
    /// </summary>
    public NavigationService(
        ContentControl host,
        Func<object> mainViewFactory,
        Func<object> loginViewFactory,
        Func<object> layout2ViewFactory,
        Func<object> orderMakingViewFactory,
        Func<object> settingsViewFactory)
    {
        _host = host ?? throw new ArgumentNullException(nameof(host));
        _mainViewFactory = mainViewFactory ?? throw new ArgumentNullException(nameof(mainViewFactory));
        _loginViewFactory = loginViewFactory ?? throw new ArgumentNullException(nameof(loginViewFactory));
        _layout2ViewFactory = layout2ViewFactory ?? throw new ArgumentNullException(nameof(layout2ViewFactory));
        _orderMakingViewFactory = orderMakingViewFactory ?? throw new ArgumentNullException(nameof(orderMakingViewFactory));
        _settingsViewFactory = settingsViewFactory ?? throw new ArgumentNullException(nameof(settingsViewFactory));
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

    /// <inheritdoc />
    public void NavigateToLayout2()
    {
        _host.Content = _layout2ViewFactory();
        LogPanelVisibilityChanged?.Invoke(this, false);
    }

    /// <inheritdoc />
    public void NavigateToOrderMaking()
    {
        _host.Content = _orderMakingViewFactory();
        LogPanelVisibilityChanged?.Invoke(this, false);
    }

    /// <inheritdoc />
    public void NavigateToSettings()
    {
        _host.Content = _settingsViewFactory();
        LogPanelVisibilityChanged?.Invoke(this, false);
    }
}
