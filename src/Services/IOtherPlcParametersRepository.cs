using Microsoft.Data.Sqlite;

namespace CursorTestApp.Services;

/// <summary>Tab3 元件數值（<c>OtherPlcParameters</c> Profile）讀寫。</summary>
public interface IOtherPlcParametersRepository
{
    IReadOnlyDictionary<string, double?> LoadValues();

    void SaveValues(IReadOnlyDictionary<string, double?> values);

    void SaveValues(IReadOnlyDictionary<string, double?> values, SqliteConnection connection, SqliteTransaction transaction);
}
