using CursorTestApp.Models.Tab3;
using Microsoft.Data.Sqlite;

namespace CursorTestApp.Services;

/// <summary>
/// Tab3：通訊表、元件名稱覆寫表，以及 <c>OtherPlcParameters</c> 參數定義／預設值種子。
/// </summary>
internal static class Tab3DatabaseBootstrap
{
    public static void EnsureAuxiliaryTables(SqliteConnection connection)
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS PlcCommChannelSettings (
                ChannelRole TEXT PRIMARY KEY,
                PortName TEXT NOT NULL,
                BaudRate INTEGER NOT NULL,
                ByteSize INTEGER NOT NULL,
                Parity TEXT NOT NULL,
                StopBits INTEGER NOT NULL,
                UpdatedAt TEXT
            );
            CREATE TABLE IF NOT EXISTS ComponentDisplayNameOverrides (
                DepartmentCode TEXT NOT NULL,
                ComponentCode TEXT NOT NULL,
                DisplayNamesJson TEXT,
                PRIMARY KEY (DepartmentCode, ComponentCode)
            );
            """;
        foreach (string stmt in sql.Split(";", StringSplitOptions.RemoveEmptyEntries))
        {
            string t = stmt.Trim();
            if (string.IsNullOrEmpty(t)) continue;
            using var cmd = connection.CreateCommand();
            cmd.CommandText = t;
            cmd.ExecuteNonQuery();
        }
    }

    public static void SeedCommChannelsIfMissing(SqliteConnection connection)
    {
        void Upsert(string role, string port, int baud, int bytes, string parity, int stop)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = """
                INSERT OR IGNORE INTO PlcCommChannelSettings (ChannelRole, PortName, BaudRate, ByteSize, Parity, StopBits, UpdatedAt)
                VALUES (@r, @p, @b, @by, @par, @s, datetime('now'))
                """;
            cmd.Parameters.AddWithValue("@r", role);
            cmd.Parameters.AddWithValue("@p", port);
            cmd.Parameters.AddWithValue("@b", baud);
            cmd.Parameters.AddWithValue("@by", bytes);
            cmd.Parameters.AddWithValue("@par", parity);
            cmd.Parameters.AddWithValue("@s", stop);
            cmd.ExecuteNonQuery();
        }

        Upsert(PlcCommChannelRole.Printer, "COM2", 115200, 8, "None", 1);
        Upsert(PlcCommChannelRole.Panel, "NONE", 115200, 8, "None", 1);
        Upsert(PlcCommChannelRole.Plc, "COM3", 19200, 8, "None", 1);
    }

    public static void SeedDefinitionsAndValues(SqliteConnection connection)
    {
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = """
                INSERT OR IGNORE INTO PlcParameterSetProfiles (ProfileId, ProfileName, BoxTypeId, CorrugatedTypeId)
                VALUES (@id, 'OtherPlcGlobal', NULL, 0)
                """;
            cmd.Parameters.AddWithValue("@id", OtherPlcParameterProfileId.Value);
            cmd.ExecuteNonQuery();
        }

        int sort = 3000;
        IReadOnlyList<Tab3PlcCatalog.ComponentTriplet> rows = Tab3PlcCatalog.GetRowsForDepartment("All");
        foreach (Tab3PlcCatalog.ComponentTriplet row in rows)
        {
            InsertDefinition(connection, $"{row.KeyPrefix}.Max", sort++, row.DefaultMax);
            InsertDefinition(connection, $"{row.KeyPrefix}.Min", sort++, row.DefaultMin);
            InsertDefinition(connection, $"{row.KeyPrefix}.Accurate", sort++, row.DefaultAccurate);
        }

        int pid = OtherPlcParameterProfileId.Value;
        foreach (Tab3PlcCatalog.ComponentTriplet row in rows)
        {
            SeedValue(connection, pid, $"{row.KeyPrefix}.Max", row.DefaultMax);
            SeedValue(connection, pid, $"{row.KeyPrefix}.Min", row.DefaultMin);
            SeedValue(connection, pid, $"{row.KeyPrefix}.Accurate", row.DefaultAccurate);
        }
    }

    private static void InsertDefinition(SqliteConnection connection, string key, int sortOrder, double? defaultReal)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            INSERT OR IGNORE INTO PlcParameterDefinitions (Key, TabGroup, ValueKind, DefaultInt, DefaultReal, SortOrder)
            VALUES (@k, 'OtherPlcParameters', 'REAL', NULL, @dr, @so)
            """;
        cmd.Parameters.AddWithValue("@k", key);
        cmd.Parameters.AddWithValue("@dr", defaultReal.HasValue ? defaultReal.Value : (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@so", sortOrder);
        cmd.ExecuteNonQuery();
    }

    private static void SeedValue(SqliteConnection connection, int profileId, string key, double? value)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            INSERT OR IGNORE INTO PlcParameterValues (ProfileId, ParameterDefinitionId, ValueReal, UpdatedAt)
            SELECT @p, d.ParameterDefinitionId, @v, datetime('now')
            FROM PlcParameterDefinitions d WHERE d.Key = @k
            """;
        cmd.Parameters.AddWithValue("@p", profileId);
        cmd.Parameters.AddWithValue("@k", key);
        if (value.HasValue)
            cmd.Parameters.AddWithValue("@v", value.Value);
        else
            cmd.Parameters.AddWithValue("@v", DBNull.Value);
        cmd.ExecuteNonQuery();
    }
}
