namespace CursorTestApp.Models;

/// <summary>
/// Tab2 <c>PlcParameterDefinitions.Key</c>（§7.5.2）。
/// </summary>
public static class BoxDieCutterDefinitionKeys
{
    public const string SelectedKnifeCombinedOption = "BoxParameters.SelectedKnifeCombinedOption";

    public const string TrimValue = "DieCutter.TrimValue";
    public const string BackBoardDensity = "DieCutter.BackBoardDensity";
    public const string BackBoardMaximum = "DieCutter.BackBoardMaximum";
    public const string DriveDensity = "DieCutter.DriveDensity";
    public const string ExpandParam = "DieCutter.ExpandParam";
    public const string SubmitWindParam = "DieCutter.SubmitWindParam";
    public const string PrintWindParam = "DieCutter.PrintWindParam";
    public const string Phase2Offset = "DieCutter.Phase2Offset";
    public const string ServerPrintOffset = "DieCutter.ServerPrintOffset";
    public const string KnifeThresholdAdding = "DieCutter.KnifeThresholdAdding";
    public const string ItemCountDefault = "DieCutter.ItemCountDefault";
    public const string CarSpeed = "DieCutter.CarSpeed";

    public static readonly IReadOnlyList<string> DieCutterRealKeys =
    [
        TrimValue, BackBoardDensity, BackBoardMaximum, DriveDensity, ExpandParam,
        SubmitWindParam, PrintWindParam, Phase2Offset, ServerPrintOffset, KnifeThresholdAdding, ItemCountDefault
    ];
}
