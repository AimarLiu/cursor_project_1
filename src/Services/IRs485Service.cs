namespace CursorTestApp.Services;

/// <summary>
/// RS485 讀寫介面（COM1: 9600,8,N,1）。模擬時可由 Layout2 模擬生產更新狀態。
/// </summary>
public interface IRs485Service
{
    /// <summary>OPT 運作中（紅燈）</summary>
    bool IsOptActive { get; }
    /// <summary>PLC 訊號（綠燈閃爍）</summary>
    bool IsPlcActive { get; }
    /// <summary>車速（RPM）</summary>
    int Speed { get; }
    /// <summary>是否發生錯誤（顯示 alarm.gif）</summary>
    bool HasError { get; }
}
