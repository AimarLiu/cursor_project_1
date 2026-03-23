using CommunityToolkit.Mvvm.ComponentModel;

namespace CursorTestApp.ViewModels;

/// <summary>Tab3 左側 DataGrid 單列（元件顯示名 + Max/Min/Accurate）。</summary>
public sealed partial class Tab3GridRowViewModel : ObservableObject
{
    public string DepartmentCode { get; }
    public string ComponentCode { get; }
    /// <summary>例：<c>Feed.DriveSideBaffle</c>（不含後綴）。</summary>
    public string KeyPrefix { get; }

    [ObservableProperty]
    private string _displayName = "";

    [ObservableProperty]
    private string _maxText = "";

    [ObservableProperty]
    private string _minText = "";

    [ObservableProperty]
    private string _accurateText = "";

    [ObservableProperty]
    private bool _showDepartmentColumn;

    public Tab3GridRowViewModel(
        string departmentCode,
        string componentCode,
        string keyPrefix,
        bool showDepartmentColumn)
    {
        DepartmentCode = departmentCode;
        ComponentCode = componentCode;
        KeyPrefix = keyPrefix;
        ShowDepartmentColumn = showDepartmentColumn;
    }
}
