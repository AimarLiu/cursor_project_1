using System.ComponentModel;

namespace CursorTestApp.Models;

/// <summary>
/// Phase 6 訂單製作：Orders 表一筆。
/// </summary>
public sealed class Order : INotifyPropertyChanged
{
    private int _id;
    private DateTime _createdAt;
    private string _orderNo = string.Empty;
    private string _versionNo = string.Empty;
    private int _orderQuantity;
    private string _boxType = string.Empty;
    private string _category = string.Empty;
    private string _phase1 = string.Empty;
    private string _phase2 = string.Empty;
    private string _phase3 = string.Empty;
    private int _length;
    private int _width;
    private string _customerName = string.Empty;
    private string _remarks = string.Empty;

    public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
    public DateTime CreatedAt { get => _createdAt; set { _createdAt = value; OnPropertyChanged(); } }
    public string OrderNo { get => _orderNo; set { _orderNo = value ?? string.Empty; OnPropertyChanged(); } }
    public string VersionNo { get => _versionNo; set { _versionNo = value ?? string.Empty; OnPropertyChanged(); } }
    public int OrderQuantity { get => _orderQuantity; set { _orderQuantity = value; OnPropertyChanged(); } }
    public string BoxType { get => _boxType; set { _boxType = value ?? string.Empty; OnPropertyChanged(); } }
    public string Category { get => _category; set { _category = value ?? string.Empty; OnPropertyChanged(); } }
    public string Phase1 { get => _phase1; set { _phase1 = value ?? string.Empty; OnPropertyChanged(); } }
    public string Phase2 { get => _phase2; set { _phase2 = value ?? string.Empty; OnPropertyChanged(); } }
    public string Phase3 { get => _phase3; set { _phase3 = value ?? string.Empty; OnPropertyChanged(); } }
    public int Length { get => _length; set { _length = value; OnPropertyChanged(); } }
    public int Width { get => _width; set { _width = value; OnPropertyChanged(); } }
    public string CustomerName { get => _customerName; set { _customerName = value ?? string.Empty; OnPropertyChanged(); } }
    public string Remarks { get => _remarks; set { _remarks = value ?? string.Empty; OnPropertyChanged(); } }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
