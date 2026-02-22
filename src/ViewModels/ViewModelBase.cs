using CommunityToolkit.Mvvm.ComponentModel;

namespace CursorTestApp.ViewModels;

/// <summary>
/// ViewModel 基底類別，提供 INotifyPropertyChanged 與 MVVM 基礎能力。
/// 使用 CommunityToolkit.Mvvm 的 ObservableObject 實作。
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
}
