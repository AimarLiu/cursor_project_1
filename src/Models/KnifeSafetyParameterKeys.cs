namespace CursorTestApp.Models;

/// <summary>
/// Tab1 刀位安全閾值之 <c>PlcParameterDefinitions.Key</c>（§7.5.1）。
/// </summary>
public static class KnifeSafetyParameterKeys
{
    public const string KnifeSlot_A_Min = "KnifeSlot_A_Min";
    public const string KnifeSlot_A_Max = "KnifeSlot_A_Max";
    public const string KnifeSlot_B_Min = "KnifeSlot_B_Min";
    public const string KnifeSlot_B_Max = "KnifeSlot_B_Max";
    public const string KnifeSlot_E_Min = "KnifeSlot_E_Min";
    public const string KnifeSlot_E_Max = "KnifeSlot_E_Max";
    public const string KnifeSlot_J_Min = "KnifeSlot_J_Min";
    public const string KnifeSlot_J_Max = "KnifeSlot_J_Max";
    public const string KnifeSlot_K_Min = "KnifeSlot_K_Min";
    public const string KnifeSlot_K_Max = "KnifeSlot_K_Max";
    public const string KnifeSlot_F_Min = "KnifeSlot_F_Min";
    public const string KnifeSlot_F_Max = "KnifeSlot_F_Max";
    public const string KnifeSlot_H_Min = "KnifeSlot_H_Min";
    public const string KnifeSlot_H_Max = "KnifeSlot_H_Max";
    public const string KnifeSlot_I_Min = "KnifeSlot_I_Min";
    public const string KnifeSlot_I_Max = "KnifeSlot_I_Max";

    /// <summary>
    /// 固定順序（UI／驗證用）。
    /// </summary>
    public static readonly IReadOnlyList<string> AllKeys =
    [
        KnifeSlot_A_Min, KnifeSlot_A_Max,
        KnifeSlot_B_Min, KnifeSlot_B_Max,
        KnifeSlot_E_Min, KnifeSlot_E_Max,
        KnifeSlot_J_Min, KnifeSlot_J_Max,
        KnifeSlot_K_Min, KnifeSlot_K_Max,
        KnifeSlot_F_Min, KnifeSlot_F_Max,
        KnifeSlot_H_Min, KnifeSlot_H_Max,
        KnifeSlot_I_Min, KnifeSlot_I_Max
    ];

    /// <summary>
    /// 需做 Min／Max 成對驗證之刀位前綴（不含 _Min/_Max）。
    /// </summary>
    public static readonly IReadOnlyList<string> SlotPrefixes =
    [
        "A", "B", "E", "J", "K", "F", "H", "I"
    ];
}
