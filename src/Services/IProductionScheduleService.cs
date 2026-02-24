using System.Collections.ObjectModel;
using CursorTestApp.Models;

namespace CursorTestApp.Services;

/// <summary>
/// 生產排程與完成訂單操作介面。
/// </summary>
public interface IProductionScheduleService
{
    /// <summary>生產排程清單（上區 DataGrid）</summary>
    ObservableCollection<ScheduleOrderItem> ScheduleOrders { get; }
    /// <summary>生產完成訂單清單（下區 DataGrid）</summary>
    ObservableCollection<CompletedOrderItem> CompletedOrders { get; }

    /// <summary>將所選排程項移入完成訂單並自排程移除。</summary>
    void MoveSelectedToCompleted(ScheduleOrderItem? item);
    /// <summary>刪除所選排程項，下方項目上移。</summary>
    void RemoveSelectedScheduleItem(ScheduleOrderItem? item);
    /// <summary>所選排程項與上一筆對調。</summary>
    void MoveScheduleItemUp(ScheduleOrderItem? item);
    /// <summary>所選排程項與下一筆對調。</summary>
    void MoveScheduleItemDown(ScheduleOrderItem? item);
    /// <summary>依關鍵字搜尋排程，回傳符合的第一筆索引，找不到回傳 -1。</summary>
    int FindScheduleIndex(string keyword);
    /// <summary>取得排程第一筆（供生產資訊看板綁定）。</summary>
    ScheduleOrderItem? FirstScheduleItem { get; }
}
