using System.Globalization;
using CursorTestApp.Models;
using Microsoft.Data.Sqlite;

namespace CursorTestApp.Services;

/// <summary>
/// Phase 6 訂單製作：Orders 表 SQLite 實作。
/// </summary>
public sealed class OrderRepository : IOrderRepository
{
    private const int PageSize = 10;
    private readonly string _databasePath;

    public OrderRepository(IDatabaseService databaseService)
    {
        _databasePath = databaseService?.DatabasePath ?? throw new ArgumentNullException(nameof(databaseService));
    }

    /// <inheritdoc />
    public Order? FindByVersionNo(string versionNo)
    {
        if (string.IsNullOrWhiteSpace(versionNo)) return null;
        using var conn = new SqliteConnection($"Data Source={_databasePath}");
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, CreatedAt, OrderNo, VersionNo, OrderQuantity, BoxType, Category, Phase1, Phase2, Phase3, Length, Width, CustomerName, Remarks FROM Orders WHERE VersionNo = @v ORDER BY CreatedAt DESC LIMIT 1";
        cmd.Parameters.AddWithValue("@v", versionNo.Trim());
        using var r = cmd.ExecuteReader();
        return r.Read() ? ReadOrder(r) : null;
    }

    /// <inheritdoc />
    public IReadOnlyList<Order> GetPage(int pageIndex)
    {
        var list = new List<Order>();
        using var conn = new SqliteConnection($"Data Source={_databasePath}");
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, CreatedAt, OrderNo, VersionNo, OrderQuantity, BoxType, Category, Phase1, Phase2, Phase3, Length, Width, CustomerName, Remarks FROM Orders ORDER BY CreatedAt DESC LIMIT @limit OFFSET @offset";
        cmd.Parameters.AddWithValue("@limit", PageSize);
        cmd.Parameters.AddWithValue("@offset", pageIndex * PageSize);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add(ReadOrder(r));
        return list;
    }

    /// <inheritdoc />
    public IReadOnlyList<Order> GetAll()
    {
        var list = new List<Order>();
        using var conn = new SqliteConnection($"Data Source={_databasePath}");
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, CreatedAt, OrderNo, VersionNo, OrderQuantity, BoxType, Category, Phase1, Phase2, Phase3, Length, Width, CustomerName, Remarks FROM Orders ORDER BY CreatedAt DESC";
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add(ReadOrder(r));
        return list;
    }

    /// <inheritdoc />
    public int GetTotalCount()
    {
        using var conn = new SqliteConnection($"Data Source={_databasePath}");
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM Orders";
        return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
    }

    /// <inheritdoc />
    public int Insert(Order order)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));
        using var conn = new SqliteConnection($"Data Source={_databasePath}");
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO Orders (CreatedAt, OrderNo, VersionNo, OrderQuantity, BoxType, Category, Phase1, Phase2, Phase3, Length, Width, CustomerName, Remarks)
            VALUES (@createdAt, @orderNo, @versionNo, @orderQuantity, @boxType, @category, @phase1, @phase2, @phase3, @length, @width, @customerName, @remarks);
            SELECT last_insert_rowid();
            """;
        AddOrderParams(cmd, order);
        return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
    }

    /// <inheritdoc />
    public void Update(Order order)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));
        using var conn = new SqliteConnection($"Data Source={_databasePath}");
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            UPDATE Orders SET CreatedAt=@createdAt, OrderNo=@orderNo, VersionNo=@versionNo, OrderQuantity=@orderQuantity, BoxType=@boxType, Category=@category,
            Phase1=@phase1, Phase2=@phase2, Phase3=@phase3, Length=@length, Width=@width, CustomerName=@customerName, Remarks=@remarks WHERE Id=@id
            """;
        cmd.Parameters.AddWithValue("@id", order.Id);
        AddOrderParams(cmd, order);
        cmd.ExecuteNonQuery();
    }

    /// <inheritdoc />
    public void Delete(int id)
    {
        using var conn = new SqliteConnection($"Data Source={_databasePath}");
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM Orders WHERE Id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    private static Order ReadOrder(SqliteDataReader r)
    {
        var createdAt = DateTime.TryParse(r.GetString(1), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt) ? dt : DateTime.MinValue;
        return new Order
        {
            Id = r.GetInt32(0),
            CreatedAt = createdAt,
            OrderNo = r.GetString(2),
            VersionNo = r.GetString(3),
            OrderQuantity = r.GetInt32(4),
            BoxType = r.GetString(5),
            Category = r.GetString(6),
            Phase1 = r.GetString(7),
            Phase2 = r.GetString(8),
            Phase3 = r.GetString(9),
            Length = r.GetInt32(10),
            Width = r.GetInt32(11),
            CustomerName = r.GetString(12),
            Remarks = r.IsDBNull(13) ? "" : r.GetString(13)
        };
    }

    private static void AddOrderParams(SqliteCommand cmd, Order order)
    {
        cmd.Parameters.AddWithValue("@createdAt", order.CreatedAt.ToString("O"));
        cmd.Parameters.AddWithValue("@orderNo", order.OrderNo ?? "");
        cmd.Parameters.AddWithValue("@versionNo", order.VersionNo ?? "");
        cmd.Parameters.AddWithValue("@orderQuantity", order.OrderQuantity);
        cmd.Parameters.AddWithValue("@boxType", order.BoxType ?? "");
        cmd.Parameters.AddWithValue("@category", order.Category ?? "");
        cmd.Parameters.AddWithValue("@phase1", order.Phase1 ?? "");
        cmd.Parameters.AddWithValue("@phase2", order.Phase2 ?? "");
        cmd.Parameters.AddWithValue("@phase3", order.Phase3 ?? "");
        cmd.Parameters.AddWithValue("@length", order.Length);
        cmd.Parameters.AddWithValue("@width", order.Width);
        cmd.Parameters.AddWithValue("@customerName", order.CustomerName ?? "");
        cmd.Parameters.AddWithValue("@remarks", order.Remarks ?? "");
    }
}
