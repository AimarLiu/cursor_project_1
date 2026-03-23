using Microsoft.Data.Sqlite;

namespace CursorTestApp.Services;

public sealed class ComponentDisplayNameRepository : IComponentDisplayNameRepository
{
    private readonly IDatabaseService _databaseService;

    public ComponentDisplayNameRepository(IDatabaseService databaseService)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
    }

    public IReadOnlyDictionary<string, string?> LoadAll()
    {
        var map = new Dictionary<string, string?>(StringComparer.Ordinal);
        using var connection = OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            SELECT DepartmentCode, ComponentCode, DisplayNamesJson
            FROM ComponentDisplayNameOverrides
            """;
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            string dept = r.GetString(0);
            string comp = r.GetString(1);
            string? json = r.IsDBNull(2) ? null : r.GetString(2);
            map[MakeKey(dept, comp)] = json;
        }

        return map;
    }

    public void ReplaceAll(IReadOnlyDictionary<string, string?> compositeKeyToJson)
    {
        using var connection = OpenConnection();
        using var tx = connection.BeginTransaction();
        ReplaceAll(compositeKeyToJson, connection, tx);
        tx.Commit();
    }

    public void ReplaceAll(IReadOnlyDictionary<string, string?> compositeKeyToJson, SqliteConnection connection, SqliteTransaction transaction)
    {
        if (connection is null) throw new ArgumentNullException(nameof(connection));
        if (transaction is null) throw new ArgumentNullException(nameof(transaction));

        using (var clear = connection.CreateCommand())
        {
            clear.Transaction = transaction;
            clear.CommandText = "DELETE FROM ComponentDisplayNameOverrides";
            clear.ExecuteNonQuery();
        }

        foreach (var kv in compositeKeyToJson)
        {
            if (string.IsNullOrWhiteSpace(kv.Value))
                continue;
            (string dept, string comp) = ParseKey(kv.Key);
            using var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = """
                INSERT INTO ComponentDisplayNameOverrides (DepartmentCode, ComponentCode, DisplayNamesJson)
                VALUES (@d, @c, @j)
                """;
            cmd.Parameters.AddWithValue("@d", dept);
            cmd.Parameters.AddWithValue("@c", comp);
            cmd.Parameters.AddWithValue("@j", kv.Value!);
            cmd.ExecuteNonQuery();
        }
    }

    public static string MakeKey(string departmentCode, string componentCode) => $"{departmentCode}|{componentCode}";

    private static (string Dept, string Comp) ParseKey(string key)
    {
        int i = key.IndexOf('|', StringComparison.Ordinal);
        if (i <= 0 || i >= key.Length - 1)
            throw new ArgumentException("Invalid composite key.", nameof(key));
        return (key[..i], key[(i + 1)..]);
    }

    private SqliteConnection OpenConnection()
    {
        var c = new SqliteConnection($"Data Source={_databaseService.DatabasePath}");
        c.Open();
        return c;
    }
}
