namespace CursorTestApp.Models.Tab3;

/// <summary>
/// Tab3 部門／元件列與 <c>PlcParameterDefinitions.Key</c> 前綴（不含 .Max/.Min/.Accurate）及種子預設值。
/// </summary>
public static class Tab3PlcCatalog
{
    public sealed class ComponentTriplet
    {
        public required string DepartmentCode { get; init; }
        public required string ComponentCode { get; init; }
        /// <summary>例：<c>Feed.DriveSideBaffle</c>、<c>Print.7.Register</c>、<c>Component.NumberOfPaper.Bundle</c>。</summary>
        public required string KeyPrefix { get; init; }
        public double? DefaultMax { get; init; }
        public double? DefaultMin { get; init; }
        public double? DefaultAccurate { get; init; }
    }

    /// <summary>部門選項（Layout：含 All；無種子者 Grid 空白）。</summary>
    public static readonly string[] DepartmentSelectorOrder =
    [
        "Feed", "Print1", "Print2", "Print3", "Print4", "Print5", "Print6",
        "Print7", "Print8", "Slotter", "Knife", "DieCut", "Stack", "Inline", "Knife II",
        "Other", "All"
    ];

    public static string GetDefaultDisplayNameKey(string departmentCode, string componentCode)
    {
        if (departmentCode.StartsWith("Print", StringComparison.Ordinal) && departmentCode.Length > "Print".Length)
        {
            string n = departmentCode["Print".Length..];
            return $"SettingsTab3_Print{n}_Name_{componentCode}";
        }

        return departmentCode switch
        {
            "Feed" => $"SettingsTab3_Feed_Name_{componentCode}",
            "Other" => $"SettingsTab3_Other_Name_{componentCode}",
            _ => $"SettingsTab3_Feed_Name_{componentCode}"
        };
    }

    public static IReadOnlyList<ComponentTriplet> GetRowsForDepartment(string departmentCode)
    {
        if (departmentCode == "All")
            return BuildAllMergedRows();

        return departmentCode switch
        {
            "Feed" => FeedRows,
            "Print1" => PrintRowsForPrintN(1),
            "Print2" => PrintRowsForPrintN(2),
            "Print3" => PrintRowsForPrintN(3),
            "Print4" or "Print5" or "Print6" => Array.Empty<ComponentTriplet>(),
            "Print7" => PrintRowsForPrintN(7),
            "Print8" => PrintRowsForPrintN(8),
            "Other" => OtherRows,
            _ => Array.Empty<ComponentTriplet>()
        };
    }

    private static IReadOnlyList<ComponentTriplet> BuildAllMergedRows()
    {
        var list = new List<ComponentTriplet>();
        list.AddRange(FeedRows);
        list.AddRange(PrintRowsForPrintN(1));
        list.AddRange(PrintRowsForPrintN(2));
        list.AddRange(PrintRowsForPrintN(3));
        list.AddRange(PrintRowsForPrintN(7));
        list.AddRange(PrintRowsForPrintN(8));
        list.AddRange(OtherRows);
        return list;
    }

    private static IReadOnlyList<ComponentTriplet> PrintRowsForPrintN(int printN)
    {
        if (printN is 7 or 8)
        {
            return
            [
                Row($"Print{printN}", "Register", $"Print.{printN}.Register", 1277, 0, 0),
                Row($"Print{printN}", "PressAperture", $"Print.{printN}.PressAperture", 9, 1.5, 0.2),
                Row($"Print{printN}", "Phase", $"Print.{printN}.Phase", 1272, 0, 1),
                Row($"Print{printN}", "LateralGap2", $"Print.{printN}.LateralGap2", 10, -10, 0.2),
                Row($"Print{printN}", "ColorLateralShift", $"Print.{printN}.ColorLateralShift", 10, -10, 1),
                Row($"Print{printN}", "LateralGap3", $"Print.{printN}.LateralGap3", 10, -10, 0.2)
            ];
        }

        double pMin = printN == 2 ? 1.5 : 0.5;
        return
        [
            Row($"Print{printN}", "Register", $"Print.{printN}.Register", 0, 0, 0),
            Row($"Print{printN}", "PressAperture", $"Print.{printN}.PressAperture", 9, pMin, 0.2),
            Row($"Print{printN}", "Phase", $"Print.{printN}.Phase", 1272, 0, 1),
            Row($"Print{printN}", "LateralGap2", $"Print.{printN}.LateralGap2", 0, 0, 0),
            Row($"Print{printN}", "ColorLateralShift", $"Print.{printN}.ColorLateralShift", 10, -10, 1),
            Row($"Print{printN}", "LateralGap3", $"Print.{printN}.LateralGap3", 0, 0, 0)
        ];
    }

    private static ComponentTriplet Row(string dept, string code, string prefix, double? max, double? min, double? acc) =>
        new()
        {
            DepartmentCode = dept,
            ComponentCode = code,
            KeyPrefix = prefix,
            DefaultMax = max,
            DefaultMin = min,
            DefaultAccurate = acc
        };

    private static readonly ComponentTriplet[] FeedRows =
    [
        Row("Feed", "DriveSideBaffle", "Feed.DriveSideBaffle", 1350, 350, 1),
        Row("Feed", "OperatorSideBaffle", "Feed.OperatorSideBaffle", 1375, 375, 1),
        Row("Feed", "RearBaffle", "Feed.RearBaffle", 1600, 340, 1),
        Row("Feed", "FeedRollerGap", "Feed.FeedRollerGap", 10, 0, 0.2),
        Row("Feed", "FrontBaffleGap", "Feed.FrontBaffleGap", 12, 0, 0.2),
        Row("Feed", "FeedPhase", "Feed.FeedPhase", 1272, 0, 1),
        Row("Feed", "CurrentSheetCount", "Feed.CurrentSheetCount", 99999, 0, 0),
        Row("Feed", "OptimizationButton", "Feed.OptimizationButton", 0, 0, 0)
    ];

    private static readonly ComponentTriplet[] OtherRows =
    [
        Row("Other", "NumberOfPaper.Bundle", "Component.NumberOfPaper.Bundle", 999, 1, 0),
        Row("Other", "BellAlarm", "Component.BellAlarm", null, 0, 0),
        Row("Other", "ContinueSubmittingPaper", "Component.ContinueSubmittingPaper", 99999, 0, 0),
        Row("Other", "CTKMaximumSpeed", "Component.CTKMaximumSpeed", null, 0, 0),
        Row("Other", "EarKnife", "Component.EarKnife", 45, 0, 0),
        Row("Other", "AlreadyProduced", "Component.AlreadyProduced", null, 0, 0)
    ];
}
