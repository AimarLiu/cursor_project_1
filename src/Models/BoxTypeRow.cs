namespace CursorTestApp.Models;

/// <summary>
/// <c>BoxTypes</c> 一列（§7.5.2）。
/// </summary>
public sealed class BoxTypeRow
{
    public required int BoxTypeId { get; init; }
    public required string Code { get; init; }
    public int KnifePullOut { get; set; }
    public bool UseEquation2 { get; set; }
    public int FrontKnifePullOut { get; set; }
    public int BackKnifePullOut { get; set; }
    public int BothKnifePullOut { get; set; }
    public bool UseDatabaseBlade { get; set; }
    public bool AutoJudge { get; set; }
}
