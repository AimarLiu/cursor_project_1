using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CursorTestApp.ViewModels;

namespace CursorTestApp.Views.Layout2;

/// <summary>
/// Phase 6 訂單製作主內容區頁面（與 Login、Layout2 同，顯示於 Shell ContentHost）。
/// </summary>
public partial class OrderMakingView : UserControl
{
    public OrderMakingView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not OrderMakingDialogViewModel vm) return;

        // 4.4 右側 DataGrid 欄位標題由 ViewModel 多語系字串填入
        ColVersionNo.Header = vm.ColVersionNo.Value;
        ColCustomerName.Header = vm.ColCustomerName.Value;
        ColBoxType.Header = vm.ColBoxType.Value;
        ColCategory.Header = vm.ColCategory.Value;
        ColCreatedAt.Header = vm.ColCreatedAt.Value;

        // 僅在「版號搜尋」或「版號查詢」觸發時捲動至中央，手動點選列不捲動
        vm.RequestScrollToSelected += () => Dispatcher.BeginInvoke(() => ScrollSelectedToCenter());
    }

    /// <summary>將選取列捲動到右側數據區可見範圍的中間。</summary>
    private void ScrollSelectedToCenter()
    {
        if (OrdersGrid.SelectedItem == null) return;
        OrdersGrid.ScrollIntoView(OrdersGrid.SelectedItem);
        var sv = FindVisualChild<ScrollViewer>(OrdersGrid);
        if (sv == null) return;
        var row = OrdersGrid.ItemContainerGenerator.ContainerFromItem(OrdersGrid.SelectedItem) as DataGridRow;
        if (row == null) return;
        row.BringIntoView();
        var rowOffsetInViewport = row.TransformToAncestor(sv).Transform(new Point(0, 0)).Y;
        var rowHalf = row.ActualHeight * 0.5;
        var viewportHalf = sv.ViewportHeight * 0.5;
        var delta = rowOffsetInViewport + rowHalf - viewportHalf;
        var newOffset = Math.Clamp(sv.VerticalOffset + delta, 0, Math.Max(0, sv.ExtentHeight - sv.ViewportHeight));
        sv.ScrollToVerticalOffset(newOffset);
    }

    private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T t) return t;
            var found = FindVisualChild<T>(child);
            if (found != null) return found;
        }
        return null;
    }

    private void VersionNoSearch_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        if (DataContext is OrderMakingDialogViewModel vm)
            vm.SearchByVersionNoCommand.Execute(null);
    }

    private void VersionNoQuery_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        if (DataContext is OrderMakingDialogViewModel vm)
            vm.QueryVersionNoInGridCommand.Execute(null);
    }

    private void OrdersGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is OrderMakingDialogViewModel vm)
            vm.PreviewSelectedCommand.Execute(null);
    }
}
