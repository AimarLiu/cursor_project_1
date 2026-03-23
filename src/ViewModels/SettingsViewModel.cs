using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CursorTestApp.Helpers;
using CursorTestApp.Services;
using Microsoft.Data.Sqlite;

namespace CursorTestApp.ViewModels;

/// <summary>
/// Phase 7 Settings 主頁 ViewModel：分頁標籤、底部 F 鍵列與導航。
/// </summary>
public sealed partial class SettingsViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly ILogService _logService;
    private readonly ILocalizationService _localizationService;
    private readonly IDatabaseService _databaseService;

    /// <summary>
    /// Tab1～Tab6 分頁標題（RESX）。
    /// </summary>
    public LocalizedString Tab1Header { get; }
    public LocalizedString Tab2Header { get; }
    public LocalizedString Tab3Header { get; }
    public LocalizedString Tab4Header { get; }
    public LocalizedString Tab5Header { get; }
    public LocalizedString Tab6Header { get; }

    /// <summary>
    /// 各分頁佔位說明（骨架階段）。
    /// </summary>
    public LocalizedString Tab1Body { get; }
    public LocalizedString Tab2Body { get; }
    public LocalizedString Tab3Body { get; }
    public LocalizedString Tab4Body { get; }
    public LocalizedString Tab5Body { get; }
    public LocalizedString Tab6Body { get; }

    public LocalizedString FooterF1 { get; }
    public LocalizedString FooterF2 { get; }
    public LocalizedString FooterF3 { get; }
    public LocalizedString FooterF5 { get; }
    public LocalizedString FooterF6 { get; }

    /// <summary>
    /// 目前選取之設定分頁索引（0～5）。自訂左欄 ListBox + 右欄內容，避免 WPF TabControl 左置時標題列被拉滿寬。
    /// </summary>
    [ObservableProperty]
    private int _selectedSettingsTabIndex;

    /// <summary>
    /// Tab1 刀位安全閾值（子 ViewModel）。
    /// </summary>
    public SettingsTab1KnifeViewModel Tab1Knife { get; }

    /// <summary>
    /// Tab2 紙箱／DieCutter 參數（子 ViewModel）。
    /// </summary>
    public SettingsTab2BoxViewModel Tab2Box { get; }

    /// <summary>
    /// Tab3 其他參數（通訊／部門 DataGrid／使用者密碼）。
    /// </summary>
    public SettingsTab3ViewModel Tab3Other { get; }

    /// <summary>
    /// 建立 SettingsViewModel。
    /// </summary>
    public SettingsViewModel(
        IDatabaseService databaseService,
        INavigationService navigationService,
        ILogService logService,
        ILocalizationService localizationService,
        IKnifeSafetyParameterRepository knifeSafetyParameterRepository,
        IBoxDieCutterSettingsRepository boxDieCutterSettingsRepository,
        IOtherPlcParametersRepository otherPlcParametersRepository,
        IPlcCommChannelRepository plcCommChannelRepository,
        IComponentDisplayNameRepository componentDisplayNameRepository,
        IUserDirectoryRepository userDirectoryRepository)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
        ArgumentNullException.ThrowIfNull(knifeSafetyParameterRepository);
        ArgumentNullException.ThrowIfNull(boxDieCutterSettingsRepository);
        ArgumentNullException.ThrowIfNull(otherPlcParametersRepository);
        ArgumentNullException.ThrowIfNull(plcCommChannelRepository);
        ArgumentNullException.ThrowIfNull(componentDisplayNameRepository);
        ArgumentNullException.ThrowIfNull(userDirectoryRepository);

        Tab1Knife = new SettingsTab1KnifeViewModel(knifeSafetyParameterRepository, localizationService);
        Tab2Box = new SettingsTab2BoxViewModel(boxDieCutterSettingsRepository, localizationService);
        Tab3Other = new SettingsTab3ViewModel(
            otherPlcParametersRepository,
            plcCommChannelRepository,
            componentDisplayNameRepository,
            userDirectoryRepository,
            localizationService);

        Tab1Header = new LocalizedString(localizationService, "SettingsTab1_TabHeader");
        Tab2Header = new LocalizedString(localizationService, "SettingsTab2_TabHeader");
        Tab3Header = new LocalizedString(localizationService, "SettingsTab3_TabHeader");
        Tab4Header = new LocalizedString(localizationService, "SettingsTab4_TabHeader");
        Tab5Header = new LocalizedString(localizationService, "SettingsTab5_TabHeader");
        Tab6Header = new LocalizedString(localizationService, "SettingsTab6_TabHeader");

        Tab1Body = new LocalizedString(localizationService, "SettingsTab1_SkeletonBody");
        Tab2Body = new LocalizedString(localizationService, "SettingsTab2_SkeletonBody");
        Tab3Body = new LocalizedString(localizationService, "SettingsTab3_SkeletonBody");
        Tab4Body = new LocalizedString(localizationService, "SettingsTab4_SkeletonBody");
        Tab5Body = new LocalizedString(localizationService, "SettingsTab5_SkeletonBody");
        Tab6Body = new LocalizedString(localizationService, "SettingsTab6_SkeletonBody");

        FooterF1 = new LocalizedString(localizationService, "Settings_Footer_F1_Exit");
        FooterF2 = new LocalizedString(localizationService, "Settings_Footer_F2_Save");
        FooterF3 = new LocalizedString(localizationService, "Settings_Footer_F3_Cancel");
        FooterF5 = new LocalizedString(localizationService, "Settings_Footer_F5_MachineDimension");
        FooterF6 = new LocalizedString(localizationService, "Settings_Footer_F6_SpecialParams");
    }

    [RelayCommand]
    private void FooterF1Exit()
    {
        _logService.Append("Settings：F1 離開 → Login");
        _navigationService.NavigateToLogin();
    }

    [RelayCommand]
    private void FooterF2Save()
    {
        using var connection = new SqliteConnection($"Data Source={_databaseService.DatabasePath}");
        connection.Open();
        using var tx = connection.BeginTransaction();

        try
        {
            if (!Tab1Knife.TrySaveToDatabase(connection, tx))
            {
                tx.Rollback();
                return;
            }
            if (!Tab2Box.TrySaveToDatabase(connection, tx))
            {
                tx.Rollback();
                return;
            }
            if (!Tab3Other.TrySaveToDatabase(connection, tx))
            {
                tx.Rollback();
                return;
            }

            tx.Commit();
            _logService.Append(_localizationService.GetString("SettingsTab123_SaveOk") ?? "Tab1, Tab2, and Tab3 saved.");
        }
        catch (Exception)
        {
            tx.Rollback();
            throw;
        }
    }

    [RelayCommand]
    private void FooterF3Cancel()
    {
        Tab1Knife.ReloadFromDatabase();
        Tab2Box.ReloadFromDatabase();
        Tab3Other.ReloadFromDatabase();
        _logService.Append(_localizationService.GetString("SettingsTab123_ReloadOk") ?? "Tab1 and Tab2 reloaded.");
    }

    [RelayCommand]
    private void FooterF5MachineDimension()
    {
        _logService.Append("Settings：F5 機器尺寸（尚未實作）");
    }

    [RelayCommand]
    private void FooterF6SpecialParams()
    {
        _logService.Append("Settings：F6 特殊參數區（尚未實作）");
    }
}
