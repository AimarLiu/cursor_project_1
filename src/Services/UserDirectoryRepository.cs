using Microsoft.Data.Sqlite;

namespace CursorTestApp.Services;

public sealed class UserDirectoryRepository : IUserDirectoryRepository
{
    private readonly IDatabaseService _databaseService;

    public UserDirectoryRepository(IDatabaseService databaseService)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
    }

    public IReadOnlyList<UserDirectoryRow> LoadAll()
    {
        var list = new List<UserDirectoryRow>();
        using var connection = OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, Username, Password FROM Users ORDER BY Username";
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new UserDirectoryRow
            {
                Id = r.GetString(0),
                Username = r.GetString(1),
                Password = r.GetString(2)
            });
        }

        return list;
    }

    public void SavePasswords(IReadOnlyList<UserDirectoryRow> rows)
    {
        using var connection = OpenConnection();
        using var tx = connection.BeginTransaction();
        SavePasswords(rows, connection, tx);
        tx.Commit();
    }

    public void SavePasswords(IReadOnlyList<UserDirectoryRow> rows, SqliteConnection connection, SqliteTransaction transaction)
    {
        if (connection is null) throw new ArgumentNullException(nameof(connection));
        if (transaction is null) throw new ArgumentNullException(nameof(transaction));

        foreach (UserDirectoryRow row in rows)
        {
            using var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = "UPDATE Users SET Password = @p WHERE Id = @id";
            cmd.Parameters.AddWithValue("@p", row.Password);
            cmd.Parameters.AddWithValue("@id", row.Id);
            if (cmd.ExecuteNonQuery() == 0)
                throw new InvalidOperationException($"Users 未找到 Id：{row.Id}");
        }
    }

    private SqliteConnection OpenConnection()
    {
        var c = new SqliteConnection($"Data Source={_databaseService.DatabasePath}");
        c.Open();
        return c;
    }
}
