using System.Collections.ObjectModel;

namespace CursorTestApp.Services;

/// <summary>
/// Log 輸出服務介面，用於右下角 Log 區域。
/// </summary>
public interface ILogService
{
    /// <summary>
    /// Log 訊息集合，格式為 [HH:mm:ss] 訊息內容。
    /// </summary>
    ObservableCollection<string> LogMessages { get; }

    /// <summary>
    /// 新增一則 Log 訊息。
    /// </summary>
    /// <param name="message">訊息內容</param>
    void Append(string message);
}
