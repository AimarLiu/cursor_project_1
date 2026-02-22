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
    /// 建立 DatabaseService，預設使用應用程式目錄下的 app.db。
    /// </summary>
    /// <param name="databasePath">可選，自訂資料庫檔案路徑</param>
    public DatabaseService(string? databasePath = null)
    {
        _databasePath = databasePath ?? Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
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

        SeedTestAccountIfEmpty(connection);
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
