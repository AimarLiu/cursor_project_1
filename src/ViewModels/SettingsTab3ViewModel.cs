using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Data.Sqlite;
using CursorTestApp.Helpers;
using CursorTestApp.Models.Tab3;
using CursorTestApp.Services;

namespace CursorTestApp.ViewModels;

/// <summary>
/// Settings Tab3：通訊、部門 DataGrid、使用者密碼（§7.5.3）。
/// </summary>
public sealed partial class SettingsTab3ViewModel : ViewModelBase
{
    private readonly IOtherPlcParametersRepository _otherPlc;
    private readonly IPlcCommChannelRepository _comm;
    private readonly IComponentDisplayNameRepository _names;
    private readonly IUserDirectoryRepository _users;
    private readonly ILocalizationService _localization;

    private Dictionary<string, double?> _plcValues = new(StringComparer.Ordinal);
    private Dictionary<string, string?> _displayJson = new(StringComparer.Ordinal);

    public ObservableCollection<Tab3GridRowViewModel> GridRows { get; } = new();
    public ObservableCollection<Tab3DepartmentItem> DepartmentItems { get; } = new();

    public IReadOnlyList<string> ComPortOptions { get; } = BuildComPorts();
    public IReadOnlyList<int> BaudOptions { get; } = [9600, 19200, 38400, 56700, 115200, 921600];
    public IReadOnlyList<int> ByteSizeOptions { get; } = [7, 8];
    public IReadOnlyList<string> ParityOptions { get; } = ["None", "Even", "Odd"];
    public IReadOnlyList<int> StopBitsOptions { get; } = [1, 2];

    public LocalizedString GroupOtherCommPorts { get; }
    public LocalizedString GroupGluePlc { get; }
    public LocalizedString GroupUserPassword { get; }
    public LocalizedString LabelPrinterPlcPort { get; }
    public LocalizedString LabelPanelPort { get; }
    public LocalizedString LabelGluePlcPort { get; }
    public LocalizedString LabelBaudRate { get; }
    public LocalizedString LabelByteSize { get; }
    public LocalizedString LabelParity { get; }
    public LocalizedString LabelStopBits { get; }
    public LocalizedString LabelUserPassword { get; }
    public LocalizedString LabelAdminPassword { get; }
    public LocalizedString ColumnComponentName { get; }
    public LocalizedString ColumnMax { get; }
    public LocalizedString ColumnMin { get; }
    public LocalizedString ColumnAccurate { get; }
    public LocalizedString ColumnDepartment { get; }
    public LocalizedString ColumnUsername { get; }
    public LocalizedString ColumnPassword { get; }

    private string? _userId;
    private string? _adminId;

    /// <summary>選「All」時顯示部門欄。</summary>
    public bool ShowDepartmentColumnInGrid => SelectedDepartmentCode == "All";

    [ObservableProperty]
    private string _selectedDepartmentCode = "Feed";

    [ObservableProperty]
    private string _printerPort = "COM2";

    [ObservableProperty]
    private string _panelPort = "NONE";

    [ObservableProperty]
    private string _gluePort = "COM3";

    [ObservableProperty]
    private int _glueBaudRate = 19200;

    [ObservableProperty]
    private int _glueByteSize = 8;

    [ObservableProperty]
    private string _glueParity = "None";

    [ObservableProperty]
    private int _glueStopBits = 1;

    [ObservableProperty]
    private string _userPassword = "";

    [ObservableProperty]
    private string _adminPassword = "";

    public SettingsTab3ViewModel(
        IOtherPlcParametersRepository otherPlc,
        IPlcCommChannelRepository comm,
        IComponentDisplayNameRepository names,
        IUserDirectoryRepository users,
        ILocalizationService localization)
    {
        _otherPlc = otherPlc ?? throw new ArgumentNullException(nameof(otherPlc));
        _comm = comm ?? throw new ArgumentNullException(nameof(comm));
        _names = names ?? throw new ArgumentNullException(nameof(names));
        _users = users ?? throw new ArgumentNullException(nameof(users));
        _localization = localization ?? throw new ArgumentNullException(nameof(localization));

        foreach (string code in Tab3PlcCatalog.DepartmentSelectorOrder)
        {
            string resxKey = MapDepartmentToResxKey(code);
            DepartmentItems.Add(new Tab3DepartmentItem
            {
                Code = code,
                Label = new LocalizedString(localization, resxKey)
            });
        }

        GroupOtherCommPorts = new LocalizedString(localization, "SettingsTab3_Group_OtherCommPorts");
        GroupGluePlc = new LocalizedString(localization, "SettingsTab3_Group_GluePlc");
        GroupUserPassword = new LocalizedString(localization, "SettingsTab3_Group_UserPasswordManagement");
        LabelPrinterPlcPort = new LocalizedString(localization, "SettingsTab3_Label_PrinterPlcPort");
        LabelPanelPort = new LocalizedString(localization, "SettingsTab3_Label_PanelPort");
        LabelGluePlcPort = new LocalizedString(localization, "SettingsTab3_Label_GluePlcPort");
        LabelBaudRate = new LocalizedString(localization, "SettingsTab3_Label_BaudRate");
        LabelByteSize = new LocalizedString(localization, "SettingsTab3_Label_ByteSize");
        LabelParity = new LocalizedString(localization, "SettingsTab3_Label_Parity");
        LabelStopBits = new LocalizedString(localization, "SettingsTab3_Label_StopBits");
        LabelUserPassword = new LocalizedString(localization, "SettingsTab3_Label_UserPassword");
        LabelAdminPassword = new LocalizedString(localization, "SettingsTab3_Label_AdminPassword");
        ColumnComponentName = new LocalizedString(localization, "SettingsTab3_Column_ComponentName");
        ColumnMax = new LocalizedString(localization, "SettingsTab3_Column_Max");
        ColumnMin = new LocalizedString(localization, "SettingsTab3_Column_Min");
        ColumnAccurate = new LocalizedString(localization, "SettingsTab3_Column_Accurate");
        ColumnDepartment = new LocalizedString(localization, "SettingsTab3_Column_Department");
        ColumnUsername = new LocalizedString(localization, "SettingsTab3_Column_UserName");
        ColumnPassword = new LocalizedString(localization, "SettingsTab3_Column_Password");

        ReloadFromDatabase();
    }

    partial void OnSelectedDepartmentCodeChanged(string value)
    {
        MergeGridIntoMaster();
        RebuildGrid();
        OnPropertyChanged(nameof(ShowDepartmentColumnInGrid));
    }

    /// <summary>F3：自 DB 重載。</summary>
    public void ReloadFromDatabase()
    {
        _plcValues = _otherPlc.LoadValues().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.Ordinal);
        _displayJson = _names.LoadAll().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.Ordinal);
        LoadCommFromRepository();
        LoadPasswordsFromRepository();
        RebuildGrid();
    }

    /// <summary>F2：驗證後寫入。</summary>
    public bool TrySaveToDatabase()
    {
        if (!TryValidateGridInputs(out string? parseErr))
        {
            MessageBox.Show(parseErr, _localization.GetString("AppName"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        MergeGridIntoMaster();
        if (!ValidatePlcRanges(out string? rangeErr))
        {
            MessageBox.Show(rangeErr, _localization.GetString("AppName"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        try
        {
            SaveCommToRepository();
            SaveUserPasswordsToRepository();
            _otherPlc.SaveValues(_plcValues);
            _names.ReplaceAll(_displayJson);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                (_localization.GetString("SettingsTab3_SaveFailed") ?? "Save failed.") + "\n" + ex.Message,
                _localization.GetString("AppName"),
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
        if (!TryValidateGridInputs(out string? parseErr))
        {
            MessageBox.Show(parseErr, _localization.GetString("AppName"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        MergeGridIntoMaster();
        if (!ValidatePlcRanges(out string? rangeErr))
        {
            MessageBox.Show(rangeErr, _localization.GetString("AppName"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        try
        {
            SaveCommToRepository(connection, transaction);
            SaveUserPasswordsToRepository(connection, transaction);
            _otherPlc.SaveValues(_plcValues, connection, transaction);
            _names.ReplaceAll(_displayJson, connection, transaction);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                (_localization.GetString("SettingsTab3_SaveFailed") ?? "Save failed.") + "\n" + ex.Message,
                _localization.GetString("AppName"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return false;
        }
    }

    private void LoadCommFromRepository()
    {
        IReadOnlyDictionary<string, PlcCommChannelDto> map = _comm.LoadAll();
        if (map.TryGetValue(PlcCommChannelRole.Printer, out PlcCommChannelDto? p))
        {
            PrinterPort = p.PortName;
        }

        if (map.TryGetValue(PlcCommChannelRole.Panel, out PlcCommChannelDto? pan))
        {
            PanelPort = pan.PortName;
        }

        if (map.TryGetValue(PlcCommChannelRole.Plc, out PlcCommChannelDto? g))
        {
            GluePort = g.PortName;
            GlueBaudRate = g.BaudRate;
            GlueByteSize = g.ByteSize;
            GlueParity = g.Parity;
            GlueStopBits = g.StopBits;
        }
    }

    private void SaveCommToRepository()
    {
        var dict = new Dictionary<string, PlcCommChannelDto>(StringComparer.Ordinal)
        {
            [PlcCommChannelRole.Printer] = new PlcCommChannelDto
            {
                ChannelRole = PlcCommChannelRole.Printer,
                PortName = PrinterPort,
                BaudRate = 115200,
                ByteSize = 8,
                Parity = "None",
                StopBits = 1
            },
            [PlcCommChannelRole.Panel] = new PlcCommChannelDto
            {
                ChannelRole = PlcCommChannelRole.Panel,
                PortName = PanelPort,
                BaudRate = 115200,
                ByteSize = 8,
                Parity = "None",
                StopBits = 1
            },
            [PlcCommChannelRole.Plc] = new PlcCommChannelDto
            {
                ChannelRole = PlcCommChannelRole.Plc,
                PortName = GluePort,
                BaudRate = GlueBaudRate,
                ByteSize = GlueByteSize,
                Parity = GlueParity,
                StopBits = GlueStopBits
            }
        };
        _comm.SaveAll(dict);
    }

    private void SaveCommToRepository(SqliteConnection connection, SqliteTransaction transaction)
    {
        var dict = new Dictionary<string, PlcCommChannelDto>(StringComparer.Ordinal)
        {
            [PlcCommChannelRole.Printer] = new PlcCommChannelDto
            {
                ChannelRole = PlcCommChannelRole.Printer,
                PortName = PrinterPort,
                BaudRate = 115200,
                ByteSize = 8,
                Parity = "None",
                StopBits = 1
            },
            [PlcCommChannelRole.Panel] = new PlcCommChannelDto
            {
                ChannelRole = PlcCommChannelRole.Panel,
                PortName = PanelPort,
                BaudRate = 115200,
                ByteSize = 8,
                Parity = "None",
                StopBits = 1
            },
            [PlcCommChannelRole.Plc] = new PlcCommChannelDto
            {
                ChannelRole = PlcCommChannelRole.Plc,
                PortName = GluePort,
                BaudRate = GlueBaudRate,
                ByteSize = GlueByteSize,
                Parity = GlueParity,
                StopBits = GlueStopBits
            }
        };
        _comm.SaveAll(dict, connection, transaction);
    }

    private void LoadPasswordsFromRepository()
    {
        IReadOnlyList<UserDirectoryRow> all = _users.LoadAll();

        UserDirectoryRow? user = all.FirstOrDefault(u => string.Equals(u.Username, "aimarliu", StringComparison.OrdinalIgnoreCase));
        UserDirectoryRow? admin = all.FirstOrDefault(u => string.Equals(u.Username, "Admin", StringComparison.OrdinalIgnoreCase));

        _userId = user?.Id;
        _adminId = admin?.Id;

        UserPassword = user?.Password ?? "";
        AdminPassword = admin?.Password ?? "";
    }

    private void SaveUserPasswordsToRepository()
    {
        if (string.IsNullOrWhiteSpace(_userId) || string.IsNullOrWhiteSpace(_adminId))
            throw new InvalidOperationException("Tab3 密碼使用者資料不存在：期望 Username 為 'aimarliu' 與 'Admin'。");

        var rows = new List<UserDirectoryRow>
        {
            new UserDirectoryRow { Id = _userId!, Username = "aimarliu", Password = UserPassword ?? string.Empty },
            new UserDirectoryRow { Id = _adminId!, Username = "Admin", Password = AdminPassword ?? string.Empty }
        };

        _users.SavePasswords(rows);
    }

    private void SaveUserPasswordsToRepository(SqliteConnection connection, SqliteTransaction transaction)
    {
        if (string.IsNullOrWhiteSpace(_userId) || string.IsNullOrWhiteSpace(_adminId))
            throw new InvalidOperationException("Tab3 密碼使用者資料不存在：期望 Username 為 'aimarliu' 與 'Admin'。");

        var rows = new List<UserDirectoryRow>
        {
            new UserDirectoryRow { Id = _userId!, Username = "aimarliu", Password = UserPassword ?? string.Empty },
            new UserDirectoryRow { Id = _adminId!, Username = "Admin", Password = AdminPassword ?? string.Empty }
        };

        _users.SavePasswords(rows, connection, transaction);
    }

    private void MergeGridIntoMaster()
    {
        string culture = _localization.CurrentCulture.Name;
        foreach (Tab3GridRowViewModel row in GridRows)
        {
            TryParseOptional(row.MaxText, out double? maxV);
            TryParseOptional(row.MinText, out double? minV);
            TryParseOptional(row.AccurateText, out double? accV);
            _plcValues[$"{row.KeyPrefix}.Max"] = maxV;
            _plcValues[$"{row.KeyPrefix}.Min"] = minV;
            _plcValues[$"{row.KeyPrefix}.Accurate"] = accV;

            string composite = ComponentDisplayNameRepository.MakeKey(row.DepartmentCode, row.ComponentCode);
            string defaultName = _localization.GetString(Tab3PlcCatalog.GetDefaultDisplayNameKey(row.DepartmentCode, row.ComponentCode))
                                 ?? row.ComponentCode;
            if (string.Equals(row.DisplayName.Trim(), defaultName.Trim(), StringComparison.Ordinal))
            {
                _displayJson.Remove(composite);
                continue;
            }

            Dictionary<string, string> bag = ReadJsonBag(composite);
            bag[culture] = row.DisplayName.Trim();
            _displayJson[composite] = JsonSerializer.Serialize(bag);
        }
    }

    private Dictionary<string, string> ReadJsonBag(string compositeKey)
    {
        if (!_displayJson.TryGetValue(compositeKey, out string? json) || string.IsNullOrWhiteSpace(json))
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                   ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        catch
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
    }

    private void RebuildGrid()
    {
        GridRows.Clear();
        bool showDept = SelectedDepartmentCode == "All";
        foreach (Tab3PlcCatalog.ComponentTriplet meta in Tab3PlcCatalog.GetRowsForDepartment(SelectedDepartmentCode))
        {
            var row = new Tab3GridRowViewModel(meta.DepartmentCode, meta.ComponentCode, meta.KeyPrefix, showDept);
            row.DisplayName = ResolveDisplayName(meta);
            row.MaxText = FormatDouble(_plcValues.GetValueOrDefault($"{meta.KeyPrefix}.Max"));
            row.MinText = FormatDouble(_plcValues.GetValueOrDefault($"{meta.KeyPrefix}.Min"));
            row.AccurateText = FormatDouble(_plcValues.GetValueOrDefault($"{meta.KeyPrefix}.Accurate"));
            GridRows.Add(row);
        }
    }

    private string ResolveDisplayName(Tab3PlcCatalog.ComponentTriplet meta)
    {
        string composite = ComponentDisplayNameRepository.MakeKey(meta.DepartmentCode, meta.ComponentCode);
        string culture = _localization.CurrentCulture.Name;
        string resxDefault = _localization.GetString(Tab3PlcCatalog.GetDefaultDisplayNameKey(meta.DepartmentCode, meta.ComponentCode))
                           ?? meta.ComponentCode;
        if (_displayJson.TryGetValue(composite, out string? json) && !string.IsNullOrWhiteSpace(json))
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty(culture, out JsonElement el))
                {
                    string? s = el.GetString();
                    if (!string.IsNullOrEmpty(s))
                        return s;
                }
            }
            catch
            {
                // fall through
            }
        }

        return resxDefault;
    }

    private static string FormatDouble(double? v)
    {
        if (!v.HasValue) return "";
        double x = v.Value;
        if (double.IsNaN(x) || double.IsInfinity(x)) return "";
        return x.ToString("0.######", CultureInfo.InvariantCulture);
    }

    private static bool TryParseOptional(string? text, out double? value)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            value = null;
            return true;
        }

        if (double.TryParse(text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
        {
            value = d;
            return true;
        }

        value = null;
        return false;
    }

    private bool TryValidateGridInputs(out string? error)
    {
        foreach (Tab3GridRowViewModel row in GridRows)
        {
            if (!TryParseOptional(row.MaxText, out _))
            {
                error = string.Format(CultureInfo.InvariantCulture, _localization.GetString("SettingsTab3_Validate_Number") ?? "Invalid number: {0}", $"{row.KeyPrefix}.Max");
                return false;
            }

            if (!TryParseOptional(row.MinText, out _))
            {
                error = string.Format(CultureInfo.InvariantCulture, _localization.GetString("SettingsTab3_Validate_Number") ?? "Invalid number: {0}", $"{row.KeyPrefix}.Min");
                return false;
            }

            if (!TryParseOptional(row.AccurateText, out _))
            {
                error = string.Format(CultureInfo.InvariantCulture, _localization.GetString("SettingsTab3_Validate_Number") ?? "Invalid number: {0}", $"{row.KeyPrefix}.Accurate");
                return false;
            }
        }

        error = null;
        return true;
    }

    private bool ValidatePlcRanges(out string? error)
    {
        foreach (Tab3PlcCatalog.ComponentTriplet meta in Tab3PlcCatalog.GetRowsForDepartment("All"))
        {
            double? max = _plcValues.GetValueOrDefault($"{meta.KeyPrefix}.Max");
            double? min = _plcValues.GetValueOrDefault($"{meta.KeyPrefix}.Min");
            if (max.HasValue && min.HasValue && min > max)
            {
                error = _localization.GetString("SettingsTab3_Validate_MinMax") ?? "Min must be <= Max.";
                return false;
            }
        }

        error = null;
        return true;
    }

    private static IReadOnlyList<string> BuildComPorts()
    {
        var list = new List<string> { "NONE" };
        for (int i = 1; i <= 20; i++)
            list.Add($"COM{i}");
        return list;
    }

    private static string MapDepartmentToResxKey(string code) => code switch
    {
        "Knife II" => "SettingsTab3_Dept_KnifeII",
        _ => $"SettingsTab3_Dept_{code.Replace(" ", "", StringComparison.Ordinal)}"
    };
}

public sealed class Tab3DepartmentItem
{
    public required string Code { get; init; }
    public required LocalizedString Label { get; init; }
}
