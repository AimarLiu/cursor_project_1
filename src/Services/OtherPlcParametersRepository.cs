using CursorTestApp.Models.Tab3;
using Microsoft.Data.Sqlite;

namespace CursorTestApp.Services;

public sealed class OtherPlcParametersRepository : IOtherPlcParametersRepository
{
    private readonly IDatabaseService _databaseService;

    public OtherPlcParametersRepository(IDatabaseService databaseService)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
    }

    public IReadOnlyDictionary<string, double?> LoadValues()
    {
        var map = new Dictionary<string, double?>(StringComparer.Ordinal);
        using var connection = OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            SELECT d.Key, v.ValueReal
            FROM PlcParameterValues v
            INNER JOIN PlcParameterDefinitions d ON d.ParameterDefinitionId = v.ParameterDefinitionId
            WHERE v.ProfileId = @p AND d.TabGroup = 'OtherPlcParameters'
            """;
        cmd.Parameters.AddWithValue("@p", OtherPlcParameterProfileId.Value);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            string key = r.GetString(0);
            if (r.IsDBNull(1))
                map[key] = null;
            else
                map[key] = r.GetDouble(1);
        }

        return map;
    }

    public void SaveValues(IReadOnlyDictionary<string, double?> values)
    {
        using var connection = OpenConnection();
        using var tx = connection.BeginTransaction();
        SaveValues(values, connection, tx);
        tx.Commit();
    }

    public void SaveValues(IReadOnlyDictionary<string, double?> values, SqliteConnection connection, SqliteTransaction transaction)
    {
        if (connection is null) throw new ArgumentNullException(nameof(connection));
        if (transaction is null) throw new ArgumentNullException(nameof(transaction));

        foreach (var kv in values)
        {
            using var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = """
                UPDATE PlcParameterValues
                SET ValueReal = @v, UpdatedAt = @u
                WHERE ProfileId = @p AND ParameterDefinitionId = (
                    SELECT ParameterDefinitionId FROM PlcParameterDefinitions WHERE Key = @k
                )
                """;
            cmd.Parameters.AddWithValue("@p", OtherPlcParameterProfileId.Value);
            cmd.Parameters.AddWithValue("@k", kv.Key);
            cmd.Parameters.AddWithValue("@u", DateTime.UtcNow.ToString("O"));
            if (kv.Value.HasValue)
                cmd.Parameters.AddWithValue("@v", kv.Value.Value);
            else
                cmd.Parameters.AddWithValue("@v", DBNull.Value);
            if (cmd.ExecuteNonQuery() == 0)
                throw new InvalidOperationException($"PlcParameterValues 未找到鍵：{kv.Key}");
        }
    }

    private SqliteConnection OpenConnection()
    {
        var c = new SqliteConnection($"Data Source={_databaseService.DatabasePath}");
        c.Open();
        return c;
    }
}
