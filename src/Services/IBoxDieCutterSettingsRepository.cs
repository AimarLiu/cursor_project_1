using CursorTestApp.Models;
using Microsoft.Data.Sqlite;

namespace CursorTestApp.Services;

/// <summary>
/// Tab2：箱型（<c>BoxTypes</c>）、刀組合選中（<c>PlcParameterValues</c> Profile 2～4）、
/// 連續參數／車速（Profile <see cref="BoxParameterProfileIds.DieCutterShared"/>）。
/// </summary>
public interface IBoxDieCutterSettingsRepository
{
    /// <summary>
    /// 讀取目前 DB 狀態。
    /// </summary>
    BoxDieCutterSnapshot Load();

    /// <summary>
    /// 以交易寫入快照（整表／整 Profile 覆寫式更新）。
    /// </summary>
    void Save(BoxDieCutterSnapshot snapshot);

    /// <summary>
    /// 使用外部交易寫入快照（整表／整 Profile 覆寫式更新）。
    /// </summary>
    void Save(BoxDieCutterSnapshot snapshot, SqliteConnection connection, SqliteTransaction transaction);

    /// <summary>
    /// 刀組合主檔與箱型允許表（供 Combo 篩選）。
    /// </summary>
    IReadOnlyList<int> GetAllowedKnifeOptionIds(int boxTypeId);

    /// <summary>
    /// 主檔 9 筆（Id + RESX DisplayTextKey）。
    /// </summary>
    IReadOnlyList<(int OptionId, string DisplayTextKey)> GetKnifeCombinedOptionRows();
}
