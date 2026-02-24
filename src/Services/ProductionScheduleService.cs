using System.Collections.ObjectModel;
using CursorTestApp.Models;

namespace CursorTestApp.Services;

/// <summary>
/// 生產排程與完成訂單操作實作（記憶體）。
/// </summary>
public sealed class ProductionScheduleService : IProductionScheduleService
{
    /// <inheritdoc />
    public ObservableCollection<ScheduleOrderItem> ScheduleOrders { get; } = new();
    /// <inheritdoc />
    public ObservableCollection<CompletedOrderItem> CompletedOrders { get; } = new();

    /// <inheritdoc />
    public ScheduleOrderItem? FirstScheduleItem => ScheduleOrders.Count > 0 ? ScheduleOrders[0] : null;

    /// <summary>若傳入 repository 則自 DB 載入；否則使用記憶體並種子範例資料。</summary>
    public ProductionScheduleService(IScheduleOrderRepository? repository = null)
    {
        if (repository != null)
        {
            foreach (var item in repository.LoadScheduleOrders())
                ScheduleOrders.Add(item);
            foreach (var item in repository.LoadCompletedOrders())
                CompletedOrders.Add(item);
        }
        if (ScheduleOrders.Count == 0)
        {
            ScheduleOrders.Add(new ScheduleOrderItem { OrderNo = "0008", VersionNo = "20260218", OrderQuantity = 10, BoxType = "E", Category = "A", CustomerName = "Aimar", Remarks = "" });
            ScheduleOrders.Add(new ScheduleOrderItem { OrderNo = "0010", VersionNo = "20260220", OrderQuantity = 300, BoxType = "E", Category = "A", CustomerName = "Mason", Remarks = "" });
            ScheduleOrders.Add(new ScheduleOrderItem { OrderNo = "0011", VersionNo = "20260226", OrderQuantity = 800, BoxType = "E", Category = "A", CustomerName = "Jenny", Remarks = "" });
        }
        if (CompletedOrders.Count == 0)
        {
            CompletedOrders.Add(new CompletedOrderItem { CompletedAt = new DateTime(2026, 1, 10, 13, 1, 12), OrderNo = "0004", VersionNo = "20260110", OrderQuantity = 600, BoxType = "E", Category = "A", CustomerName = "Aimar", Remarks = "" });
            CompletedOrders.Add(new CompletedOrderItem { CompletedAt = new DateTime(2026, 2, 11, 11, 52, 42), OrderNo = "0005", VersionNo = "20260110", OrderQuantity = 199, BoxType = "E", Category = "A", CustomerName = "Mason", Remarks = "" });
        }
    }

    /// <inheritdoc />
    public void MoveSelectedToCompleted(ScheduleOrderItem? item)
    {
        if (item == null) return;
        int idx = ScheduleOrders.IndexOf(item);
        if (idx < 0) return;
        ScheduleOrders.RemoveAt(idx);
        var completed = new CompletedOrderItem
        {
            CompletedAt = DateTime.Now,
            OrderNo = item.OrderNo,
            VersionNo = item.VersionNo,
            OrderQuantity = item.OrderQuantity,
            BoxType = item.BoxType,
            Category = item.Category,
            CustomerName = item.CustomerName,
            Remarks = item.Remarks
        };
        CompletedOrders.Add(completed);
    }

    /// <inheritdoc />
    public void RemoveSelectedScheduleItem(ScheduleOrderItem? item)
    {
        if (item != null && ScheduleOrders.Contains(item))
            ScheduleOrders.Remove(item);
    }

    /// <inheritdoc />
    public void MoveScheduleItemUp(ScheduleOrderItem? item)
    {
        if (item == null) return;
        int idx = ScheduleOrders.IndexOf(item);
        if (idx <= 0) return;
        ScheduleOrders.RemoveAt(idx);
        ScheduleOrders.Insert(idx - 1, item);
    }

    /// <inheritdoc />
    public void MoveScheduleItemDown(ScheduleOrderItem? item)
    {
        if (item == null) return;
        int idx = ScheduleOrders.IndexOf(item);
        if (idx < 0 || idx >= ScheduleOrders.Count - 1) return;
        ScheduleOrders.RemoveAt(idx);
        ScheduleOrders.Insert(idx + 1, item);
    }

    /// <inheritdoc />
    public int FindScheduleIndex(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return -1;
        var k = keyword.Trim();
        for (int i = 0; i < ScheduleOrders.Count; i++)
        {
            var o = ScheduleOrders[i];
            if (string.Equals(o.OrderNo, k, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(o.VersionNo, k, StringComparison.OrdinalIgnoreCase) ||
                (o.CustomerName?.Contains(k, StringComparison.OrdinalIgnoreCase) == true))
                return i;
        }
        return -1;
    }
}
