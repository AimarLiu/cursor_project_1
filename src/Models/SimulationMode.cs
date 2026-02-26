namespace CursorTestApp.Models;

/// <summary>
/// 模擬生產模式：由 Phase 5 排程管理 Dialog 之 F2/F3 觸發。
/// </summary>
public enum SimulationMode
{
    /// <summary>未啟動或僅關閉 Dialog（F1 離開）</summary>
    None = 0,
    /// <summary>F2 前置排單：僅第一筆訂單生產 5 個後自動停止</summary>
    SmallBatch = 1,
    /// <summary>F3 把全排量：依序生產至排程清空或手動停止</summary>
    FullBatch = 2
}
