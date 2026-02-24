using System.Globalization;
using CursorTestApp.Models;
using Microsoft.Data.Sqlite;

namespace CursorTestApp.Services;

/// <summary>
/// 自 SQLite 載入生產排程與完成訂單。
/// </summary>
public sealed class ScheduleOrderRepository : IScheduleOrderRepository
{
    private readonly string _databasePath;

    public ScheduleOrderRepository(IDatabaseService databaseService)
    {
        _databasePath = databaseService?.DatabasePath ?? throw new ArgumentNullException(nameof(databaseService));
    }

    /// <inheritdoc />
    public IReadOnlyList<ScheduleOrderItem> LoadScheduleOrders()
    {
        var list = new List<ScheduleOrderItem>();
        using var conn = new SqliteConnection($"Data Source={_databasePath}");
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT OrderNo, VersionNo, OrderQuantity, BoxType, Category, CustomerName, Remarks FROM ScheduleOrders ORDER BY SortOrder, Id";
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new ScheduleOrderItem
            {
                OrderNo = r.GetString(0),
                VersionNo = r.GetString(1),
                OrderQuantity = r.GetInt32(2),
                BoxType = r.GetString(3),
                Category = r.GetString(4),
                CustomerName = r.GetString(5),
                Remarks = r.IsDBNull(6) ? "" : r.GetString(6)
            });
        }
        return list;
    }

    /// <inheritdoc />
    public IReadOnlyList<CompletedOrderItem> LoadCompletedOrders()
    {
        var list = new List<CompletedOrderItem>();
        using var conn = new SqliteConnection($"Data Source={_databasePath}");
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT CompletedAt, OrderNo, VersionNo, OrderQuantity, BoxType, Category, CustomerName, Remarks FROM CompletedOrders ORDER BY CompletedAt DESC";
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            var completedAt = DateTime.TryParse(r.GetString(0), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt) ? dt : DateTime.MinValue;
            list.Add(new CompletedOrderItem
            {
                CompletedAt = completedAt,
                OrderNo = r.GetString(1),
                VersionNo = r.GetString(2),
                OrderQuantity = r.GetInt32(3),
                BoxType = r.GetString(4),
                Category = r.GetString(5),
                CustomerName = r.GetString(6),
                Remarks = r.IsDBNull(7) ? "" : r.GetString(7)
            });
        }
        return list;
    }
}
