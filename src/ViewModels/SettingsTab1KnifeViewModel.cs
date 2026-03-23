using System.Globalization;
using Microsoft.Data.Sqlite;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CursorTestApp.Helpers;
using CursorTestApp.Models;
using CursorTestApp.Services;

namespace CursorTestApp.ViewModels;

/// <summary>
/// Settings Tab1：刀位安全閾值（16 鍵）之顯示、驗證與儲存。
/// </summary>
public sealed partial class SettingsTab1KnifeViewModel : ViewModelBase
{
    private readonly IKnifeSafetyParameterRepository _repository;
    private readonly ILocalizationService _localizationService;

    public LocalizedString MaxE { get; }
    public LocalizedString MinE { get; }
    public LocalizedString MaxJ { get; }
    public LocalizedString MinJ { get; }
    public LocalizedString MaxK { get; }
    public LocalizedString MinK { get; }
    public LocalizedString MaxF { get; }
    public LocalizedString MinF { get; }
    public LocalizedString MaxA { get; }
    public LocalizedString MinA { get; }
    public LocalizedString MaxH { get; }
    public LocalizedString MinH { get; }
    public LocalizedString MaxI { get; }
    public LocalizedString MinI { get; }
    public LocalizedString MaxB { get; }
    public LocalizedString MinB { get; }

    [ObservableProperty] private string _knifeSlotEMax = "";
    [ObservableProperty] private string _knifeSlotEMin = "";
    [ObservableProperty] private string _knifeSlotJMax = "";
    [ObservableProperty] private string _knifeSlotJMin = "";
    [ObservableProperty] private string _knifeSlotKMax = "";
    [ObservableProperty] private string _knifeSlotKMin = "";
    [ObservableProperty] private string _knifeSlotFMax = "";
    [ObservableProperty] private string _knifeSlotFMin = "";
    [ObservableProperty] private string _knifeSlotAMax = "";
    [ObservableProperty] private string _knifeSlotAMin = "";
    [ObservableProperty] private string _knifeSlotHMax = "";
    [ObservableProperty] private string _knifeSlotHMin = "";
    [ObservableProperty] private string _knifeSlotIMax = "";
    [ObservableProperty] private string _knifeSlotIMin = "";
    [ObservableProperty] private string _knifeSlotBMax = "";
    [ObservableProperty] private string _knifeSlotBMin = "";

    public SettingsTab1KnifeViewModel(
        IKnifeSafetyParameterRepository repository,
        ILocalizationService localizationService)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));

        MaxE = new LocalizedString(localizationService, "SettingsTab1_MaxE");
        MinE = new LocalizedString(localizationService, "SettingsTab1_MinE");
        MaxJ = new LocalizedString(localizationService, "SettingsTab1_MaxJ");
        MinJ = new LocalizedString(localizationService, "SettingsTab1_MinJ");
        MaxK = new LocalizedString(localizationService, "SettingsTab1_MaxK");
        MinK = new LocalizedString(localizationService, "SettingsTab1_MinK");
        MaxF = new LocalizedString(localizationService, "SettingsTab1_MaxF");
        MinF = new LocalizedString(localizationService, "SettingsTab1_MinF");
        MaxA = new LocalizedString(localizationService, "SettingsTab1_MaxA");
        MinA = new LocalizedString(localizationService, "SettingsTab1_MinA");
        MaxH = new LocalizedString(localizationService, "SettingsTab1_MaxH");
        MinH = new LocalizedString(localizationService, "SettingsTab1_MinH");
        MaxI = new LocalizedString(localizationService, "SettingsTab1_MaxI");
        MinI = new LocalizedString(localizationService, "SettingsTab1_MinI");
        MaxB = new LocalizedString(localizationService, "SettingsTab1_MaxB");
        MinB = new LocalizedString(localizationService, "SettingsTab1_MinB");

        ReloadFromDatabase();
    }

    /// <summary>
    /// F3：自 DB 重載並覆蓋編輯中內容。
    /// </summary>
    public void ReloadFromDatabase()
    {
        IReadOnlyDictionary<string, int?> data = _repository.LoadAll();
        foreach (string key in KnifeSafetyParameterKeys.AllKeys)
        {
            string text = data.TryGetValue(key, out int? v) && v.HasValue ? v.Value.ToString(CultureInfo.InvariantCulture) : "";
            SetFieldByKey(key, text);
        }
    }

    /// <summary>
    /// F2：驗證後寫入 DB。
    /// </summary>
    public bool TrySaveToDatabase()
    {
        if (!TryBuildDictionary(out Dictionary<string, int?>? dict, out string? errorMessage))
        {
            MessageBox.Show(errorMessage, _localizationService.GetString("AppName"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        try
        {
            _repository.SaveAll(dict!);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                _localizationService.GetString("SettingsTab1_SaveFailed") + "\n" + ex.Message,
                _localizationService.GetString("AppName"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return false;
        }
    }

    /// <summary>
    /// F2：使用外部交易寫入 DB（Phase 7 單一 transaction）。
    /// </summary>
    public bool TrySaveToDatabase(SqliteConnection connection, SqliteTransaction transaction)
    {
        if (!TryBuildDictionary(out Dictionary<string, int?>? dict, out string? errorMessage))
        {
            MessageBox.Show(errorMessage, _localizationService.GetString("AppName"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        try
        {
            _repository.SaveAll(dict!, connection, transaction);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                _localizationService.GetString("SettingsTab1_SaveFailed") + "\n" + ex.Message,
                _localizationService.GetString("AppName"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return false;
        }
    }

    private bool TryBuildDictionary(out Dictionary<string, int?>? dict, out string? errorMessage)
    {
        dict = null;
        errorMessage = null;
        var result = new Dictionary<string, int?>(StringComparer.Ordinal);

        foreach (string key in KnifeSafetyParameterKeys.AllKeys)
        {
            string raw = GetFieldByKey(key).Trim();
            if (string.IsNullOrEmpty(raw))
            {
                result[key] = null;
                continue;
            }

            if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int n))
            {
                errorMessage = _localizationService.GetString("SettingsTab1_Validate_Number");
                return false;
            }

            result[key] = n;
        }

        foreach (string slot in KnifeSafetyParameterKeys.SlotPrefixes)
        {
            string minK = $"KnifeSlot_{slot}_Min";
            string maxK = $"KnifeSlot_{slot}_Max";
            int? min = result[minK];
            int? max = result[maxK];
            if (min.HasValue && max.HasValue && min.Value > max.Value)
            {
                errorMessage = _localizationService.GetString("SettingsTab1_Validate_MinMax");
                return false;
            }
        }

        dict = result;
        return true;
    }

    private string GetFieldByKey(string key) => key switch
    {
        KnifeSafetyParameterKeys.KnifeSlot_A_Min => KnifeSlotAMin,
        KnifeSafetyParameterKeys.KnifeSlot_A_Max => KnifeSlotAMax,
        KnifeSafetyParameterKeys.KnifeSlot_B_Min => KnifeSlotBMin,
        KnifeSafetyParameterKeys.KnifeSlot_B_Max => KnifeSlotBMax,
        KnifeSafetyParameterKeys.KnifeSlot_E_Min => KnifeSlotEMin,
        KnifeSafetyParameterKeys.KnifeSlot_E_Max => KnifeSlotEMax,
        KnifeSafetyParameterKeys.KnifeSlot_J_Min => KnifeSlotJMin,
        KnifeSafetyParameterKeys.KnifeSlot_J_Max => KnifeSlotJMax,
        KnifeSafetyParameterKeys.KnifeSlot_K_Min => KnifeSlotKMin,
        KnifeSafetyParameterKeys.KnifeSlot_K_Max => KnifeSlotKMax,
        KnifeSafetyParameterKeys.KnifeSlot_F_Min => KnifeSlotFMin,
        KnifeSafetyParameterKeys.KnifeSlot_F_Max => KnifeSlotFMax,
        KnifeSafetyParameterKeys.KnifeSlot_H_Min => KnifeSlotHMin,
        KnifeSafetyParameterKeys.KnifeSlot_H_Max => KnifeSlotHMax,
        KnifeSafetyParameterKeys.KnifeSlot_I_Min => KnifeSlotIMin,
        KnifeSafetyParameterKeys.KnifeSlot_I_Max => KnifeSlotIMax,
        _ => "",
    };

    private void SetFieldByKey(string key, string value)
    {
        switch (key)
        {
            case KnifeSafetyParameterKeys.KnifeSlot_A_Min: KnifeSlotAMin = value; break;
            case KnifeSafetyParameterKeys.KnifeSlot_A_Max: KnifeSlotAMax = value; break;
            case KnifeSafetyParameterKeys.KnifeSlot_B_Min: KnifeSlotBMin = value; break;
            case KnifeSafetyParameterKeys.KnifeSlot_B_Max: KnifeSlotBMax = value; break;
            case KnifeSafetyParameterKeys.KnifeSlot_E_Min: KnifeSlotEMin = value; break;
            case KnifeSafetyParameterKeys.KnifeSlot_E_Max: KnifeSlotEMax = value; break;
            case KnifeSafetyParameterKeys.KnifeSlot_J_Min: KnifeSlotJMin = value; break;
            case KnifeSafetyParameterKeys.KnifeSlot_J_Max: KnifeSlotJMax = value; break;
            case KnifeSafetyParameterKeys.KnifeSlot_K_Min: KnifeSlotKMin = value; break;
            case KnifeSafetyParameterKeys.KnifeSlot_K_Max: KnifeSlotKMax = value; break;
            case KnifeSafetyParameterKeys.KnifeSlot_F_Min: KnifeSlotFMin = value; break;
            case KnifeSafetyParameterKeys.KnifeSlot_F_Max: KnifeSlotFMax = value; break;
            case KnifeSafetyParameterKeys.KnifeSlot_H_Min: KnifeSlotHMin = value; break;
            case KnifeSafetyParameterKeys.KnifeSlot_H_Max: KnifeSlotHMax = value; break;
            case KnifeSafetyParameterKeys.KnifeSlot_I_Min: KnifeSlotIMin = value; break;
            case KnifeSafetyParameterKeys.KnifeSlot_I_Max: KnifeSlotIMax = value; break;
        }
    }
}
