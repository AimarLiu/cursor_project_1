using Microsoft.Data.Sqlite;
using System.Data;

namespace CursorTestApp.Services;

/// <summary>
/// Tab1 刀位安全閾值（KnifeSlot_*）之讀寫；對應 <c>PlcParameterValues</c> 單一全域 Profile。
/// </summary>
public interface IKnifeSafetyParameterRepository
{
    /// <summary>
    /// 讀取 16 鍵；鍵名為 <c>KnifeSlot_A_Min</c> 等；值為 <c>null</c> 表示未設定／空白。
    /// </summary>
    IReadOnlyDictionary<string, int?> LoadAll();

    /// <summary>
    /// 以交易寫入全部鍵（整數或 <c>null</c>）。
    /// </summary>
    void SaveAll(IReadOnlyDictionary<string, int?> values);

    /// <summary>
    /// 使用外部交易寫入全部鍵（整數或 <c>null</c>）。
    /// 用於 Phase 7 單一 transaction（合併 Tab1～Tab3）。
    /// </summary>
    void SaveAll(IReadOnlyDictionary<string, int?> values, SqliteConnection connection, SqliteTransaction transaction);
}
