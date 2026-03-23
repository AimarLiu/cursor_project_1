using Microsoft.Data.Sqlite;

namespace CursorTestApp.Services;

/// <summary>§7.5.3：<c>ComponentDisplayNameOverrides</c>（JSON 多語）。</summary>
public interface IComponentDisplayNameRepository
{
    /// <summary>Key：<c>DepartmentCode|ComponentCode</c>；Value：JSON 字串或 null。</summary>
    IReadOnlyDictionary<string, string?> LoadAll();

    /// <summary>以目前字典覆寫整表（先清空再寫入非空白 JSON）。</summary>
    void ReplaceAll(IReadOnlyDictionary<string, string?> compositeKeyToJson);

    /// <summary>使用外部交易覆寫整表（先清空再寫入非空白 JSON）。</summary>
    void ReplaceAll(IReadOnlyDictionary<string, string?> compositeKeyToJson, SqliteConnection connection, SqliteTransaction transaction);
}
