using System.Windows;

namespace CursorTestApp.Views.Layout2;

/// <summary>
/// F4 預覽搜尋 Dialog（確定/取消）。
/// </summary>
public partial class F4SearchDialog : Window
{
    public F4SearchDialog()
    {
        InitializeComponent();
        Owner = Application.Current.MainWindow;
    }

    public string? Keyword => TbKeyword?.Text?.Trim();

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
