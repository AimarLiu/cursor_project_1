using CursorTestApp.Helpers;

namespace CursorTestApp.Models;

/// <summary>
/// Tab2 車速 Combo 列（綁定 <see cref="LocalizedString"/> 以支援語系切換）。
/// </summary>
public sealed class CarSpeedOptionRow
{
    public required int Value { get; init; }
    public required LocalizedString Label { get; init; }
}
