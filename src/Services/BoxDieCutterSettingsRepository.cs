using CursorTestApp.Models;
using Microsoft.Data.Sqlite;

namespace CursorTestApp.Services;

/// <inheritdoc />
public sealed class BoxDieCutterSettingsRepository : IBoxDieCutterSettingsRepository
{
    private readonly IDatabaseService _databaseService;

    public BoxDieCutterSettingsRepository(IDatabaseService databaseService)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
    }

    /// <inheritdoc />
    public IReadOnlyList<(int OptionId, string DisplayTextKey)> GetKnifeCombinedOptionRows()
    {
        var list = new List<(int, string)>();
        using var connection = OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            SELECT KnifeCombinedOptionId, DisplayTextKey FROM KnifeCombinedOptions
            ORDER BY SortOrder
            """;
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add((r.GetInt32(0), r.GetString(1)));
        return list;
    }

    /// <inheritdoc />
    public IReadOnlyList<int> GetAllowedKnifeOptionIds(int boxTypeId)
    {
        var list = new List<int>();
        using var connection = OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            SELECT KnifeCombinedOptionId FROM KnifeCombinedOptionByBoxType
            WHERE BoxTypeId = @b
            ORDER BY KnifeCombinedOptionId
            """;
        cmd.Parameters.AddWithValue("@b", boxTypeId);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add(r.GetInt32(0));
        return list;
    }

    /// <inheritdoc />
    public BoxDieCutterSnapshot Load()
    {
        using var connection = OpenConnection();
        BoxTypeRow c = LoadBoxType(connection, BoxTypeIds.C) ?? throw new InvalidOperationException("BoxTypes C missing.");
        BoxTypeRow e = LoadBoxType(connection, BoxTypeIds.E) ?? throw new InvalidOperationException("BoxTypes E missing.");
        BoxTypeRow d = LoadBoxType(connection, BoxTypeIds.Da7) ?? throw new InvalidOperationException("BoxTypes DA7 missing.");

        int skC = LoadIntValue(connection, BoxParameterProfileIds.BoxC, BoxDieCutterDefinitionKeys.SelectedKnifeCombinedOption) ?? 0;
        int skE = LoadIntValue(connection, BoxParameterProfileIds.BoxE, BoxDieCutterDefinitionKeys.SelectedKnifeCombinedOption) ?? 0;
        int skD = LoadIntValue(connection, BoxParameterProfileIds.BoxDa7, BoxDieCutterDefinitionKeys.SelectedKnifeCombinedOption) ?? 0;

        var reals = new Dictionary<string, double>(StringComparer.Ordinal);
        foreach (string key in BoxDieCutterDefinitionKeys.DieCutterRealKeys)
        {
            double? v = LoadRealValue(connection, BoxParameterProfileIds.DieCutterShared, key);
            reals[key] = v ?? DefaultRealForKey(key);
        }

        int car = LoadIntValue(connection, BoxParameterProfileIds.DieCutterShared, BoxDieCutterDefinitionKeys.CarSpeed) ?? 0;

        return new BoxDieCutterSnapshot
        {
            BoxC = c,
            BoxE = e,
            BoxDa7 = d,
            SelectedKnifeC = skC,
            SelectedKnifeE = skE,
            SelectedKnifeDa7 = skD,
            DieCutterReals = reals,
            CarSpeed = car is 0 or 1 ? car : 0
        };
    }

    /// <inheritdoc />
    public void Save(BoxDieCutterSnapshot snapshot)
    {
        using var connection = OpenConnection();
        using var tx = connection.BeginTransaction();
        Save(snapshot, connection, tx);
        tx.Commit();
    }

    public void Save(BoxDieCutterSnapshot snapshot, SqliteConnection connection, SqliteTransaction transaction)
    {
        if (connection is null) throw new ArgumentNullException(nameof(connection));
        if (transaction is null) throw new ArgumentNullException(nameof(transaction));

        UpdateBoxType(connection, transaction, snapshot.BoxC);
        UpdateBoxType(connection, transaction, snapshot.BoxE);
        UpdateBoxType(connection, transaction, snapshot.BoxDa7);

        SetIntValue(connection, transaction, BoxParameterProfileIds.BoxC, BoxDieCutterDefinitionKeys.SelectedKnifeCombinedOption, snapshot.SelectedKnifeC);
        SetIntValue(connection, transaction, BoxParameterProfileIds.BoxE, BoxDieCutterDefinitionKeys.SelectedKnifeCombinedOption, snapshot.SelectedKnifeE);
        SetIntValue(connection, transaction, BoxParameterProfileIds.BoxDa7, BoxDieCutterDefinitionKeys.SelectedKnifeCombinedOption, snapshot.SelectedKnifeDa7);

        foreach (var kv in snapshot.DieCutterReals)
            SetRealValue(connection, transaction, BoxParameterProfileIds.DieCutterShared, kv.Key, kv.Value);

        SetIntValue(connection, transaction, BoxParameterProfileIds.DieCutterShared, BoxDieCutterDefinitionKeys.CarSpeed, snapshot.CarSpeed is 0 or 1 ? snapshot.CarSpeed : 0);
    }

    private static BoxTypeRow? LoadBoxType(SqliteConnection connection, int boxTypeId)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            SELECT BoxTypeId, Code, KnifePullOut, UseEquation2, FrontKnifePullOut, BackKnifePullOut, BothKnifePullOut,
                   UseDatabaseBlade, AutoJudge
            FROM BoxTypes WHERE BoxTypeId = @id
            """;
        cmd.Parameters.AddWithValue("@id", boxTypeId);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        return new BoxTypeRow
        {
            BoxTypeId = r.GetInt32(0),
            Code = r.GetString(1),
            KnifePullOut = r.GetInt32(2),
            UseEquation2 = r.GetInt32(3) != 0,
            FrontKnifePullOut = r.GetInt32(4),
            BackKnifePullOut = r.GetInt32(5),
            BothKnifePullOut = r.GetInt32(6),
            UseDatabaseBlade = r.GetInt32(7) != 0,
            AutoJudge = r.GetInt32(8) != 0
        };
    }

    private static void UpdateBoxType(SqliteConnection connection, SqliteTransaction tx, BoxTypeRow row)
    {
        using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
            UPDATE BoxTypes SET
                KnifePullOut = @k, UseEquation2 = @ue, FrontKnifePullOut = @f, BackKnifePullOut = @bk, BothKnifePullOut = @bo,
                UseDatabaseBlade = @udb, AutoJudge = @aj
            WHERE BoxTypeId = @id
            """;
        cmd.Parameters.AddWithValue("@k", row.KnifePullOut);
        cmd.Parameters.AddWithValue("@ue", row.UseEquation2 ? 1 : 0);
        cmd.Parameters.AddWithValue("@f", row.FrontKnifePullOut);
        cmd.Parameters.AddWithValue("@bk", row.BackKnifePullOut);
        cmd.Parameters.AddWithValue("@bo", row.BothKnifePullOut);
        cmd.Parameters.AddWithValue("@udb", row.UseDatabaseBlade ? 1 : 0);
        cmd.Parameters.AddWithValue("@aj", row.AutoJudge ? 1 : 0);
        cmd.Parameters.AddWithValue("@id", row.BoxTypeId);
        if (cmd.ExecuteNonQuery() != 1)
            throw new InvalidOperationException($"BoxTypes 更新失敗：BoxTypeId={row.BoxTypeId}");
    }

    private static int? LoadIntValue(SqliteConnection connection, int profileId, string key)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            SELECT v.ValueInt
            FROM PlcParameterValues v
            INNER JOIN PlcParameterDefinitions d ON d.ParameterDefinitionId = v.ParameterDefinitionId
            WHERE v.ProfileId = @p AND d.Key = @k
            """;
        cmd.Parameters.AddWithValue("@p", profileId);
        cmd.Parameters.AddWithValue("@k", key);
        object? o = cmd.ExecuteScalar();
        if (o == null || o == DBNull.Value) return null;
        return Convert.ToInt32(o);
    }

    private static double? LoadRealValue(SqliteConnection connection, int profileId, string key)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            SELECT v.ValueReal
            FROM PlcParameterValues v
            INNER JOIN PlcParameterDefinitions d ON d.ParameterDefinitionId = v.ParameterDefinitionId
            WHERE v.ProfileId = @p AND d.Key = @k
            """;
        cmd.Parameters.AddWithValue("@p", profileId);
        cmd.Parameters.AddWithValue("@k", key);
        object? o = cmd.ExecuteScalar();
        if (o == null || o == DBNull.Value) return null;
        return Convert.ToDouble(o);
    }

    private static void SetIntValue(SqliteConnection connection, SqliteTransaction tx, int profileId, string key, int value)
    {
        using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
            UPDATE PlcParameterValues SET ValueInt = @v, UpdatedAt = @u
            WHERE ProfileId = @p AND ParameterDefinitionId = (SELECT ParameterDefinitionId FROM PlcParameterDefinitions WHERE Key = @k)
            """;
        cmd.Parameters.AddWithValue("@v", value);
        cmd.Parameters.AddWithValue("@u", DateTime.UtcNow.ToString("O"));
        cmd.Parameters.AddWithValue("@p", profileId);
        cmd.Parameters.AddWithValue("@k", key);
        if (cmd.ExecuteNonQuery() != 1)
            throw new InvalidOperationException($"PlcParameterValues INT 更新失敗：Profile={profileId}, Key={key}");
    }

    private static void SetRealValue(SqliteConnection connection, SqliteTransaction tx, int profileId, string key, double value)
    {
        using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
            UPDATE PlcParameterValues SET ValueReal = @v, UpdatedAt = @u
            WHERE ProfileId = @p AND ParameterDefinitionId = (SELECT ParameterDefinitionId FROM PlcParameterDefinitions WHERE Key = @k)
            """;
        cmd.Parameters.AddWithValue("@v", value);
        cmd.Parameters.AddWithValue("@u", DateTime.UtcNow.ToString("O"));
        cmd.Parameters.AddWithValue("@p", profileId);
        cmd.Parameters.AddWithValue("@k", key);
        if (cmd.ExecuteNonQuery() != 1)
            throw new InvalidOperationException($"PlcParameterValues REAL 更新失敗：Profile={profileId}, Key={key}");
    }

    private static double DefaultRealForKey(string key) => key switch
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

    private SqliteConnection OpenConnection()
    {
        var c = new SqliteConnection($"Data Source={_databaseService.DatabasePath}");
        c.Open();
        return c;
    }
}
