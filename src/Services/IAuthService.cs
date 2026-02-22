using CursorTestApp.Models;

namespace CursorTestApp.Services;

/// <summary>
/// 登入驗證服務介面。
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 以密碼驗證使用者。直接字串比對，不加密。
    /// </summary>
    /// <param name="password">使用者輸入的密碼</param>
    /// <returns>驗證成功回傳使用者，否則回傳 null</returns>
    User? ValidatePassword(string password);

    /// <summary>
    /// 依使用者名稱取得使用者。
    /// </summary>
    /// <param name="username">使用者名稱</param>
    /// <returns>使用者，找不到則回傳 null</returns>
    User? GetUserByUsername(string username);
}
