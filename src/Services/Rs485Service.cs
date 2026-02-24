using System.ComponentModel;

namespace CursorTestApp.Services;

/// <summary>
/// RS485 服務實作。目前為模擬用，實際連線可後續接上。
/// </summary>
public sealed class Rs485Service : IRs485Service, INotifyPropertyChanged
{
    private bool _isOptActive;
    private bool _isPlcActive;
    private int _speed;
    private bool _hasError;

    /// <inheritdoc />
    public bool IsOptActive { get => _isOptActive; set { _isOptActive = value; OnPropertyChanged(); } }
    /// <inheritdoc />
    public bool IsPlcActive { get => _isPlcActive; set { _isPlcActive = value; OnPropertyChanged(); } }
    /// <inheritdoc />
    public int Speed { get => _speed; set { _speed = value; OnPropertyChanged(); } }
    /// <inheritdoc />
    public bool HasError { get => _hasError; set { _hasError = value; OnPropertyChanged(); } }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
