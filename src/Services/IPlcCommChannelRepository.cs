using CursorTestApp.Models.Tab3;

using Microsoft.Data.Sqlite;

namespace CursorTestApp.Services;

/// <summary>§7.5.3：<c>PlcCommChannelSettings</c> 三列讀寫。</summary>
public interface IPlcCommChannelRepository
{
    IReadOnlyDictionary<string, PlcCommChannelDto> LoadAll();

    void SaveAll(IReadOnlyDictionary<string, PlcCommChannelDto> rows);

    void SaveAll(IReadOnlyDictionary<string, PlcCommChannelDto> rows, SqliteConnection connection, SqliteTransaction transaction);
}
