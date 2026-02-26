using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CursorTestApp.Helpers;
using CursorTestApp.Models;
using CursorTestApp.Services;
using CursorTestApp.Views.Layout2;

namespace CursorTestApp.ViewModels;

/// <summary>
/// Layout2 主畫面 ViewModel（生產資訊看板、排程、完成訂單、模擬生產）。
/// </summary>
public sealed partial class Layout2ViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly ILogService _logService;
    private readonly IProductionScheduleService _scheduleService;
    private readonly Rs485Service _rs485Service;
    private readonly ILocalizationService _localizationService;
    private DispatcherTimer? _simSpeedTimer;
    private DispatcherTimer? _simProductionTimer;
    private readonly Random _rnd = new();
    private SimulationMode _simulationMode = SimulationMode.None;

    [ObservableProperty]
    private ScheduleOrderItem? _selectedScheduleItem;

    [ObservableProperty]
    private bool _isSimulating;

    /// <summary>車速（RPM），模擬時 400–500 每秒更新</summary>
    [ObservableProperty]
    private int _speed;

    /// <summary>目前產量，模擬時每 30 秒 +1</summary>
    [ObservableProperty]
    private int _currentQuantity;

    /// <summary>預估完成所需時間顯示（停車中 或 剩餘時間）</summary>
    [ObservableProperty]
    private string _estimatedTimeDisplay = "Parking";

    /// <summary>Layout2 語系字串（RESX），供 View 綁定 .Value</summary>
    public LocalizedString ProductionInfoPanelTitle { get; }
    public LocalizedString OptLabel { get; }
    public LocalizedString PlcLabel { get; }
    public LocalizedString ScheduleTitle { get; }
    public LocalizedString F2ProductionCompleteToolTip { get; }
    public LocalizedString F4PreviewToolTip { get; }
    public LocalizedString F6RemoveOrderToolTip { get; }
    public LocalizedString OrderNoHeader { get; }
    public LocalizedString VersionNoHeader { get; }
    public LocalizedString OrderQuantityHeader { get; }
    public LocalizedString BoxTypeHeader { get; }
    public LocalizedString CategoryHeader { get; }
    public LocalizedString CustomerNameHeader { get; }
    public LocalizedString RemarksHeader { get; }
    public LocalizedString DateTimeHeader { get; }
    public LocalizedString AdjustOrderLabel { get; }
    public LocalizedString OrderMoveUpToolTip { get; }
    public LocalizedString OrderMoveDownToolTip { get; }
    public LocalizedString BackToLoginToolTip { get; }
    public LocalizedString F7OrderMakeToolTip { get; }
    public LocalizedString F1ScheduleManageToolTip { get; }
    /// <summary>模擬生產中時 F1 按鈕文字（F1 生產中）。</summary>
    public LocalizedString F1ProducingToolTip { get; }
    public LocalizedString F11StatusDisplayToolTip { get; }
    public LocalizedString ShutdownToolTip { get; }
    public LocalizedString ProductionOrderLabel { get; }
    public LocalizedString ProductionVersionLabel { get; }
    public LocalizedString SpeedLabel { get; }
    public LocalizedString CurrentQuantityLabel { get; }
    public LocalizedString OrderQuantityLabel { get; }
    public LocalizedString EstimatedCompleteLabel { get; }
    public LocalizedString OrderMakeWindowTitle { get; }
    public LocalizedString BlankPageText { get; }
    public LocalizedString StatusDialogTitle { get; }
    public LocalizedString StatusDialogContent { get; }

    /// <summary>左側面板字體大小：英文 16、其餘語系 20。</summary>
    public int LeftPanelFontSize => string.Equals(_localizationService.CurrentCulture.TwoLetterISOLanguageName, "en", StringComparison.OrdinalIgnoreCase) ? 16 : 20;

    /// <summary>F1 按鈕 ToolTip（模擬中顯示 F1 生產中，否則 F1 排程管理）。</summary>
    public string F1ButtonToolTip => IsSimulating ? F1ProducingToolTip.Value : F1ScheduleManageToolTip.Value;

    public Layout2ViewModel(
        INavigationService navigationService,
        ILogService logService,
        IProductionScheduleService scheduleService,
        Rs485Service rs485Service,
        ILocalizationService localizationService)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
        _rs485Service = rs485Service ?? throw new ArgumentNullException(nameof(rs485Service));
        _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));

        ProductionInfoPanelTitle = new LocalizedString(_localizationService, "Layout2_ProductionInfoPanelTitle");
        OptLabel = new LocalizedString(_localizationService, "Layout2_OPT");
        PlcLabel = new LocalizedString(_localizationService, "Layout2_PLC");
        ScheduleTitle = new LocalizedString(_localizationService, "Layout2_ScheduleTitle");
        F2ProductionCompleteToolTip = new LocalizedString(_localizationService, "Layout2_F2ProductionComplete");
        F4PreviewToolTip = new LocalizedString(_localizationService, "Layout2_F4Preview");
        F6RemoveOrderToolTip = new LocalizedString(_localizationService, "Layout2_F6RemoveOrder");
        OrderNoHeader = new LocalizedString(_localizationService, "Layout2_OrderNo");
        VersionNoHeader = new LocalizedString(_localizationService, "Layout2_VersionNo");
        OrderQuantityHeader = new LocalizedString(_localizationService, "Layout2_OrderQuantity");
        BoxTypeHeader = new LocalizedString(_localizationService, "Layout2_BoxType");
        CategoryHeader = new LocalizedString(_localizationService, "Layout2_Category");
        CustomerNameHeader = new LocalizedString(_localizationService, "Layout2_CustomerName");
        RemarksHeader = new LocalizedString(_localizationService, "Layout2_Remarks");
        DateTimeHeader = new LocalizedString(_localizationService, "Layout2_DateTime");
        AdjustOrderLabel = new LocalizedString(_localizationService, "Layout2_AdjustOrder");
        OrderMoveUpToolTip = new LocalizedString(_localizationService, "Layout2_OrderMoveUp");
        OrderMoveDownToolTip = new LocalizedString(_localizationService, "Layout2_OrderMoveDown");
        BackToLoginToolTip = new LocalizedString(_localizationService, "Layout2_BackToLogin");
        F7OrderMakeToolTip = new LocalizedString(_localizationService, "Layout2_F7OrderMake");
        F1ScheduleManageToolTip = new LocalizedString(_localizationService, "Layout2_F1ScheduleManage");
        F1ProducingToolTip = new LocalizedString(_localizationService, "Layout2_F1Producing");
        F11StatusDisplayToolTip = new LocalizedString(_localizationService, "Layout2_F11StatusDisplay");
        ShutdownToolTip = new LocalizedString(_localizationService, "Layout2_Shutdown");
        ProductionOrderLabel = new LocalizedString(_localizationService, "Layout2_ProductionOrder");
        ProductionVersionLabel = new LocalizedString(_localizationService, "Layout2_ProductionVersion");
        SpeedLabel = new LocalizedString(_localizationService, "Layout2_Speed");
        CurrentQuantityLabel = new LocalizedString(_localizationService, "Layout2_CurrentQuantity");
        OrderQuantityLabel = new LocalizedString(_localizationService, "Layout2_OrderQuantityLabel");
        EstimatedCompleteLabel = new LocalizedString(_localizationService, "Layout2_EstimatedComplete");
        OrderMakeWindowTitle = new LocalizedString(_localizationService, "Layout2_OrderMakeWindowTitle");
        BlankPageText = new LocalizedString(_localizationService, "Layout2_BlankPage");
        StatusDialogTitle = new LocalizedString(_localizationService, "Layout2_StatusDialogTitle");
        StatusDialogContent = new LocalizedString(_localizationService, "Layout2_StatusDialogContent");

        _localizationService.CultureChanged += (_, _) =>
        {
            UpdateEstimatedTime();
            OnPropertyChanged(nameof(LeftPanelFontSize));
        };

        _scheduleService.ScheduleOrders.CollectionChanged += (_, _) => UpdateFirstItemBindings();
        _scheduleService.CompletedOrders.CollectionChanged += (_, _) => { };
        UpdateFirstItemBindings();
    }

    /// <summary>排程第一筆（生產訂單號、版號、受訂量）供左側看板綁定</summary>
    public ScheduleOrderItem? FirstScheduleItem => _scheduleService.FirstScheduleItem;
    public ObservableCollection<ScheduleOrderItem> ScheduleOrders => _scheduleService.ScheduleOrders;
    public ObservableCollection<CompletedOrderItem> CompletedOrders => _scheduleService.CompletedOrders;
    public Rs485Service Rs485 => _rs485Service;

    private void UpdateFirstItemBindings()
    {
        OnPropertyChanged(nameof(FirstScheduleItem));
        if (!IsSimulating)
            UpdateEstimatedTime();
    }

    private void UpdateEstimatedTime()
    {
        var parking = _localizationService.GetString("Layout2_Parking") ?? "Parking";
        var first = FirstScheduleItem;
        if (first == null || first.OrderQuantity <= 0)
        {
            EstimatedTimeDisplay = parking;
            return;
        }
        int remaining = first.OrderQuantity - CurrentQuantity;
        if (remaining <= 0)
            EstimatedTimeDisplay = parking;
        else if (IsSimulating)
        {
            var fmt = _localizationService.GetString("Layout2_EstimatedTimeSec") ?? "About {0} sec";
            EstimatedTimeDisplay = string.Format(CultureInfo.CurrentCulture, fmt, remaining * 30);
        }
        else
            EstimatedTimeDisplay = parking;
    }

    [RelayCommand]
    private void NavigateBack()
    {
        StopSimulation();
        _logService.Append("返回登入畫面");
        _navigationService.NavigateToLogin();
    }

    [RelayCommand]
    private void F2ProductionComplete()
    {
        var item = SelectedScheduleItem;
        if (item == null) return;
        var orderNo = item.OrderNo;
        SelectedScheduleItem = null; // 先清除選取，避免從 ScheduleOrders 移除時 DataGrid SelectedItem 綁定拋出例外
        _scheduleService.MoveSelectedToCompleted(item);
        _logService.Append($"F2 生產完成：{orderNo}");
    }

    [RelayCommand]
    private void F6RemoveOrder()
    {
        var item = SelectedScheduleItem;
        if (item == null) return;
        var orderNo = item.OrderNo;
        SelectedScheduleItem = null; // 先清除選取，避免從 ScheduleOrders 移除時 DataGrid SelectedItem 綁定拋出例外
        _scheduleService.RemoveSelectedScheduleItem(item);
        _logService.Append($"F6 撤單：{orderNo}");
    }

    [RelayCommand]
    private void OrderMoveUp()
    {
        if (SelectedScheduleItem != null)
            _scheduleService.MoveScheduleItemUp(SelectedScheduleItem);
    }

    [RelayCommand]
    private void OrderMoveDown()
    {
        if (SelectedScheduleItem != null)
            _scheduleService.MoveScheduleItemDown(SelectedScheduleItem);
    }

    [RelayCommand]
    private void F1StartSchedule()
    {
        if (IsSimulating)
        {
            StopSimulation();
            return;
        }
        if (ScheduleOrders.Count == 0)
        {
            _logService.Append("排程為空，無法啟動模擬生產");
            return;
        }
        var vm = new ScheduleManageDialogViewModel(_scheduleService, _localizationService);
        var dialog = new ScheduleManageDialog(vm)
        {
            Owner = Application.Current.MainWindow
        };
        dialog.ShowDialog();
        if (dialog.DialogResult == true && dialog.Tag is SimulationMode mode && mode != SimulationMode.None)
            StartSimulationWithMode(mode);
    }

    private void StartSimulationWithMode(SimulationMode mode)
    {
        _simulationMode = mode;
        IsSimulating = true;
        _rs485Service.IsOptActive = true;
        _rs485Service.IsPlcActive = true;
        _rs485Service.HasError = false;
        _logService.Append(mode == SimulationMode.SmallBatch ? "F2 前置排單：進入模擬生產（小量 5 個）" : "F3 把全排量：進入模擬生產");

        _simSpeedTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _simSpeedTimer.Tick += (_, _) =>
        {
            Speed = _rnd.Next(400, 501);
            _rs485Service.Speed = Speed;
        };
        _simSpeedTimer.Start();
        Speed = _rnd.Next(400, 501);

        _simProductionTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
        _simProductionTimer.Tick += OnSimProductionTick;
        _simProductionTimer.Start();
    }

    private void OnSimProductionTick(object? sender, EventArgs e)
    {
        CurrentQuantity++;
        UpdateEstimatedTime();
        if (_simulationMode == SimulationMode.SmallBatch)
        {
            if (CurrentQuantity >= 5)
            {
                StopSimulation();
                _logService.Append("模擬生產（小量）：已生產 5 個，停止");
            }
            return;
        }
        var first = FirstScheduleItem;
        if (first != null && CurrentQuantity >= first.OrderQuantity)
        {
            _scheduleService.MoveSelectedToCompleted(first);
            CurrentQuantity = 0;
            _logService.Append($"模擬完成一筆：{first.OrderNo}");
            if (ScheduleOrders.Count == 0)
            {
                StopSimulation();
                _logService.Append("模擬生產：全部完成");
            }
        }
    }

    private void StopSimulation()
    {
        _simulationMode = SimulationMode.None;
        IsSimulating = false;
        _simSpeedTimer?.Stop();
        _simSpeedTimer = null;
        _simProductionTimer?.Stop();
        _simProductionTimer = null;
        _rs485Service.IsOptActive = false;
        _rs485Service.IsPlcActive = false;
        Speed = 0;
        _rs485Service.Speed = 0;
        OnPropertyChanged(nameof(FirstScheduleItem));
        UpdateEstimatedTime();
    }

    partial void OnIsSimulatingChanged(bool value)
    {
        OnPropertyChanged(nameof(F1ButtonToolTip));
        if (!value) return;
        CurrentQuantity = 0;
        UpdateEstimatedTime();
    }

    partial void OnCurrentQuantityChanged(int value) => UpdateEstimatedTime();

    /// <summary>F4 預覽：搜尋並選取排程項。回傳 true 表示找到並已選取。</summary>
    public bool SearchAndSelectSchedule(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return false;
        int idx = _scheduleService.FindScheduleIndex(keyword);
        if (idx < 0)
        {
            _logService.Append($"F4 預覽：找不到「{keyword}」");
            return false;
        }
        SelectedScheduleItem = ScheduleOrders[idx];
        _logService.Append($"F4 預覽：已選取「{keyword}」");
        return true;
    }

    [RelayCommand]
    private void F7OrderEdit()
    {
        var w = new Window
        {
            Title = OrderMakeWindowTitle.Value,
            Width = 400,
            Height = 300,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Application.Current.MainWindow
        };
        w.Content = new System.Windows.Controls.TextBlock { Text = BlankPageText.Value, Margin = new Thickness(24), VerticalAlignment = VerticalAlignment.Center };
        w.Show();
    }

    [RelayCommand]
    private void F11StatusDialog()
    {
        var w = new Window
        {
            Title = StatusDialogTitle.Value,
            Width = 400,
            Height = 300,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Application.Current.MainWindow
        };
        w.Content = new System.Windows.Controls.TextBlock { Text = StatusDialogContent.Value, Margin = new Thickness(24), VerticalAlignment = VerticalAlignment.Center };
        w.Show();
    }
}
