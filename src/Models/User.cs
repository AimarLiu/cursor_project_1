namespace CursorTestApp.Models;

/// <summary>
/// 使用者模型，對應 Users 資料表。
/// </summary>
public sealed class User
{
    /// <summary>
    /// 使用者 ID。
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// 使用者名稱。
    /// </summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// 密碼（本專案為測試用，以明文儲存）。
    /// </summary>
    public string Password { get; init; } = string.Empty;
}
