using CursorTestApp.Models.Tab3;
using Microsoft.Data.Sqlite;

namespace CursorTestApp.Services;

public sealed class PlcCommChannelRepository : IPlcCommChannelRepository
{
    private readonly IDatabaseService _databaseService;

    public PlcCommChannelRepository(IDatabaseService databaseService)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
    }

    public IReadOnlyDictionary<string, PlcCommChannelDto> LoadAll()
    {
        var map = new Dictionary<string, PlcCommChannelDto>(StringComparer.Ordinal);
        using var connection = OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            SELECT ChannelRole, PortName, BaudRate, ByteSize, Parity, StopBits
            FROM PlcCommChannelSettings
            """;
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            string role = r.GetString(0);
            map[role] = new PlcCommChannelDto
            {
                ChannelRole = role,
                PortName = r.GetString(1),
                BaudRate = r.GetInt32(2),
                ByteSize = r.GetInt32(3),
                Parity = r.GetString(4),
                StopBits = r.GetInt32(5)
            };
        }

        return map;
    }

    public void SaveAll(IReadOnlyDictionary<string, PlcCommChannelDto> rows)
    {
        using var connection = OpenConnection();
        using var tx = connection.BeginTransaction();
        SaveAll(rows, connection, tx);
        tx.Commit();
    }

    public void SaveAll(IReadOnlyDictionary<string, PlcCommChannelDto> rows, SqliteConnection connection, SqliteTransaction transaction)
    {
        if (connection is null) throw new ArgumentNullException(nameof(connection));
        if (transaction is null) throw new ArgumentNullException(nameof(transaction));

        foreach (var kv in rows)
        {
            using var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = """
                UPDATE PlcCommChannelSettings
                SET PortName=@p, BaudRate=@b, ByteSize=@by, Parity=@par, StopBits=@s, UpdatedAt=@u
                WHERE ChannelRole=@r
                """;
            PlcCommChannelDto d = kv.Value;
            cmd.Parameters.AddWithValue("@r", d.ChannelRole);
            cmd.Parameters.AddWithValue("@p", d.PortName);
            cmd.Parameters.AddWithValue("@b", d.BaudRate);
            cmd.Parameters.AddWithValue("@by", d.ByteSize);
            cmd.Parameters.AddWithValue("@par", d.Parity);
            cmd.Parameters.AddWithValue("@s", d.StopBits);
            cmd.Parameters.AddWithValue("@u", DateTime.UtcNow.ToString("O"));
            if (cmd.ExecuteNonQuery() == 0)
                throw new InvalidOperationException($"PlcCommChannelSettings 缺少列：{d.ChannelRole}");
        }
    }

    private SqliteConnection OpenConnection()
    {
        var c = new SqliteConnection($"Data Source={_databaseService.DatabasePath}");
        c.Open();
        return c;
    }
}
