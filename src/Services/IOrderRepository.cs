using CursorTestApp.Models;

namespace CursorTestApp.Services;

/// <summary>
/// Phase 6 訂單製作：Orders 表查詢、新增、更新、刪除。
/// </summary>
public interface IOrderRepository
{
    /// <summary>依版號查詢，若有重複取最新一筆（CreatedAt 降序）。</summary>
    Order? FindByVersionNo(string versionNo);
    /// <summary>分頁取得訂單列表（每頁 10 筆，依 CreatedAt 降序）。</summary>
    IReadOnlyList<Order> GetPage(int pageIndex);
    /// <summary>取得全部訂單（依 CreatedAt 降序），供 4.4 不分頁捲動列表。</summary>
    IReadOnlyList<Order> GetAll();
    /// <summary>總筆數。</summary>
    int GetTotalCount();
    /// <summary>新增一筆訂單，回傳新 Id。</summary>
    int Insert(Order order);
    /// <summary>更新一筆訂單。</summary>
    void Update(Order order);
    /// <summary>依 Id 刪除一筆。</summary>
    void Delete(int id);
}
