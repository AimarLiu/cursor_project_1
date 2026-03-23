namespace CursorTestApp.Models;

/// <summary>
/// §7.5.2／§7.5.5：<c>PlcParameterSetProfiles.ProfileId</c> 定值（與種子一致）。
/// </summary>
public static class BoxParameterProfileIds
{
    public const int KnifeSafetyGlobal = 1;
    public const int BoxC = 2;
    public const int BoxE = 3;
    public const int BoxDa7 = 4;
    /// <summary>右欄 <c>DieCutter.*</c> 共用一 Profile（單一 UI 欄位組）。</summary>
    public const int DieCutterShared = 5;
}
