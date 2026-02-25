using System.IO;
using Microsoft.Data.Sqlite;

namespace CursorTestApp.Services;

/// <summary>
/// SQLite 資料庫服務實作，負責初始化與建表。
/// </summary>
public sealed class DatabaseService : IDatabaseService
{
    private const string CreateTableSql = """
        CREATE TABLE IF NOT EXISTS Users (
            Id TEXT PRIMARY KEY,
            Username TEXT NOT NULL UNIQUE,
            Password TEXT NOT NULL
        );
        """;

    /// <summary>
    /// 測試帳號：Id=0001, Username=aimarliu, Password=aimarliu
    /// </summary>
    private const string SeedUserId = "0001";
    private const string SeedUsername = "aimarliu";
    private const string SeedPassword = "aimarliu";

    private readonly string _databasePath;

    /// <inheritdoc />
    public string DatabasePath => _databasePath;

    /// <summary>
    /// 建立 DatabaseService。未指定路徑時使用 %LocalAppData%\{AppName}\app.db，避免單檔發佈在另一台電腦時 BaseDirectory 唯讀導致 crash。
    /// </summary>
    /// <param name="databasePath">可選，自訂資料庫檔案路徑</param>
    public DatabaseService(string? databasePath = null)
    {
        _databasePath = databasePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CursorTestApp",
            "app.db");
    }

    /// <inheritdoc />
    public void Initialize()
    {
        string directory = Path.GetDirectoryName(_databasePath) ?? _databasePath;
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using SqliteConnection connection = new($"Data Source={_databasePath}");
        connection.Open();

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = CreateTableSql;
            command.ExecuteNonQuery();
        }

        EnsureScheduleTables(connection);
        SeedScheduleDataIfEmpty(connection);
        SeedTestAccountIfEmpty(connection);
    }

    private static void SeedScheduleDataIfEmpty(SqliteConnection connection)
    {
        using var countCmd = connection.CreateCommand();
        countCmd.CommandText = "SELECT COUNT(*) FROM ScheduleOrders";
        if ((long)(countCmd.ExecuteScalar() ?? 0L) > 0) return;

        var inserts = new[]
        {
            "INSERT INTO ScheduleOrders (OrderNo, VersionNo, OrderQuantity, BoxType, Category, CustomerName, Remarks, SortOrder) VALUES ('0008','20260218',10,'E','A','Aimar','',0)",
            "INSERT INTO ScheduleOrders (OrderNo, VersionNo, OrderQuantity, BoxType, Category, CustomerName, Remarks, SortOrder) VALUES ('0010','20260220',300,'E','A','Mason','',1)",
            "INSERT INTO ScheduleOrders (OrderNo, VersionNo, OrderQuantity, BoxType, Category, CustomerName, Remarks, SortOrder) VALUES ('0011','20260226',800,'E','A','Jenny','',2)",
        };
        foreach (var sql in inserts)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        using var countCompleted = connection.CreateCommand();
        countCompleted.CommandText = "SELECT COUNT(*) FROM CompletedOrders";
        if ((long)(countCompleted.ExecuteScalar() ?? 0L) > 0) return;

        var completed = new[]
        {
            "INSERT INTO CompletedOrders (CompletedAt, OrderNo, VersionNo, OrderQuantity, BoxType, Category, CustomerName, Remarks) VALUES ('2026-01-10T13:01:12','0004','20260110',600,'E','A','Aimar','')",
            "INSERT INTO CompletedOrders (CompletedAt, OrderNo, VersionNo, OrderQuantity, BoxType, Category, CustomerName, Remarks) VALUES ('2026-02-11T11:52:42','0005','20260110',199,'E','A','Mason','')",
        };
        foreach (var sql in completed)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }

    private static void EnsureScheduleTables(SqliteConnection connection)
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS ScheduleOrders (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OrderNo TEXT NOT NULL,
                VersionNo TEXT NOT NULL,
                OrderQuantity INTEGER NOT NULL,
                BoxType TEXT NOT NULL,
                Category TEXT NOT NULL,
                CustomerName TEXT NOT NULL,
                Remarks TEXT NOT NULL,
                SortOrder INTEGER NOT NULL DEFAULT 0
            );
            CREATE TABLE IF NOT EXISTS CompletedOrders (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CompletedAt TEXT NOT NULL,
                OrderNo TEXT NOT NULL,
                VersionNo TEXT NOT NULL,
                OrderQuantity INTEGER NOT NULL,
                BoxType TEXT NOT NULL,
                Category TEXT NOT NULL,
                CustomerName TEXT NOT NULL,
                Remarks TEXT NOT NULL
            );
            CREATE INDEX IF NOT EXISTS IX_ScheduleOrders_SortOrder ON ScheduleOrders(SortOrder);
            CREATE INDEX IF NOT EXISTS IX_CompletedOrders_CompletedAt ON CompletedOrders(CompletedAt);
            """;
        foreach (var stmt in sql.Split(";", StringSplitOptions.RemoveEmptyEntries))
        {
            var t = stmt.Trim();
            if (string.IsNullOrEmpty(t)) continue;
            using var cmd = connection.CreateCommand();
            cmd.CommandText = t;
            cmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// 若 Users 表為空，則插入測試帳號。
    /// </summary>
    private static void SeedTestAccountIfEmpty(SqliteConnection connection)
    {
        using SqliteCommand countCmd = connection.CreateCommand();
        countCmd.CommandText = "SELECT COUNT(*) FROM Users";
        long count = (long)(countCmd.ExecuteScalar() ?? 0L);

        if (count > 0)
            return;

        using SqliteCommand insertCmd = connection.CreateCommand();
        insertCmd.CommandText = "INSERT INTO Users (Id, Username, Password) VALUES (@Id, @Username, @Password)";
        insertCmd.Parameters.AddWithValue("@Id", SeedUserId);
        insertCmd.Parameters.AddWithValue("@Username", SeedUsername);
        insertCmd.Parameters.AddWithValue("@Password", SeedPassword);
        insertCmd.ExecuteNonQuery();
    }
}
