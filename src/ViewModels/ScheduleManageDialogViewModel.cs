using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CursorTestApp.Helpers;
using CursorTestApp.Models;
using CursorTestApp.Services;

namespace CursorTestApp.ViewModels;

/// <summary>
/// Phase 5 排程管理子頁（排單管理）Dialog 的 ViewModel。
/// </summary>
public sealed partial class ScheduleManageDialogViewModel : ViewModelBase
{
    private readonly IProductionScheduleService _scheduleService;
    private readonly ILocalizationService _localizationService;

    /// <summary>關閉時帶回的模式（F2/F3 為小量/全量，F1 離開為 null）。</summary>
    public event EventHandler<SimulationMode?>? RequestClose;

    public ObservableCollection<ScheduleOrderItem> ScheduleOrders => _scheduleService.ScheduleOrders;
    public ScheduleOrderItem? FirstScheduleItem => _scheduleService.FirstScheduleItem;

    public LocalizedString Title { get; }
    public LocalizedString VersionLabel { get; }
    public LocalizedString OrderNoHeader { get; }
    public LocalizedString VersionNoHeader { get; }
    public LocalizedString OrderQuantityHeader { get; }
    public LocalizedString BoxTypeHeader { get; }
    public LocalizedString CategoryHeader { get; }
    public LocalizedString CustomerNameHeader { get; }
    public LocalizedString RemarksHeader { get; }
    public LocalizedString F4Print { get; }
    public LocalizedString F2PreSchedule { get; }
    public LocalizedString F3FullSchedule { get; }
    public LocalizedString F1Exit { get; }

    public ScheduleManageDialogViewModel(
        IProductionScheduleService scheduleService,
        ILocalizationService localizationService)
    {
        _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
        _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));

        Title = new LocalizedString(_localizationService, "ScheduleManageDialog_Title");
        VersionLabel = new LocalizedString(_localizationService, "ScheduleManageDialog_VersionLabel");
        OrderNoHeader = new LocalizedString(_localizationService, "Layout2_OrderNo");
        VersionNoHeader = new LocalizedString(_localizationService, "Layout2_VersionNo");
        OrderQuantityHeader = new LocalizedString(_localizationService, "Layout2_OrderQuantity");
        BoxTypeHeader = new LocalizedString(_localizationService, "Layout2_BoxType");
        CategoryHeader = new LocalizedString(_localizationService, "Layout2_Category");
        CustomerNameHeader = new LocalizedString(_localizationService, "Layout2_CustomerName");
        RemarksHeader = new LocalizedString(_localizationService, "Layout2_Remarks");
        F4Print = new LocalizedString(_localizationService, "ScheduleManageDialog_F4Print");
        F2PreSchedule = new LocalizedString(_localizationService, "ScheduleManageDialog_F2PreSchedule");
        F3FullSchedule = new LocalizedString(_localizationService, "ScheduleManageDialog_F3FullSchedule");
        F1Exit = new LocalizedString(_localizationService, "ScheduleManageDialog_F1Exit");
    }

    [RelayCommand]
    private void F1ExitDialog() => RequestClose?.Invoke(this, null);

    [RelayCommand]
    private void F2PreScheduleDialog() => RequestClose?.Invoke(this, SimulationMode.SmallBatch);

    [RelayCommand]
    private void F3FullScheduleDialog() => RequestClose?.Invoke(this, SimulationMode.FullBatch);

    [RelayCommand]
    private void F4PrintDialog()
    {
        // 後續 Phase 補齊，暫無功能
    }
}
