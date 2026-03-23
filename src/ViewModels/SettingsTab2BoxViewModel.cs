using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Data.Sqlite;
using CursorTestApp.Helpers;
using CursorTestApp.Models;
using CursorTestApp.Services;

namespace CursorTestApp.ViewModels;

/// <summary>
/// Settings Tab2：箱型刀選擇 + DieCutter 連續參數／車速。
/// </summary>
public sealed partial class SettingsTab2BoxViewModel : ViewModelBase
{
    private readonly IBoxDieCutterSettingsRepository _repository;
    private readonly ILocalizationService _localizationService;

    public LocalizedString GroupC { get; }
    public LocalizedString GroupE { get; }
    /// <summary>巢狀於 E 型箱選擇刀內之「E型箱設定」（公式／退刀）。</summary>
    public LocalizedString GroupESettings { get; }
    public LocalizedString GroupDa7 { get; }
    public LocalizedString LabelSlotBitePaperReturnKnife { get; }
    public LocalizedString CheckUseDatabaseBlade { get; }
    public LocalizedString CheckUseFormula2 { get; }
    public LocalizedString LabelEFrontKnifeRetract { get; }
    public LocalizedString LabelERearKnifeRetract { get; }
    public LocalizedString SubtitleWithoutFormula2 { get; }
    public LocalizedString LabelSlotFrontBackRetractEach { get; }
    public LocalizedString CheckAutoJudge { get; }

    public LocalizedString LTrim { get; }
    public LocalizedString LBackBoardDensity { get; }
    public LocalizedString LBackBoardMax { get; }
    public LocalizedString LDriveDensity { get; }
    public LocalizedString LExpand { get; }
    public LocalizedString LSubmitWind { get; }
    public LocalizedString LPrintWind { get; }
    public LocalizedString LPhase2Offset { get; }
    public LocalizedString LServoPrintOffset { get; }
    public LocalizedString LKnifeThreshold { get; }
    public LocalizedString LItemCountDefault { get; }
    public LocalizedString LCarSpeed { get; }
    public LocalizedString CarSpeedOpt0 { get; }
    public LocalizedString CarSpeedOpt1 { get; }

    /// <summary>車速 Combo（0／1），標籤隨語系更新。</summary>
    public IReadOnlyList<CarSpeedOptionRow> CarSpeedOptionRows { get; }

    public ObservableCollection<KnifeComboDisplayItem> CKnifeOptions { get; } = new();
    public ObservableCollection<KnifeComboDisplayItem> EKnifeOptions { get; } = new();
    public ObservableCollection<KnifeComboDisplayItem> Da7KnifeOptions { get; } = new();

    [ObservableProperty] private string _cKnifePullOut = "";
    [ObservableProperty] private string _eKnifePullOut = "";
    [ObservableProperty] private string _da7KnifePullOut = "";

    [ObservableProperty] private int _cKnifeSelectedId;
    [ObservableProperty] private int _eKnifeSelectedId;
    [ObservableProperty] private int _da7KnifeSelectedId;

    [ObservableProperty] private bool _eUseDatabaseBlade;
    [ObservableProperty] private bool _eUseEquation2;
    [ObservableProperty] private string _eFrontKnifePullOut = "";
    [ObservableProperty] private string _eBackKnifePullOut = "";
    [ObservableProperty] private string _eBothKnifePullOut = "";

    [ObservableProperty] private bool _da7AutoJudge;

    [ObservableProperty] private string _dieTrim = "";
    [ObservableProperty] private string _dieBackDensity = "";
    [ObservableProperty] private string _dieBackMax = "";
    [ObservableProperty] private string _dieDriveDensity = "";
    [ObservableProperty] private string _dieExpand = "";
    [ObservableProperty] private string _dieSubmitWind = "";
    [ObservableProperty] private string _diePrintWind = "";
    [ObservableProperty] private string _diePhase2 = "";
    [ObservableProperty] private string _dieServoOffset = "";
    [ObservableProperty] private string _dieKnifeThreshold = "";
    [ObservableProperty] private string _dieItemCount = "";

    /// <summary>0 或 1，對應車速 Combo。</summary>
    [ObservableProperty] private int _carSpeed;

    public SettingsTab2BoxViewModel(IBoxDieCutterSettingsRepository repository, ILocalizationService localizationService)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));

        GroupC = new LocalizedString(localizationService, "SettingsTab2_Group_C_SelectKnife");
        GroupE = new LocalizedString(localizationService, "SettingsTab2_Group_E_SelectKnife");
        GroupESettings = new LocalizedString(localizationService, "SettingsTab2_Group_E_BoxSettings");
        GroupDa7 = new LocalizedString(localizationService, "SettingsTab2_Group_DA7_SelectKnife");
        LabelSlotBitePaperReturnKnife = new LocalizedString(localizationService, "SettingsTab2_Label_SlotBitePaperReturnKnife");
        CheckUseDatabaseBlade = new LocalizedString(localizationService, "SettingsTab2_Check_UseDatabaseBlade");
        CheckUseFormula2 = new LocalizedString(localizationService, "SettingsTab2_Check_UseFormula2");
        LabelEFrontKnifeRetract = new LocalizedString(localizationService, "SettingsTab2_Label_E_FrontKnifeRetract");
        LabelERearKnifeRetract = new LocalizedString(localizationService, "SettingsTab2_Label_E_RearKnifeRetract");
        SubtitleWithoutFormula2 = new LocalizedString(localizationService, "SettingsTab2_Subtitle_WithoutFormula2");
        LabelSlotFrontBackRetractEach = new LocalizedString(localizationService, "SettingsTab2_Label_SlotFrontBackRetractEach");
        CheckAutoJudge = new LocalizedString(localizationService, "SettingsTab2_Check_AutoJudge");

        LTrim = new LocalizedString(localizationService, "SettingsTab2_Label_TrimValue");
        LBackBoardDensity = new LocalizedString(localizationService, "SettingsTab2_Label_BackBaffleTightness");
        LBackBoardMax = new LocalizedString(localizationService, "SettingsTab2_Label_PaperFeedBackBaffleMax");
        LDriveDensity = new LocalizedString(localizationService, "SettingsTab2_Label_DriveSideTightness");
        LExpand = new LocalizedString(localizationService, "SettingsTab2_Label_PaperFeedExtension");
        LSubmitWind = new LocalizedString(localizationService, "SettingsTab2_Label_PaperFeedAirVolume");
        LPrintWind = new LocalizedString(localizationService, "SettingsTab2_Label_PrintingAirVolume");
        LPhase2Offset = new LocalizedString(localizationService, "SettingsTab2_Label_DestroyPhase2Offset");
        LServoPrintOffset = new LocalizedString(localizationService, "SettingsTab2_Label_ServoPrintOffset");
        LKnifeThreshold = new LocalizedString(localizationService, "SettingsTab2_Label_SafetyBladeExtra");
        LItemCountDefault = new LocalizedString(localizationService, "SettingsTab2_Label_DropCollectCountDefault");
        LCarSpeed = new LocalizedString(localizationService, "SettingsTab2_Label_CarSpeedFormat");
        CarSpeedOpt0 = new LocalizedString(localizationService, "SettingsTab2_CarSpeed_Option0");
        CarSpeedOpt1 = new LocalizedString(localizationService, "SettingsTab2_CarSpeed_Option1");
        CarSpeedOptionRows =
        [
            new CarSpeedOptionRow { Value = 0, Label = CarSpeedOpt0 },
            new CarSpeedOptionRow { Value = 1, Label = CarSpeedOpt1 }
        ];

        ReloadFromDatabase();
    }

    /// <summary>F3：自 DB 重載。</summary>
    public void ReloadFromDatabase()
    {
        BoxDieCutterSnapshot s = _repository.Load();
        RebuildComboLists();
        ApplySnapshot(s);
        // 刀組合 ComboBox：ItemsSource Clear 再填後，Loaded 再刷新一次（含「同值不觸發」時的強制通知）。
        if (Application.Current?.Dispatcher != null)
        {
            _ = Application.Current.Dispatcher.BeginInvoke(
                () => RefreshKnifeComboDisplayAfterItemsSourceRebuild(),
                DispatcherPriority.Loaded);
        }
    }

    /// <summary>F2：驗證後寫入。</summary>
    public bool TrySaveToDatabase()
    {
        if (!TryBuildSnapshot(out BoxDieCutterSnapshot? snap, out string? err))
        {
            MessageBox.Show(err, _localizationService.GetString("AppName"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        try
        {
            _repository.Save(snap!);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                (_localizationService.GetString("SettingsTab2_SaveFailed") ?? "Save failed.") + "\n" + ex.Message,
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
        if (!TryBuildSnapshot(out BoxDieCutterSnapshot? snap, out string? err))
        {
            MessageBox.Show(err, _localizationService.GetString("AppName"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        try
        {
            _repository.Save(snap!, connection, transaction);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                (_localizationService.GetString("SettingsTab2_SaveFailed") ?? "Save failed.") + "\n" + ex.Message,
                _localizationService.GetString("AppName"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return false;
        }
    }

    private void ApplySnapshot(BoxDieCutterSnapshot s)
    {
        CKnifePullOut = s.BoxC.KnifePullOut.ToString(CultureInfo.InvariantCulture);
        EKnifePullOut = s.BoxE.KnifePullOut.ToString(CultureInfo.InvariantCulture);
        Da7KnifePullOut = s.BoxDa7.KnifePullOut.ToString(CultureInfo.InvariantCulture);

        CKnifeSelectedId = s.SelectedKnifeC;
        EKnifeSelectedId = s.SelectedKnifeE;
        Da7KnifeSelectedId = s.SelectedKnifeDa7;

        EUseDatabaseBlade = s.BoxE.UseDatabaseBlade;
        EUseEquation2 = s.BoxE.UseEquation2;
        EFrontKnifePullOut = s.BoxE.FrontKnifePullOut.ToString(CultureInfo.InvariantCulture);
        EBackKnifePullOut = s.BoxE.BackKnifePullOut.ToString(CultureInfo.InvariantCulture);
        EBothKnifePullOut = s.BoxE.BothKnifePullOut.ToString(CultureInfo.InvariantCulture);
        Da7AutoJudge = s.BoxDa7.AutoJudge;

        DieTrim = FormatDouble(s.DieCutterReals[BoxDieCutterDefinitionKeys.TrimValue]);
        DieBackDensity = FormatDouble(s.DieCutterReals[BoxDieCutterDefinitionKeys.BackBoardDensity]);
        DieBackMax = FormatDouble(s.DieCutterReals[BoxDieCutterDefinitionKeys.BackBoardMaximum]);
        DieDriveDensity = FormatDouble(s.DieCutterReals[BoxDieCutterDefinitionKeys.DriveDensity]);
        DieExpand = FormatDouble(s.DieCutterReals[BoxDieCutterDefinitionKeys.ExpandParam]);
        DieSubmitWind = FormatDouble(s.DieCutterReals[BoxDieCutterDefinitionKeys.SubmitWindParam]);
        DiePrintWind = FormatDouble(s.DieCutterReals[BoxDieCutterDefinitionKeys.PrintWindParam]);
        DiePhase2 = FormatDouble(s.DieCutterReals[BoxDieCutterDefinitionKeys.Phase2Offset]);
        DieServoOffset = FormatDouble(s.DieCutterReals[BoxDieCutterDefinitionKeys.ServerPrintOffset]);
        DieKnifeThreshold = FormatDouble(s.DieCutterReals[BoxDieCutterDefinitionKeys.KnifeThresholdAdding]);
        DieItemCount = FormatDouble(s.DieCutterReals[BoxDieCutterDefinitionKeys.ItemCountDefault]);
        CarSpeed = s.CarSpeed;

        RefreshKnifeComboDisplayAfterItemsSourceRebuild();
    }

    /// <summary>
    /// 刀組合 Combo 若目前選中 Id 不在清單內（或綁定異常），改為清單第 0 筆（第一個可選 OptionId）。
    /// </summary>
    private void EnsureKnifeComboSelectionsDefaultToFirstRow()
    {
        if (CKnifeOptions.Count > 0 && !CKnifeOptions.Any(i => i.OptionId == CKnifeSelectedId))
            CKnifeSelectedId = CKnifeOptions[0].OptionId;
        if (EKnifeOptions.Count > 0 && !EKnifeOptions.Any(i => i.OptionId == EKnifeSelectedId))
            EKnifeSelectedId = EKnifeOptions[0].OptionId;
        if (Da7KnifeOptions.Count > 0 && !Da7KnifeOptions.Any(i => i.OptionId == Da7KnifeSelectedId))
            Da7KnifeSelectedId = Da7KnifeOptions[0].OptionId;
    }

    /// <summary>
    /// <see cref="RebuildComboLists"/> 會 Clear 刀組合下拉清單；若重載後選中 Id 與先前相同，
    /// ObservableProperty 可能不觸發變更，WPF ComboBox 常留空白。先對齊合法 Id，再於有第二筆選項時短暫改選另一筆後還原，並強制 PropertyChanged。
    /// </summary>
    private void RefreshKnifeComboDisplayAfterItemsSourceRebuild()
    {
        EnsureKnifeComboSelectionsDefaultToFirstRow();

        static void Bump(
            ObservableCollection<KnifeComboDisplayItem> list,
            Action<int> set,
            Func<int> get)
        {
            if (list.Count == 0) return;
            int target = get();
            KnifeComboDisplayItem? alt = list.FirstOrDefault(i => i.OptionId != target);
            if (alt != null)
            {
                set(alt.OptionId);
                set(target);
            }
            else
            {
                set(target);
            }
        }

        Bump(CKnifeOptions, v => CKnifeSelectedId = v, () => CKnifeSelectedId);
        Bump(EKnifeOptions, v => EKnifeSelectedId = v, () => EKnifeSelectedId);
        Bump(Da7KnifeOptions, v => Da7KnifeSelectedId = v, () => Da7KnifeSelectedId);

        OnPropertyChanged(nameof(CKnifeSelectedId));
        OnPropertyChanged(nameof(EKnifeSelectedId));
        OnPropertyChanged(nameof(Da7KnifeSelectedId));
        OnPropertyChanged(nameof(CarSpeed));
    }

    private static string FormatDouble(double v) =>
        v.ToString("0.######", CultureInfo.InvariantCulture);

    private void RebuildComboLists()
    {
        IReadOnlyList<(int OptionId, string DisplayTextKey)> meta = _repository.GetKnifeCombinedOptionRows();
        void Fill(ObservableCollection<KnifeComboDisplayItem> target, int boxTypeId)
        {
            target.Clear();
            var allowed = new HashSet<int>(_repository.GetAllowedKnifeOptionIds(boxTypeId));
            foreach ((int id, string dkey) in meta)
            {
                if (!allowed.Contains(id)) continue;
                string text = _localizationService.GetString(dkey) ?? dkey;
                target.Add(new KnifeComboDisplayItem { OptionId = id, DisplayText = text });
            }
        }

        Fill(CKnifeOptions, BoxTypeIds.C);
        Fill(EKnifeOptions, BoxTypeIds.E);
        Fill(Da7KnifeOptions, BoxTypeIds.Da7);
    }

    private bool TryBuildSnapshot(out BoxDieCutterSnapshot? snap, out string? error)
    {
        snap = null;
        error = null;

        if (!TryParseInt(CKnifePullOut, out int cPull, out error)) return false;
        if (!TryParseInt(EKnifePullOut, out int ePull, out error)) return false;
        if (!TryParseInt(Da7KnifePullOut, out int dPull, out error)) return false;

        if (!ValidateKnifeId(BoxTypeIds.C, CKnifeSelectedId, out error)) return false;
        if (!ValidateKnifeId(BoxTypeIds.E, EKnifeSelectedId, out error)) return false;
        if (!ValidateKnifeId(BoxTypeIds.Da7, Da7KnifeSelectedId, out error)) return false;

        if (!TryParseInt(EFrontKnifePullOut, out int eFront, out error)) return false;
        if (!TryParseInt(EBackKnifePullOut, out int eBack, out error)) return false;
        if (!TryParseInt(EBothKnifePullOut, out int eBoth, out error)) return false;

        var reals = new Dictionary<string, double>(StringComparer.Ordinal);
        foreach (string key in BoxDieCutterDefinitionKeys.DieCutterRealKeys)
        {
            string raw = GetDieFieldByKey(key);
            if (!TryParseDouble(raw, out double d, out error))
                return false;
            reals[key] = d;
        }

        int cs = CarSpeed is 0 or 1 ? CarSpeed : 0;
        if (CarSpeed != 0 && CarSpeed != 1)
        {
            error = _localizationService.GetString("SettingsTab2_Validate_CarSpeed");
            return false;
        }

        snap = new BoxDieCutterSnapshot
        {
            BoxC = new BoxTypeRow
            {
                BoxTypeId = BoxTypeIds.C,
                Code = "C",
                KnifePullOut = cPull,
                UseEquation2 = false,
                FrontKnifePullOut = 0,
                BackKnifePullOut = 0,
                BothKnifePullOut = 0,
                UseDatabaseBlade = false,
                AutoJudge = false
            },
            BoxE = new BoxTypeRow
            {
                BoxTypeId = BoxTypeIds.E,
                Code = "E",
                KnifePullOut = ePull,
                UseEquation2 = EUseEquation2,
                FrontKnifePullOut = eFront,
                BackKnifePullOut = eBack,
                BothKnifePullOut = eBoth,
                UseDatabaseBlade = EUseDatabaseBlade,
                AutoJudge = false
            },
            BoxDa7 = new BoxTypeRow
            {
                BoxTypeId = BoxTypeIds.Da7,
                Code = "DA7",
                KnifePullOut = dPull,
                UseEquation2 = false,
                FrontKnifePullOut = 0,
                BackKnifePullOut = 0,
                BothKnifePullOut = 0,
                UseDatabaseBlade = false,
                AutoJudge = Da7AutoJudge
            },
            SelectedKnifeC = CKnifeSelectedId,
            SelectedKnifeE = EKnifeSelectedId,
            SelectedKnifeDa7 = Da7KnifeSelectedId,
            DieCutterReals = reals,
            CarSpeed = cs
        };
        return true;
    }

    private bool ValidateKnifeId(int boxTypeId, int selectedId, out string? err)
    {
        err = null;
        var allowed = new HashSet<int>(_repository.GetAllowedKnifeOptionIds(boxTypeId));
        if (!allowed.Contains(selectedId))
        {
            err = _localizationService.GetString("SettingsTab2_Validate_KnifeOption");
            return false;
        }

        return true;
    }

    private string GetDieFieldByKey(string key) => key switch
    {
        BoxDieCutterDefinitionKeys.TrimValue => DieTrim,
        BoxDieCutterDefinitionKeys.BackBoardDensity => DieBackDensity,
        BoxDieCutterDefinitionKeys.BackBoardMaximum => DieBackMax,
        BoxDieCutterDefinitionKeys.DriveDensity => DieDriveDensity,
        BoxDieCutterDefinitionKeys.ExpandParam => DieExpand,
        BoxDieCutterDefinitionKeys.SubmitWindParam => DieSubmitWind,
        BoxDieCutterDefinitionKeys.PrintWindParam => DiePrintWind,
        BoxDieCutterDefinitionKeys.Phase2Offset => DiePhase2,
        BoxDieCutterDefinitionKeys.ServerPrintOffset => DieServoOffset,
        BoxDieCutterDefinitionKeys.KnifeThresholdAdding => DieKnifeThreshold,
        BoxDieCutterDefinitionKeys.ItemCountDefault => DieItemCount,
        _ => ""
    };

    private bool TryParseInt(string raw, out int v, out string? err)
    {
        err = null;
        raw = raw.Trim();
        if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out v))
        {
            err = _localizationService.GetString("SettingsTab2_Validate_Integer") ?? "Invalid integer.";
            return false;
        }

        return true;
    }

    private bool TryParseDouble(string raw, out double v, out string? err)
    {
        err = null;
        raw = raw.Trim();
        if (!double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out v))
        {
            err = _localizationService.GetString("SettingsTab2_Validate_Number") ?? "Invalid number.";
            return false;
        }

        return true;
    }
}
