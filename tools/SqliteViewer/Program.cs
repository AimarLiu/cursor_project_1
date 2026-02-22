using Microsoft.Data.Sqlite;

namespace SqliteViewer;

/// <summary>
/// SQLite 資料庫檢視小工具：指定 .db 檔路徑後瀏覽其表與資料。
/// </summary>
/// <example>
/// dotnet run -- path/to/app.db
/// dotnet run -- "C:\path\to\app.db"
/// </example>
internal static class Program
{
    static void Main(string[] args)
    {
        string dbPath = args.Length > 0 ? args[0].Trim().Trim('"') : PromptForPath();
        if (string.IsNullOrWhiteSpace(dbPath))
        {
            Console.WriteLine("未指定資料庫路徑。");
            return;
        }

        if (!File.Exists(dbPath))
        {
            Console.WriteLine($"找不到檔案：{dbPath}");
            return;
        }

        try
        {
            BrowseDatabase(dbPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"錯誤：{ex.Message}");
        }
    }

    static string PromptForPath()
    {
        Console.Write("請輸入 .db 檔路徑：");
        return Console.ReadLine() ?? "";
    }

    static void BrowseDatabase(string dbPath)
    {
        Console.WriteLine($"\n=== 資料庫：{dbPath} ===\n");

        using var conn = new SqliteConnection($"Data Source={dbPath}");
        conn.Open();

        var tables = GetTableNames(conn);
        if (tables.Count == 0)
        {
            Console.WriteLine("（無任何表）");
            return;
        }

        Console.WriteLine($"找到 {tables.Count} 個表：\n");

        foreach (var tableName in tables)
        {
            DisplayTable(conn, tableName);
        }
    }

    static List<string> GetTableNames(SqliteConnection conn)
    {
        var list = new List<string>();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT name FROM sqlite_master
            WHERE type = 'table' AND name NOT LIKE 'sqlite_%'
            ORDER BY name
            """;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(reader.GetString(0));
        }
        return list;
    }

    static void DisplayTable(SqliteConnection conn, string tableName)
    {
        Console.WriteLine($"--- 表：{tableName} ---");

        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM [{tableName}]";
        using var reader = cmd.ExecuteReader();

        if (!reader.HasRows)
        {
            Console.WriteLine("（無資料）\n");
            return;
        }

        // 欄位名稱
        var columns = new List<string>();
        for (int i = 0; i < reader.FieldCount; i++)
            columns.Add(reader.GetName(i));

        Console.WriteLine(string.Join(" | ", columns));
        Console.WriteLine(new string('-', Math.Min(80, columns.Sum(c => c.Length + 3))));

        int rowCount = 0;
        while (reader.Read())
        {
            var values = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var val = reader.IsDBNull(i) ? "(null)" : reader.GetValue(i)?.ToString() ?? "";
                values.Add(val);
            }
            Console.WriteLine(string.Join(" | ", values));
            rowCount++;
        }
        Console.WriteLine($"共 {rowCount} 筆\n");
    }
}
