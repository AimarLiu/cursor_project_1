using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CursorTestApp.Helpers;
using CursorTestApp.Models;
using CursorTestApp.Services;

namespace CursorTestApp.ViewModels;

/// <summary>
/// Phase 6 訂單製作 Dialog ViewModel。流程 3.0～3.4、反灰／解除反灰、F5 雙態。
/// </summary>
public sealed partial class OrderMakingDialogViewModel : ViewModelBase
{
    private readonly IOrderRepository _orderRepo;
    private readonly IProductionScheduleService _scheduleService;
    private readonly ILocalizationService _localizationService;
    private readonly ILogService _logService;
    private readonly INavigationService? _navigationService;

    [ObservableProperty]
    private string _versionNoSearch = string.Empty;

    [ObservableProperty]
    private string _versionNoQuery = string.Empty;

    [ObservableProperty]
    private string _orderNo = string.Empty;

    [ObservableProperty]
    private int _orderQuantity;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsBoxTypeE))]
    [NotifyPropertyChangedFor(nameof(IsBoxTypeS))]
    private string _boxType = "E";

    public bool IsBoxTypeE { get => BoxType == "E"; set { if (value) BoxType = "E"; } }
    public bool IsBoxTypeS { get => BoxType == "S"; set { if (value) BoxType = "S"; } }

    [ObservableProperty]
    private string _category = "A";

    [ObservableProperty]
    private string _customerName = string.Empty;

    [ObservableProperty]
    private string _remarks = string.Empty;

    [ObservableProperty]
    private string _phase1 = string.Empty;

    [ObservableProperty]
    private string _phase2 = string.Empty;

    [ObservableProperty]
    private string _phase3 = string.Empty;

    [ObservableProperty]
    private int _length;

    [ObservableProperty]
    private int _width;

    /// <summary>左側輸入區與下側預覽區是否可編輯（依流程反灰）。</summary>
    [ObservableProperty]
    private bool _isLeftAndBottomEnabled;

    /// <summary>F4 刪除是否可點擊。</summary>
    [ObservableProperty]
    private bool _isF4Enabled;

    /// <summary>F5 完成編輯／新增訂單是否可點擊。</summary>
    [ObservableProperty]
    private bool _isF5Enabled;

    /// <summary>F2 加入排程／離開是否可點擊。</summary>
    [ObservableProperty]
    private bool _isF2Enabled;

    /// <summary>F5 按鈕文字：完成編輯訂單 或 新增訂單。</summary>
    [ObservableProperty]
    private string _f5ButtonText = string.Empty;

    /// <summary>是否為 F5 新增訂單模式（用於底部 F5 按鈕圖示：true 顯示 add，false 顯示 edit）。</summary>
    [ObservableProperty]
    private bool _isF5AddOrderMode;

    /// <summary>目前編輯中的訂單 Id（0 表示新增）。</summary>
    private int _currentOrderId;
    private DateTime _currentCreatedAt;

    [ObservableProperty]
    private int _currentPageIndex;

    [ObservableProperty]
    private int _totalPageCount = 1;

    public ObservableCollection<Order> OrdersPage { get; } = new();
    [ObservableProperty] private Order? _selectedOrder;

    public LocalizedString Title { get; }
    public LocalizedString VersionNoLabel { get; }
    public LocalizedString F6Search { get; }
    public LocalizedString VersionNoQueryLabel { get; }
    public LocalizedString PreviewLabel { get; }
    public LocalizedString SubmitVersionPositionLabel { get; }
    public LocalizedString OrderNoLabel { get; }
    public LocalizedString OrderQuantityLabel { get; }
    public LocalizedString CategoryLabel { get; }
    public LocalizedString BoxTypeLabel { get; }
    /// <summary>箱型 Radio：E（與 DB 代碼一致，可於 RESX 調整顯示）。</summary>
    public LocalizedString BoxTypeRadioE { get; }
    /// <summary>箱型 Radio：S。</summary>
    public LocalizedString BoxTypeRadioS { get; }
    public LocalizedString CustomerNameLabel { get; }
    public LocalizedString RemarksLabel { get; }
    public LocalizedString LengthLabel { get; }
    public LocalizedString WidthLabel { get; }
    public LocalizedString PhaseLabel { get; }
    public LocalizedString Phase1ColorLabel { get; }
    public LocalizedString Phase2ColorLabel { get; }
    public LocalizedString Phase3ColorLabel { get; }
    public LocalizedString F4Delete { get; }
    public LocalizedString F5CompleteEdit { get; }
    public LocalizedString F5AddOrder { get; }
    public LocalizedString F2AddToSchedule { get; }
    public LocalizedString F1Exit { get; }
    public LocalizedString DialogConfirmNewVersion { get; }
    public LocalizedString DialogOrderSaved { get; }
    /// <summary>4.4 右側 DataGrid 欄位標題（多語系）。</summary>
    public LocalizedString ColVersionNo { get; }
    public LocalizedString ColCustomerName { get; }
    public LocalizedString ColBoxType { get; }
    public LocalizedString ColCategory { get; }
    public LocalizedString ColCreatedAt { get; }

    /// <summary>楞別選項（A, AB, B, BC, C, E）。</summary>
    public static IReadOnlyList<string> CategoryOptions { get; } = new[] { "A", "AB", "B", "BC", "C", "E" };

    /// <summary>僅在「版號搜尋」或「版號查詢」設定選取後發出，供 View 捲動至該列中央；手動點選列時不發出。</summary>
    public event Action? RequestScrollToSelected;

    public OrderMakingDialogViewModel(
        IOrderRepository orderRepo,
        IProductionScheduleService scheduleService,
        ILocalizationService localizationService,
        ILogService logService,
        INavigationService? navigationService = null)
    {
        _orderRepo = orderRepo ?? throw new ArgumentNullException(nameof(orderRepo));
        _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
        _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _navigationService = navigationService;

        Title = new LocalizedString(_localizationService, "OrderMakingDialog_Title");
        VersionNoLabel = new LocalizedString(_localizationService, "OrderMakingDialog_VersionNo");
        F6Search = new LocalizedString(_localizationService, "OrderMakingDialog_F6Search");
        VersionNoQueryLabel = new LocalizedString(_localizationService, "OrderMakingDialog_VersionNoQuery");
        PreviewLabel = new LocalizedString(_localizationService, "OrderMakingDialog_Preview");
        SubmitVersionPositionLabel = new LocalizedString(_localizationService, "OrderMakingDialog_SubmitVersionPosition");
        OrderNoLabel = new LocalizedString(_localizationService, "Layout2_OrderNo");
        OrderQuantityLabel = new LocalizedString(_localizationService, "Layout2_OrderQuantity");
        CategoryLabel = new LocalizedString(_localizationService, "Layout2_Category");
        BoxTypeLabel = new LocalizedString(_localizationService, "Layout2_BoxType");
        BoxTypeRadioE = new LocalizedString(_localizationService, "OrderMakingDialog_BoxTypeE");
        BoxTypeRadioS = new LocalizedString(_localizationService, "OrderMakingDialog_BoxTypeS");
        CustomerNameLabel = new LocalizedString(_localizationService, "Layout2_CustomerName");
        RemarksLabel = new LocalizedString(_localizationService, "Layout2_Remarks");
        LengthLabel = new LocalizedString(_localizationService, "OrderMakingDialog_LengthLabel");
        WidthLabel = new LocalizedString(_localizationService, "OrderMakingDialog_WidthLabel");
        PhaseLabel = new LocalizedString(_localizationService, "OrderMakingDialog_Phase");
        Phase1ColorLabel = new LocalizedString(_localizationService, "OrderMakingDialog_Phase1Color");
        Phase2ColorLabel = new LocalizedString(_localizationService, "OrderMakingDialog_Phase2Color");
        Phase3ColorLabel = new LocalizedString(_localizationService, "OrderMakingDialog_Phase3Color");
        F4Delete = new LocalizedString(_localizationService, "OrderMakingDialog_F4Delete");
        F5CompleteEdit = new LocalizedString(_localizationService, "OrderMakingDialog_F5CompleteEdit");
        F5AddOrder = new LocalizedString(_localizationService, "OrderMakingDialog_F5AddOrder");
        F2AddToSchedule = new LocalizedString(_localizationService, "OrderMakingDialog_F2AddToSchedule");
        F1Exit = new LocalizedString(_localizationService, "OrderMakingDialog_F1Exit");
        DialogConfirmNewVersion = new LocalizedString(_localizationService, "OrderMakingDialog_ConfirmNewVersion");
        DialogOrderSaved = new LocalizedString(_localizationService, "OrderMakingDialog_OrderSaved");
        ColVersionNo = new LocalizedString(_localizationService, "Layout2_VersionNo");
        ColCustomerName = new LocalizedString(_localizationService, "Layout2_CustomerName");
        ColBoxType = new LocalizedString(_localizationService, "Layout2_BoxType");
        ColCategory = new LocalizedString(_localizationService, "Layout2_Category");
        ColCreatedAt = new LocalizedString(_localizationService, "OrderMakingDialog_ColCreatedAt");

        F5ButtonText = F5CompleteEdit.Value;
        IsF5AddOrderMode = false;
        SetInitialState();
        RefreshOrdersPage();
    }

    private void SetInitialState()
    {
        IsLeftAndBottomEnabled = false;
        IsF4Enabled = false;
        IsF5Enabled = false;
        IsF2Enabled = false;
        F5ButtonText = F5CompleteEdit.Value;
        IsF5AddOrderMode = false;
        _currentOrderId = 0;
    }

    /// <summary>4.4 不分頁：載入全部訂單，DataGrid 以捲軸顯示。</summary>
    private void RefreshOrdersPage()
    {
        OrdersPage.Clear();
        foreach (var o in _orderRepo.GetAll())
            OrdersPage.Add(o);
    }

    [RelayCommand]
    private void SearchByVersionNo()
    {
        var v = VersionNoSearch?.Trim() ?? "";
        if (string.IsNullOrEmpty(v))
        {
            _logService.Append("請輸入版號後按 F6 搜尋");
            return;
        }
        var order = _orderRepo.FindByVersionNo(v);
        if (order != null)
        {
            LoadOrderToForm(order);
            IsLeftAndBottomEnabled = true;
            IsF4Enabled = true;
            IsF5Enabled = true;
            IsF2Enabled = true;
            F5ButtonText = F5CompleteEdit.Value;
            IsF5AddOrderMode = false;
            _currentOrderId = order.Id;
            // 定位到右側數據區該筆並選取，並請 View 捲動至中央
            foreach (var o in OrdersPage)
            {
                if (o.Id == order.Id) { SelectedOrder = o; break; }
            }
            RequestScrollToSelected?.Invoke();
            _logService.Append($"已找到版號「{v}」並帶出訂單");
        }
        else
        {
            var confirm = MessageBox.Show(
                DialogConfirmNewVersion.Value,
                Title.Value,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (confirm == MessageBoxResult.Yes)
            {
                VersionNoSearch = v;
                OrderNo = "";
                OrderQuantity = 0;
                BoxType = "E";
                Category = "A";
                CustomerName = "";
                Remarks = "";
                Phase1 = "";
                Phase2 = "";
                Phase3 = "";
                Length = 0;
                Width = 0;
                IsLeftAndBottomEnabled = true;
                IsF5Enabled = true;
                IsF2Enabled = true;
                IsF4Enabled = false;
                F5ButtonText = F5AddOrder.Value;
                IsF5AddOrderMode = true;
                _currentOrderId = 0;
                _logService.Append($"新增模式：版號「{v}」");
            }
        }
    }

    [RelayCommand]
    private void QueryVersionNoInGrid()
    {
        var v = VersionNoQuery?.Trim() ?? "";
        if (string.IsNullOrEmpty(v)) return;
        for (int i = 0; i < OrdersPage.Count; i++)
        {
            if (string.Equals(OrdersPage[i].VersionNo, v, StringComparison.OrdinalIgnoreCase))
            {
                SelectedOrder = OrdersPage[i];
                RequestScrollToSelected?.Invoke();
                return;
            }
        }
        var found = _orderRepo.FindByVersionNo(v);
        if (found != null)
        {
            RefreshOrdersPage();
            for (int i = 0; i < OrdersPage.Count; i++)
            {
                if (OrdersPage[i].Id == found.Id) { SelectedOrder = OrdersPage[i]; break; }
            }
            RequestScrollToSelected?.Invoke();
        }
    }

    [RelayCommand]
    private void PreviewSelected()
    {
        if (SelectedOrder == null) return;
        LoadOrderToForm(SelectedOrder);
        IsLeftAndBottomEnabled = true;
        IsF4Enabled = true;
        IsF5Enabled = true;
        IsF2Enabled = true;
        F5ButtonText = F5CompleteEdit.Value;
        IsF5AddOrderMode = false;
        _currentOrderId = SelectedOrder.Id;
    }

    [RelayCommand]
    private void SaveOrder()
    {
        var v = VersionNoSearch?.Trim() ?? "";
        if (string.IsNullOrEmpty(v))
        {
            MessageBox.Show("請先輸入版號。", Title.Value, MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        var order = new Order
        {
            Id = _currentOrderId,
            CreatedAt = _currentOrderId > 0 ? _currentCreatedAt : DateTime.Now,
            OrderNo = OrderNo?.Trim() ?? "",
            VersionNo = v,
            OrderQuantity = OrderQuantity,
            BoxType = BoxType ?? "E",
            Category = Category ?? "A",
            Phase1 = Phase1 ?? "",
            Phase2 = Phase2 ?? "",
            Phase3 = Phase3 ?? "",
            Length = Length,
            Width = Width,
            CustomerName = CustomerName?.Trim() ?? "",
            Remarks = Remarks?.Trim() ?? ""
        };
        if (_currentOrderId > 0)
        {
            _orderRepo.Update(order);
            _logService.Append($"已更新訂單版號「{v}」");
        }
        else
        {
            order.CreatedAt = DateTime.Now;
            _currentOrderId = _orderRepo.Insert(order);
            _logService.Append($"已新增訂單版號「{v}」");
        }
        MessageBox.Show(string.Format(DialogOrderSaved.Value, v), Title.Value, MessageBoxButton.OK);
        IsLeftAndBottomEnabled = false;
        IsF4Enabled = false;
        IsF5Enabled = false;
        IsF2Enabled = false;
        F5ButtonText = F5CompleteEdit.Value;
        IsF5AddOrderMode = false;
        _currentOrderId = 0;
        RefreshOrdersPage();
    }

    [RelayCommand]
    private void DeleteOrder()
    {
        if (_currentOrderId <= 0) return;
        _orderRepo.Delete(_currentOrderId);
        _logService.Append("已刪除該筆訂單");
        _currentOrderId = 0;
        ClearForm();
        IsLeftAndBottomEnabled = false;
        IsF4Enabled = false;
        IsF5Enabled = false;
        IsF2Enabled = false;
        F5ButtonText = F5CompleteEdit.Value;
        IsF5AddOrderMode = false;
        RefreshOrdersPage();
    }

    [RelayCommand]
    private void SubmitVersionPosition()
    {
        // 送入版號位置：後續 Phase 補齊功能，目前僅顯示按鈕
        _logService.Append("送入版號位置（功能待實作）");
    }

    [RelayCommand]
    private void AddToScheduleAndClose()
    {
        var item = new ScheduleOrderItem
        {
            OrderNo = OrderNo?.Trim() ?? "",
            VersionNo = VersionNoSearch?.Trim() ?? "",
            OrderQuantity = OrderQuantity,
            BoxType = BoxType ?? "E",
            Category = Category ?? "A",
            CustomerName = CustomerName?.Trim() ?? "",
            Remarks = Remarks?.Trim() ?? ""
        };
        _scheduleService.AddOrderToSchedule(item);
        _logService.Append($"已將版號「{item.VersionNo}」加入排程");
        _navigationService?.NavigateToLayout2();
    }

    [RelayCommand]
    private void Close()
    {
        _navigationService?.NavigateToLayout2();
    }

    /// <summary>上箭頭：選取「版號查詢」同版號的上一筆訂單，不循環。</summary>
    [RelayCommand]
    private void PrevPage()
    {
        var v = (VersionNoQuery?.Trim() ?? "").Trim();
        if (string.IsNullOrEmpty(v)) return;
        var sameVersion = new List<int>();
        for (int i = 0; i < OrdersPage.Count; i++)
        {
            if (string.Equals(OrdersPage[i].VersionNo, v, StringComparison.OrdinalIgnoreCase))
                sameVersion.Add(i);
        }
        if (SelectedOrder == null || sameVersion.Count == 0) return;
        int currentIdx = OrdersPage.IndexOf(SelectedOrder);
        int pos = sameVersion.IndexOf(currentIdx);
        if (pos <= 0) return;
        SelectedOrder = OrdersPage[sameVersion[pos - 1]];
    }

    /// <summary>下箭頭：選取「版號查詢」同版號的下一筆訂單，不循環。</summary>
    [RelayCommand]
    private void NextPage()
    {
        var v = (VersionNoQuery?.Trim() ?? "").Trim();
        if (string.IsNullOrEmpty(v)) return;
        var sameVersion = new List<int>();
        for (int i = 0; i < OrdersPage.Count; i++)
        {
            if (string.Equals(OrdersPage[i].VersionNo, v, StringComparison.OrdinalIgnoreCase))
                sameVersion.Add(i);
        }
        if (SelectedOrder == null || sameVersion.Count == 0) return;
        int currentIdx = OrdersPage.IndexOf(SelectedOrder);
        int pos = sameVersion.IndexOf(currentIdx);
        if (pos < 0 || pos >= sameVersion.Count - 1) return;
        SelectedOrder = OrdersPage[sameVersion[pos + 1]];
    }

    private void LoadOrderToForm(Order order)
    {
        _currentCreatedAt = order.CreatedAt;
        VersionNoSearch = order.VersionNo;
        OrderNo = order.OrderNo;
        OrderQuantity = order.OrderQuantity;
        BoxType = order.BoxType;
        Category = order.Category;
        CustomerName = order.CustomerName;
        Remarks = order.Remarks;
        Phase1 = order.Phase1;
        Phase2 = order.Phase2;
        Phase3 = order.Phase3;
        Length = order.Length;
        Width = order.Width;
        _currentOrderId = order.Id;
    }

    private void ClearForm()
    {
        OrderNo = "";
        OrderQuantity = 0;
        BoxType = "E";
        Category = "A";
        CustomerName = "";
        Remarks = "";
        Phase1 = "";
        Phase2 = "";
        Phase3 = "";
        Length = 0;
        Width = 0;
    }
}
