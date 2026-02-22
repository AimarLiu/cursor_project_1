using CursorTestApp.Models;
using Microsoft.Data.Sqlite;

namespace CursorTestApp.Services;

/// <summary>
/// 登入驗證服務實作，以密碼直接字串比對。
/// </summary>
public sealed class AuthService : IAuthService
{
    private readonly IDatabaseService _databaseService;

    /// <summary>
    /// 建立 AuthService。
    /// </summary>
    /// <param name="databaseService">資料庫服務，用於執行查詢</param>
    public AuthService(IDatabaseService databaseService)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
    }

    /// <inheritdoc />
    public User? ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return null;

        using SqliteConnection connection = new($"Data Source={_databaseService.DatabasePath}");
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Username, Password FROM Users WHERE Password = @Password LIMIT 1";
        command.Parameters.AddWithValue("@Password", password);

        using SqliteDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new User
            {
                Id = reader.GetString(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2)
            };
        }

        return null;
    }

    /// <inheritdoc />
    public User? GetUserByUsername(string username)
    {
        if (string.IsNullOrEmpty(username))
            return null;

        using SqliteConnection connection = new($"Data Source={_databaseService.DatabasePath}");
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Username, Password FROM Users WHERE Username = @Username LIMIT 1";
        command.Parameters.AddWithValue("@Username", username);

        using SqliteDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new User
            {
                Id = reader.GetString(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2)
            };
        }

        return null;
    }
}
