namespace CursorTestApp.Services;

/// <summary>
/// 資料庫服務介面，負責初始化與建表。
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// 初始化資料庫連線，若資料表不存在則建立。
    /// </summary>
    void Initialize();

    /// <summary>
    /// 取得資料庫檔案路徑。
    /// </summary>
    string DatabasePath { get; }
}
