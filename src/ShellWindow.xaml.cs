using System.Windows;
using System.Windows.Controls;
using CursorTestApp.Services;
using CursorTestApp.ViewModels;
using CursorTestApp.Views.Login;
using CursorTestApp.Views.Main;
using Wpf.Ui.Controls;

namespace CursorTestApp;

/// <summary>
/// 主殼視窗，承載登入/主畫面與 Log 區域。
/// </summary>
public partial class ShellWindow : FluentWindow
{
    private readonly ILogService _logService;
    private readonly INavigationService _navigationService;
    private readonly ILocalizationService _localizationService;

    public ShellWindow()
    {
        InitializeComponent();

        IDatabaseService databaseService = new DatabaseService();
        IAuthService authService = new AuthService(databaseService);
        _logService = new LogService();
        _localizationService = new LocalizationService();
        _navigationService = new NavigationService(
            ContentHost,
            CreateMainView,
            () => CreateLoginView(authService));

        LogPanel.DataContext = _logService;
        _logService.Append("應用程式已啟動");

        _navigationService.LogPanelVisibilityChanged += OnLogPanelVisibilityChanged;
        _navigationService.NavigateToLogin();
    }

    private UserControl CreateLoginView(IAuthService authService)
    {
        LoginViewModel viewModel = new(authService, _logService, _navigationService, _localizationService);
        return new LoginView { DataContext = viewModel };
    }

    private UserControl CreateMainView()
    {
        MainViewModel viewModel = new(_navigationService, _logService, _localizationService);
        return new MainView { DataContext = viewModel };
    }

    private void OnLogPanelVisibilityChanged(object? sender, bool visible)
    {
        LogPanelBorder.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
    }
}
