namespace CursorTestApp.Models;

/// <summary>
/// Tab2 自 DB 載入之一致性快照（供儲存／還原）。
/// </summary>
public sealed class BoxDieCutterSnapshot
{
    public required BoxTypeRow BoxC { get; init; }
    public required BoxTypeRow BoxE { get; init; }
    public required BoxTypeRow BoxDa7 { get; init; }
    public int SelectedKnifeC { get; init; }
    public int SelectedKnifeE { get; init; }
    public int SelectedKnifeDa7 { get; init; }
    public required IReadOnlyDictionary<string, double> DieCutterReals { get; init; }
    public int CarSpeed { get; init; }
}
