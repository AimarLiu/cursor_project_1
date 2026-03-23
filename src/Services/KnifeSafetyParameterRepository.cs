using CursorTestApp.Models;
using Microsoft.Data.Sqlite;

namespace CursorTestApp.Services;

/// <summary>
/// Tab1 刀位閾值 SQLite 存取（單一 ProfileId = <see cref="KnifeProfileId"/>）。
/// </summary>
public sealed class KnifeSafetyParameterRepository : IKnifeSafetyParameterRepository
{
    /// <summary>§7.5.5：全域刀位安全設定檔（單一列）。</summary>
    internal const int KnifeProfileId = 1;

    private readonly IDatabaseService _databaseService;

    public KnifeSafetyParameterRepository(IDatabaseService databaseService)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, int?> LoadAll()
    {
        var result = KnifeSafetyParameterKeys.AllKeys.ToDictionary(k => k, _ => (int?)null);
        using var connection = OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            SELECT d."Key", v.ValueInt
            FROM PlcParameterValues v
            INNER JOIN PlcParameterDefinitions d ON d.ParameterDefinitionId = v.ParameterDefinitionId
            WHERE v.ProfileId = @p AND d."Key" LIKE 'KnifeSlot_%'
            """;
        cmd.Parameters.AddWithValue("@p", KnifeProfileId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            string key = reader.GetString(0);
            if (reader.IsDBNull(1))
                result[key] = null;
            else
                result[key] = reader.GetInt32(1);
        }

        return result;
    }

    /// <inheritdoc />
    public void SaveAll(IReadOnlyDictionary<string, int?> values)
    {
        using var connection = OpenConnection();
        using var tx = connection.BeginTransaction();
        SaveAll(values, connection, tx);
        tx.Commit();
    }

    public void SaveAll(IReadOnlyDictionary<string, int?> values, SqliteConnection connection, SqliteTransaction transaction)
    {
        if (connection is null) throw new ArgumentNullException(nameof(connection));
        if (transaction is null) throw new ArgumentNullException(nameof(transaction));

        foreach (string key in KnifeSafetyParameterKeys.AllKeys)
        {
            values.TryGetValue(key, out int? val);
            using var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = """
                UPDATE PlcParameterValues
                SET ValueInt = @v, UpdatedAt = @u
                WHERE ProfileId = @p AND ParameterDefinitionId = (
                    SELECT ParameterDefinitionId FROM PlcParameterDefinitions WHERE "Key" = @k
                )
                """;
            cmd.Parameters.AddWithValue("@p", KnifeProfileId);
            cmd.Parameters.AddWithValue("@k", key);
            cmd.Parameters.AddWithValue("@u", DateTime.UtcNow.ToString("O"));
            if (val.HasValue)
                cmd.Parameters.AddWithValue("@v", val.Value);
            else
                cmd.Parameters.AddWithValue("@v", DBNull.Value);

            int n = cmd.ExecuteNonQuery();
            if (n == 0)
                throw new InvalidOperationException($"PlcParameterValues 未找到鍵：{key}（請確認已種子）。");
        }
    }

    private SqliteConnection OpenConnection()
    {
        var c = new SqliteConnection($"Data Source={_databaseService.DatabasePath}");
        c.Open();
        return c;
    }
}
