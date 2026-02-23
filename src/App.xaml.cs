using System.Windows;
using CursorTestApp.Services;

namespace CursorTestApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <inheritdoc />
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            IDatabaseService databaseService = new DatabaseService();
            databaseService.Initialize();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"資料庫初始化失敗：{ex.Message}",
                "錯誤",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
        }
    }
}

