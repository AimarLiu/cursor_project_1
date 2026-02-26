using System.Windows;
using System.Windows.Controls;
using CursorTestApp.Models;
using CursorTestApp.ViewModels;

namespace CursorTestApp.Views.Layout2;

/// <summary>
/// Phase 5 排程管理子頁（排單管理）Dialog。F2/F3 關閉時帶回 SimulationMode，F1 離開為 null。
/// </summary>
public partial class ScheduleManageDialog : Window
{
    public ScheduleManageDialog(ScheduleManageDialogViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += OnLoaded;
        viewModel.RequestClose += (_, mode) =>
        {
            Tag = mode;
            DialogResult = mode.HasValue && mode != SimulationMode.None;
            Close();
        };
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not ScheduleManageDialogViewModel vm) return;
        DialogColOrderNo.Header = vm.OrderNoHeader.Value;
        DialogColVersionNo.Header = vm.VersionNoHeader.Value;
        DialogColOrderQuantity.Header = vm.OrderQuantityHeader.Value;
        DialogColBoxType.Header = vm.BoxTypeHeader.Value;
        DialogColCategory.Header = vm.CategoryHeader.Value;
        DialogColCustomerName.Header = vm.CustomerNameHeader.Value;
        DialogColRemarks.Header = vm.RemarksHeader.Value;
    }
}
