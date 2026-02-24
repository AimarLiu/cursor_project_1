using CursorTestApp.Models;

namespace CursorTestApp.Services;

/// <summary>
/// 生產排程與完成訂單的資料庫讀取。
/// </summary>
public interface IScheduleOrderRepository
{
    /// <summary>自 DB 載入生產排程（依 SortOrder）。</summary>
    IReadOnlyList<ScheduleOrderItem> LoadScheduleOrders();
    /// <summary>自 DB 載入生產完成訂單（依 CompletedAt 降序）。</summary>
    IReadOnlyList<CompletedOrderItem> LoadCompletedOrders();
}
