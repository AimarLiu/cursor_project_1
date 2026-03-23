using System.IO;
using CursorTestApp.Models;
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
        EnsureOrdersTable(connection);
        SeedOrdersIfEmpty(connection);
        SeedTestAccountIfEmpty(connection);
        EnsureAdminUserIfMissing(connection);
        EnsurePlcParameterTables(connection);
        SeedKnifeSafetyTab1IfNeeded(connection);
        EnsureTab2CatalogTables(connection);
        SeedTab2CatalogAndPlcParameters(connection);
        Tab3DatabaseBootstrap.EnsureAuxiliaryTables(connection);
        Tab3DatabaseBootstrap.SeedCommChannelsIfMissing(connection);
        Tab3DatabaseBootstrap.SeedDefinitionsAndValues(connection);
    }

    /// <summary>
    /// Phase 7：若尚無 Admin 帳號則插入（Id=0002, Admin/Admin）。
    /// </summary>
    private static void EnsureAdminUserIfMissing(SqliteConnection connection)
    {
        using SqliteCommand countCmd = connection.CreateCommand();
        countCmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = 'Admin'";
        if ((long)(countCmd.ExecuteScalar() ?? 0L) > 0)
            return;

        using SqliteCommand insertCmd = connection.CreateCommand();
        insertCmd.CommandText = "INSERT INTO Users (Id, Username, Password) VALUES (@Id, @Username, @Password)";
        insertCmd.Parameters.AddWithValue("@Id", "0002");
        insertCmd.Parameters.AddWithValue("@Username", "Admin");
        insertCmd.Parameters.AddWithValue("@Password", "Admin");
        insertCmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Phase 7 Tab1：PLC 參數表（§7.5.5 精簡版，先支援刀位閾值）。
    /// </summary>
    private static void EnsurePlcParameterTables(SqliteConnection connection)
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS PlcParameterDefinitions (
                ParameterDefinitionId INTEGER PRIMARY KEY AUTOINCREMENT,
                Key TEXT NOT NULL UNIQUE,
                TabGroup TEXT NOT NULL,
                ValueKind TEXT NOT NULL,
                DefaultInt INTEGER,
                DefaultReal REAL,
                SortOrder INTEGER NOT NULL DEFAULT 0
            );
            CREATE TABLE IF NOT EXISTS PlcParameterSetProfiles (
                ProfileId INTEGER PRIMARY KEY AUTOINCREMENT,
                ProfileName TEXT NOT NULL,
                BoxTypeId INTEGER,
                CorrugatedTypeId INTEGER
            );
            CREATE TABLE IF NOT EXISTS PlcParameterValues (
                ProfileId INTEGER NOT NULL,
                ParameterDefinitionId INTEGER NOT NULL,
                ValueInt INTEGER,
                ValueReal REAL,
                ValueBool INTEGER,
                ValueText TEXT,
                UpdatedAt TEXT,
                PRIMARY KEY (ProfileId, ParameterDefinitionId),
                FOREIGN KEY (ProfileId) REFERENCES PlcParameterSetProfiles(ProfileId),
                FOREIGN KEY (ParameterDefinitionId) REFERENCES PlcParameterDefinitions(ParameterDefinitionId)
            );
            CREATE UNIQUE INDEX IF NOT EXISTS IX_PlcParameterValues_Profile_Param
                ON PlcParameterValues(ProfileId, ParameterDefinitionId);
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
    /// 種子：Tab1 <c>KnifeSlot_*</c> 定義與預設值（§7.5.1）；ProfileId=1 全域刀位設定。
    /// </summary>
    private static void SeedKnifeSafetyTab1IfNeeded(SqliteConnection connection)
    {
        const int profileId = KnifeSafetyParameterRepository.KnifeProfileId;

        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = """
                INSERT OR IGNORE INTO PlcParameterSetProfiles (ProfileId, ProfileName, BoxTypeId, CorrugatedTypeId)
                VALUES (@id, 'KnifeSafetyGlobal', NULL, NULL)
                """;
            cmd.Parameters.AddWithValue("@id", profileId);
            cmd.ExecuteNonQuery();
        }

        int sort = 100;
        foreach (string key in KnifeSafetyParameterKeys.AllKeys)
        {
            using var insDef = connection.CreateCommand();
            insDef.CommandText = """
                INSERT OR IGNORE INTO PlcParameterDefinitions (Key, TabGroup, ValueKind, DefaultInt, SortOrder)
                VALUES (@k, 'KnifeSafety', 'INT', @d, @s)
                """;
            insDef.Parameters.AddWithValue("@k", key);
            object? defBox = DefaultIntForKnifeKey(key);
            insDef.Parameters.AddWithValue("@d", defBox ?? (object)DBNull.Value);
            insDef.Parameters.AddWithValue("@s", sort++);
            insDef.ExecuteNonQuery();
        }

        using (var seedVals = connection.CreateCommand())
        {
            seedVals.CommandText = """
                INSERT OR IGNORE INTO PlcParameterValues (ProfileId, ParameterDefinitionId, ValueInt, UpdatedAt)
                SELECT @pid, d.ParameterDefinitionId,
                    CASE d.Key
                        WHEN 'KnifeSlot_A_Min' THEN 750
                        WHEN 'KnifeSlot_A_Max' THEN 2700
                        WHEN 'KnifeSlot_B_Min' THEN 340
                        WHEN 'KnifeSlot_B_Max' THEN 1150
                        WHEN 'KnifeSlot_E_Min' THEN 220
                        WHEN 'KnifeSlot_J_Min' THEN 100
                        WHEN 'KnifeSlot_K_Min' THEN 220
                        WHEN 'KnifeSlot_F_Min' THEN 100
                        WHEN 'KnifeSlot_H_Min' THEN 0
                        WHEN 'KnifeSlot_H_Max' THEN 1272
                        WHEN 'KnifeSlot_I_Min' THEN 0
                        WHEN 'KnifeSlot_I_Max' THEN 1272
                        ELSE NULL
                    END,
                    datetime('now')
                FROM PlcParameterDefinitions d
                WHERE d.Key IN (
                    'KnifeSlot_A_Min','KnifeSlot_A_Max','KnifeSlot_B_Min','KnifeSlot_B_Max',
                    'KnifeSlot_E_Min','KnifeSlot_E_Max','KnifeSlot_J_Min','KnifeSlot_J_Max',
                    'KnifeSlot_K_Min','KnifeSlot_K_Max','KnifeSlot_F_Min','KnifeSlot_F_Max',
                    'KnifeSlot_H_Min','KnifeSlot_H_Max','KnifeSlot_I_Min','KnifeSlot_I_Max'
                )
                """;
            seedVals.Parameters.AddWithValue("@pid", profileId);
            seedVals.ExecuteNonQuery();
        }
    }

    private static object? DefaultIntForKnifeKey(string key) => key switch
    {
        KnifeSafetyParameterKeys.KnifeSlot_A_Min => 750,
        KnifeSafetyParameterKeys.KnifeSlot_A_Max => 2700,
        KnifeSafetyParameterKeys.KnifeSlot_B_Min => 340,
        KnifeSafetyParameterKeys.KnifeSlot_B_Max => 1150,
        KnifeSafetyParameterKeys.KnifeSlot_E_Min => 220,
        KnifeSafetyParameterKeys.KnifeSlot_J_Min => 100,
        KnifeSafetyParameterKeys.KnifeSlot_K_Min => 220,
        KnifeSafetyParameterKeys.KnifeSlot_F_Min => 100,
        KnifeSafetyParameterKeys.KnifeSlot_H_Min => 0,
        KnifeSafetyParameterKeys.KnifeSlot_H_Max => 1272,
        KnifeSafetyParameterKeys.KnifeSlot_I_Min => 0,
        KnifeSafetyParameterKeys.KnifeSlot_I_Max => 1272,
        _ => null,
    };

    /// <summary>
    /// Phase 7 Tab2：箱型、刀組合主檔、關聯表。
    /// </summary>
    private static void EnsureTab2CatalogTables(SqliteConnection connection)
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS CorrugatedTypes (
                CorrugatedTypeId INTEGER PRIMARY KEY,
                Code TEXT NOT NULL UNIQUE,
                LabelKey TEXT
            );
            CREATE TABLE IF NOT EXISTS BoxTypes (
                BoxTypeId INTEGER PRIMARY KEY,
                Code TEXT NOT NULL UNIQUE,
                KnifePullOut INTEGER NOT NULL,
                UseEquation2 INTEGER NOT NULL DEFAULT 0,
                FrontKnifePullOut INTEGER NOT NULL DEFAULT 0,
                BackKnifePullOut INTEGER NOT NULL DEFAULT 0,
                BothKnifePullOut INTEGER NOT NULL DEFAULT 0,
                UseDatabaseBlade INTEGER NOT NULL DEFAULT 0,
                AutoJudge INTEGER NOT NULL DEFAULT 0
            );
            CREATE TABLE IF NOT EXISTS KnifeCombinedOptions (
                KnifeCombinedOptionId INTEGER PRIMARY KEY,
                Code TEXT NOT NULL,
                DisplayTextKey TEXT NOT NULL,
                SortOrder INTEGER NOT NULL
            );
            CREATE TABLE IF NOT EXISTS KnifeCombinedOptionByBoxType (
                BoxTypeId INTEGER NOT NULL,
                KnifeCombinedOptionId INTEGER NOT NULL,
                PRIMARY KEY (BoxTypeId, KnifeCombinedOptionId)
            );
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
    /// Tab2 種子：CorrugatedTypes、BoxTypes、KnifeCombined*、Profile 2～5、<c>PlcParameterDefinitions</c>／<c>Values</c>（DieCutter／選刀）。
    /// </summary>
    private static void SeedTab2CatalogAndPlcParameters(SqliteConnection connection)
    {
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = """
                INSERT OR IGNORE INTO CorrugatedTypes (CorrugatedTypeId, Code, LabelKey)
                VALUES (0, 'Default', 'CorrugatedType_Default');
                """;
            cmd.ExecuteNonQuery();
        }

        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = """
                INSERT OR IGNORE INTO BoxTypes (BoxTypeId, Code, KnifePullOut, UseEquation2, FrontKnifePullOut, BackKnifePullOut, BothKnifePullOut, UseDatabaseBlade, AutoJudge)
                VALUES
                (1, 'C', -5, 0, 0, 0, 0, 0, 0),
                (2, 'E', -5, 0, 20, 20, 5, 0, 0),
                (3, 'DA7', 0, 0, 0, 0, 0, 0, 0);
                """;
            cmd.ExecuteNonQuery();
        }

        for (int i = 0; i <= 8; i++)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = """
                INSERT OR IGNORE INTO KnifeCombinedOptions (KnifeCombinedOptionId, Code, DisplayTextKey, SortOrder)
                VALUES (@id, @code, @dk, @s)
                """;
            cmd.Parameters.AddWithValue("@id", i);
            cmd.Parameters.AddWithValue("@code", i.ToString(System.Globalization.CultureInfo.InvariantCulture));
            cmd.Parameters.AddWithValue("@dk", $"SettingsTab2_KnifeCombo_{i}");
            cmd.Parameters.AddWithValue("@s", i);
            cmd.ExecuteNonQuery();
        }

        void Link(int boxTypeId, params int[] optIds)
        {
            foreach (int oid in optIds)
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = """
                    INSERT OR IGNORE INTO KnifeCombinedOptionByBoxType (BoxTypeId, KnifeCombinedOptionId)
                    VALUES (@b, @o)
                    """;
                cmd.Parameters.AddWithValue("@b", boxTypeId);
                cmd.Parameters.AddWithValue("@o", oid);
                cmd.ExecuteNonQuery();
            }
        }

        Link(1, 0, 1, 2);
        Link(2, 0, 2, 3, 4, 5);
        Link(3, 6, 7, 8);

        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = """
                INSERT OR IGNORE INTO PlcParameterSetProfiles (ProfileId, ProfileName, BoxTypeId, CorrugatedTypeId)
                VALUES
                (2, 'Box_C', 1, 0),
                (3, 'Box_E', 2, 0),
                (4, 'Box_DA7', 3, 0),
                (5, 'DieCutterShared', NULL, 0);
                """;
            cmd.ExecuteNonQuery();
        }

        InsertOrIgnoreDefinition(connection, BoxDieCutterDefinitionKeys.SelectedKnifeCombinedOption, "BoxParameters", "INT", 2000, 0, null);
        int sort = 2001;
        foreach (string k in BoxDieCutterDefinitionKeys.DieCutterRealKeys)
        {
            InsertOrIgnoreDefinition(connection, k, "BoxParameters", "REAL", sort++, null, DefaultRealForDieKey(k));
        }

        InsertOrIgnoreDefinition(connection, BoxDieCutterDefinitionKeys.CarSpeed, "BoxParameters", "INT", 2100, 0, null);

        SeedPlcValueInt(connection, BoxParameterProfileIds.BoxC, BoxDieCutterDefinitionKeys.SelectedKnifeCombinedOption, 0);
        SeedPlcValueInt(connection, BoxParameterProfileIds.BoxE, BoxDieCutterDefinitionKeys.SelectedKnifeCombinedOption, 0);
        SeedPlcValueInt(connection, BoxParameterProfileIds.BoxDa7, BoxDieCutterDefinitionKeys.SelectedKnifeCombinedOption, 0);

        foreach (string k in BoxDieCutterDefinitionKeys.DieCutterRealKeys)
            SeedPlcValueReal(connection, BoxParameterProfileIds.DieCutterShared, k, DefaultRealForDieKey(k));

        SeedPlcValueInt(connection, BoxParameterProfileIds.DieCutterShared, BoxDieCutterDefinitionKeys.CarSpeed, 0);
    }

    private static double DefaultRealForDieKey(string key) => key switch
    {
        BoxDieCutterDefinitionKeys.TrimValue => 3,
        BoxDieCutterDefinitionKeys.BackBoardDensity => 1,
        BoxDieCutterDefinitionKeys.BackBoardMaximum => 1600,
        BoxDieCutterDefinitionKeys.DriveDensity => 0,
        BoxDieCutterDefinitionKeys.ExpandParam => 30,
        BoxDieCutterDefinitionKeys.SubmitWindParam => 0,
        BoxDieCutterDefinitionKeys.PrintWindParam => 0,
        BoxDieCutterDefinitionKeys.Phase2Offset => 100,
        BoxDieCutterDefinitionKeys.ServerPrintOffset => 0,
        BoxDieCutterDefinitionKeys.KnifeThresholdAdding => 20,
        BoxDieCutterDefinitionKeys.ItemCountDefault => 3,
        _ => 0
    };

    private static void InsertOrIgnoreDefinition(
        SqliteConnection connection,
        string key,
        string tabGroup,
        string valueKind,
        int sortOrder,
        int? defaultInt,
        double? defaultReal)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            INSERT OR IGNORE INTO PlcParameterDefinitions (Key, TabGroup, ValueKind, DefaultInt, DefaultReal, SortOrder)
            VALUES (@k, @tg, @vk, @di, @dr, @so)
            """;
        cmd.Parameters.AddWithValue("@k", key);
        cmd.Parameters.AddWithValue("@tg", tabGroup);
        cmd.Parameters.AddWithValue("@vk", valueKind);
        cmd.Parameters.AddWithValue("@di", defaultInt.HasValue ? defaultInt.Value : (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@dr", defaultReal.HasValue ? defaultReal.Value : (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@so", sortOrder);
        cmd.ExecuteNonQuery();
    }

    private static void SeedPlcValueInt(SqliteConnection connection, int profileId, string key, int value)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            INSERT OR IGNORE INTO PlcParameterValues (ProfileId, ParameterDefinitionId, ValueInt, UpdatedAt)
            SELECT @p, d.ParameterDefinitionId, @v, datetime('now')
            FROM PlcParameterDefinitions d WHERE d.Key = @k
            """;
        cmd.Parameters.AddWithValue("@p", profileId);
        cmd.Parameters.AddWithValue("@v", value);
        cmd.Parameters.AddWithValue("@k", key);
        cmd.ExecuteNonQuery();
    }

    private static void SeedPlcValueReal(SqliteConnection connection, int profileId, string key, double value)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            INSERT OR IGNORE INTO PlcParameterValues (ProfileId, ParameterDefinitionId, ValueReal, UpdatedAt)
            SELECT @p, d.ParameterDefinitionId, @v, datetime('now')
            FROM PlcParameterDefinitions d WHERE d.Key = @k
            """;
        cmd.Parameters.AddWithValue("@p", profileId);
        cmd.Parameters.AddWithValue("@v", value);
        cmd.Parameters.AddWithValue("@k", key);
        cmd.ExecuteNonQuery();
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

    private static void EnsureOrdersTable(SqliteConnection connection)
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS Orders (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CreatedAt TEXT NOT NULL,
                OrderNo TEXT NOT NULL,
                VersionNo TEXT NOT NULL,
                OrderQuantity INTEGER NOT NULL,
                BoxType TEXT NOT NULL,
                Category TEXT NOT NULL,
                Phase1 TEXT NOT NULL,
                Phase2 TEXT NOT NULL,
                Phase3 TEXT NOT NULL,
                Length INTEGER NOT NULL,
                Width INTEGER NOT NULL,
                CustomerName TEXT NOT NULL,
                Remarks TEXT NOT NULL
            );
            CREATE INDEX IF NOT EXISTS IX_Orders_VersionNo ON Orders(VersionNo);
            CREATE INDEX IF NOT EXISTS IX_Orders_CreatedAt ON Orders(CreatedAt);
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

    private static void SeedOrdersIfEmpty(SqliteConnection connection)
    {
        using var countCmd = connection.CreateCommand();
        countCmd.CommandText = "SELECT COUNT(*) FROM Orders";
        if ((long)(countCmd.ExecuteScalar() ?? 0L) > 0) return;

        var rnd = new Random(42);
        var boxTypes = new[] { "E", "S" };
        var categories = new[] { "A", "AB", "B", "BC", "C", "E" };
        var names = new[] { "Alice", "Bob", "Carol", "David", "Eve", "Frank", "Grace", "Henry", "Ivy", "Jack" };

        var minDate = new DateTime(2024, 1, 1, 13, 0, 0);
        var maxDate = new DateTime(2026, 1, 10, 13, 0, 0);
        for (int i = 0; i < 100; i++)
        {
            var orderNo = rnd.Next(100, 1000).ToString("D4");
            var rangeTicks = (maxDate - minDate).Ticks;
            var createdAt = minDate.AddTicks((long)(rnd.NextDouble() * rangeTicks));
            var versionNo = $"{createdAt.Year}{orderNo}";
            using var cmd = connection.CreateCommand();
            cmd.CommandText = """
                INSERT INTO Orders (CreatedAt, OrderNo, VersionNo, OrderQuantity, BoxType, Category, Phase1, Phase2, Phase3, Length, Width, CustomerName, Remarks)
                VALUES (@createdAt, @orderNo, @versionNo, @orderQuantity, @boxType, @category, @phase1, @phase2, @phase3, @length, @width, @customerName, @remarks)
                """;
            cmd.Parameters.AddWithValue("@createdAt", createdAt.ToString("O"));
            cmd.Parameters.AddWithValue("@orderNo", orderNo);
            cmd.Parameters.AddWithValue("@versionNo", versionNo);
            cmd.Parameters.AddWithValue("@orderQuantity", rnd.Next(10, 1001));
            cmd.Parameters.AddWithValue("@boxType", boxTypes[rnd.Next(boxTypes.Length)]);
            cmd.Parameters.AddWithValue("@category", categories[rnd.Next(categories.Length)]);
            cmd.Parameters.AddWithValue("@phase1", rnd.Next(50, 151).ToString());
            cmd.Parameters.AddWithValue("@phase2", rnd.Next(200, 300).ToString());
            cmd.Parameters.AddWithValue("@phase3", rnd.Next(300, 400).ToString());
            cmd.Parameters.AddWithValue("@length", rnd.Next(1000, 5001));
            cmd.Parameters.AddWithValue("@width", rnd.Next(1000, 5001));
            cmd.Parameters.AddWithValue("@customerName", names[rnd.Next(names.Length)]);
            cmd.Parameters.AddWithValue("@remarks", "");
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
