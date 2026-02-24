using System.ComponentModel;

namespace CursorTestApp.Models;

/// <summary>
/// 生產完成訂單一筆（DataGrid 下區），含日期時間。
/// </summary>
public sealed class CompletedOrderItem : INotifyPropertyChanged
{
    private DateTime _completedAt;
    private string _orderNo = string.Empty;
    private string _versionNo = string.Empty;
    private int _orderQuantity;
    private string _boxType = string.Empty;
    private string _category = string.Empty;
    private string _customerName = string.Empty;
    private string _remarks = string.Empty;

    /// <summary>完成日期時間</summary>
    public DateTime CompletedAt { get => _completedAt; set { _completedAt = value; OnPropertyChanged(); OnPropertyChanged(nameof(CompletedAtDisplay)); } }
    /// <summary>完成時間顯示用（yyyy/MM/dd HH:mm:ss）</summary>
    public string CompletedAtDisplay => _completedAt.ToString("yyyy/MM/dd HH:mm:ss");
    /// <summary>生產訂單號</summary>
    public string OrderNo { get => _orderNo; set { _orderNo = value ?? string.Empty; OnPropertyChanged(); } }
    /// <summary>版號</summary>
    public string VersionNo { get => _versionNo; set { _versionNo = value ?? string.Empty; OnPropertyChanged(); } }
    /// <summary>受訂量</summary>
    public int OrderQuantity { get => _orderQuantity; set { _orderQuantity = value; OnPropertyChanged(); } }
    /// <summary>箱型</summary>
    public string BoxType { get => _boxType; set { _boxType = value ?? string.Empty; OnPropertyChanged(); } }
    /// <summary>類別</summary>
    public string Category { get => _category; set { _category = value ?? string.Empty; OnPropertyChanged(); } }
    /// <summary>客戶名稱</summary>
    public string CustomerName { get => _customerName; set { _customerName = value ?? string.Empty; OnPropertyChanged(); } }
    /// <summary>備註</summary>
    public string Remarks { get => _remarks; set { _remarks = value ?? string.Empty; OnPropertyChanged(); } }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
