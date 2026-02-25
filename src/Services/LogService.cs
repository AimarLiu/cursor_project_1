using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;

namespace CursorTestApp.Services;

/// <summary>
/// Log 服務實作，格式為 [HH:mm:ss] 訊息內容。
/// 所有對 LogMessages 的新增皆在 UI 執行緒、且延後至 Layout 之後執行，避免 ItemsControl 與項目來源不一致（異機仍發生時改用此方式）。
/// </summary>
public sealed class LogService : ILogService
{
    private const string LogFormat = "[{0:HH:mm:ss}] {1}";
    private static readonly DispatcherPriority LogAddPriority = DispatcherPriority.Loaded;

    /// <inheritdoc />
    public ObservableCollection<string> LogMessages { get; } = new();

    /// <inheritdoc />
    public void Append(string message)
    {
        string line = string.Format(CultureInfo.CurrentCulture, LogFormat, DateTime.Now, message);
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher == null)
        {
            LogMessages.Add(line);
            return;
        }
        // 一律以 Loaded 優先級排程，避免在 layout/measure 過程中新增導致異機「ItemsControl 與其項目來源不一致」
        dispatcher.InvokeAsync(() => LogMessages.Add(line), LogAddPriority);
    }
}
