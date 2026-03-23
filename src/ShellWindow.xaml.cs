using System.Windows;
using System.Windows.Controls;
using CursorTestApp.Services;
using CursorTestApp.ViewModels;
using CursorTestApp.Views.Layout2;
using CursorTestApp.Views.Login;
using CursorTestApp.Views.Main;
using CursorTestApp.Views.Settings;
using Wpf.Ui.Controls;

namespace CursorTestApp;

/// <summary>
/// 主殼視窗，承載登入/主畫面/Layout2 與 Log 區域。
/// </summary>
public partial class ShellWindow : FluentWindow
{
    private readonly IDatabaseService _databaseService;
    private readonly IKnifeSafetyParameterRepository _knifeSafetyRepository;
    private readonly IBoxDieCutterSettingsRepository _boxDieCutterRepository;
    private readonly IOtherPlcParametersRepository _otherPlcParametersRepository;
    private readonly IPlcCommChannelRepository _plcCommChannelRepository;
    private readonly IComponentDisplayNameRepository _componentDisplayNameRepository;
    private readonly IUserDirectoryRepository _userDirectoryRepository;
    private readonly ILogService _logService;
    private readonly INavigationService _navigationService;
    private readonly ILocalizationService _localizationService;
    private readonly IProductionScheduleService _scheduleService;
    private readonly IOrderRepository _orderRepository;
    private readonly Rs485Service _rs485Service;
    private readonly Rs485BackgroundService _rs485Background;

    public ShellWindow()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();
        _databaseService.Initialize();
        _knifeSafetyRepository = new KnifeSafetyParameterRepository(_databaseService);
        _boxDieCutterRepository = new BoxDieCutterSettingsRepository(_databaseService);
        _otherPlcParametersRepository = new OtherPlcParametersRepository(_databaseService);
        _plcCommChannelRepository = new PlcCommChannelRepository(_databaseService);
        _componentDisplayNameRepository = new ComponentDisplayNameRepository(_databaseService);
        _userDirectoryRepository = new UserDirectoryRepository(_databaseService);
        IAuthService authService = new AuthService(_databaseService);
        _logService = new LogService();
        _localizationService = new LocalizationService();
        IScheduleOrderRepository scheduleRepo = new ScheduleOrderRepository(_databaseService);
        _scheduleService = new ProductionScheduleService(scheduleRepo);
        _orderRepository = new OrderRepository(_databaseService);
        _rs485Service = new Rs485Service();
        _rs485Background = new Rs485BackgroundService(_rs485Service);
        _navigationService = new NavigationService(
            ContentHost,
            CreateMainView,
            () => CreateLoginView(authService),
            CreateLayout2View,
            CreateOrderMakingView,
            CreateSettingsView);

        LogPanel.DataContext = _logService;
        _logService.Append("應用程式已啟動");

        _navigationService.LogPanelVisibilityChanged += OnLogPanelVisibilityChanged;
        _navigationService.NavigateToLogin();

        Loaded += (_, _) => _rs485Background.Start();
        Closed += (_, _) => _rs485Background.Stop();
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

    private UserControl CreateLayout2View()
    {
        Layout2ViewModel viewModel = new(_navigationService, _logService, _scheduleService, _orderRepository, _rs485Service, _localizationService);
        return new Layout2View { DataContext = viewModel };
    }

    private UserControl CreateOrderMakingView()
    {
        OrderMakingDialogViewModel viewModel = new(_orderRepository, _scheduleService, _localizationService, _logService, _navigationService);
        return new OrderMakingView { DataContext = viewModel };
    }

    private UserControl CreateSettingsView()
    {
        SettingsViewModel viewModel = new(
            _databaseService,
            _navigationService,
            _logService,
            _localizationService,
            _knifeSafetyRepository,
            _boxDieCutterRepository,
            _otherPlcParametersRepository,
            _plcCommChannelRepository,
            _componentDisplayNameRepository,
            _userDirectoryRepository);
        return new SettingsView { DataContext = viewModel };
    }

    private void OnLogPanelVisibilityChanged(object? sender, bool visible)
    {
        LogPanelBorder.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
    }
}
