using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CursorTestApp.Helpers;
using CursorTestApp.ViewModels;

namespace CursorTestApp.Views.Settings.Tabs;

public partial class SettingsTab3OtherPlcParametersView : UserControl
{
    private SettingsTab3ViewModel? _vm;

    public SettingsTab3OtherPlcParametersView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        DataContextChanged += OnDataContextChanged;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AttachAndRefresh();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        DetachLocalizedHandlers();
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        AttachAndRefresh();
    }

    private void AttachAndRefresh()
    {
        if (DataContext is not SettingsTab3ViewModel vm)
        {
            DetachLocalizedHandlers();
            return;
        }

        if (ReferenceEquals(_vm, vm))
        {
            RefreshColumnHeaders();
            return;
        }

        DetachLocalizedHandlers();
        _vm = vm;
        Hook(vm.ColumnDepartment);
        Hook(vm.ColumnComponentName);
        Hook(vm.ColumnMax);
        Hook(vm.ColumnMin);
        Hook(vm.ColumnAccurate);
        RefreshColumnHeaders();
    }

    private void Hook(LocalizedString text)
    {
        text.PropertyChanged += OnLocalizedChanged;
    }

    private void Unhook(LocalizedString text)
    {
        text.PropertyChanged -= OnLocalizedChanged;
    }

    private void OnLocalizedChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LocalizedString.Value))
        {
            RefreshColumnHeaders();
        }
    }

    private void DetachLocalizedHandlers()
    {
        if (_vm is null) return;
        Unhook(_vm.ColumnDepartment);
        Unhook(_vm.ColumnComponentName);
        Unhook(_vm.ColumnMax);
        Unhook(_vm.ColumnMin);
        Unhook(_vm.ColumnAccurate);
        _vm = null;
    }

    private void RefreshColumnHeaders()
    {
        if (_vm is null) return;
        ColDepartment.Header = _vm.ColumnDepartment.Value;
        ColComponentName.Header = _vm.ColumnComponentName.Value;
        ColMax.Header = _vm.ColumnMax.Value;
        ColMin.Header = _vm.ColumnMin.Value;
        ColAccurate.Header = _vm.ColumnAccurate.Value;
    }
}
