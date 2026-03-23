using Microsoft.Data.Sqlite;

namespace CursorTestApp.Services;

/// <summary>Settings Tab3：<c>Users</c> 列表與密碼更新。</summary>
public interface IUserDirectoryRepository
{
    IReadOnlyList<UserDirectoryRow> LoadAll();

    void SavePasswords(IReadOnlyList<UserDirectoryRow> rows);

    void SavePasswords(IReadOnlyList<UserDirectoryRow> rows, SqliteConnection connection, SqliteTransaction transaction);
}

public sealed class UserDirectoryRow
{
    public required string Id { get; init; }
    public required string Username { get; init; }
    public required string Password { get; set; }
}
