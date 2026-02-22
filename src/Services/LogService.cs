using System.Collections.ObjectModel;
using System.Globalization;

namespace CursorTestApp.Services;

/// <summary>
/// Log 服務實作，格式為 [HH:mm:ss] 訊息內容。
/// </summary>
public sealed class LogService : ILogService
{
    private const string LogFormat = "[{0:HH:mm:ss}] {1}";

    /// <inheritdoc />
    public ObservableCollection<string> LogMessages { get; } = new();

    /// <inheritdoc />
    public void Append(string message)
    {
        string line = string.Format(CultureInfo.CurrentCulture, LogFormat, DateTime.Now, message);
        LogMessages.Add(line);
    }
}
