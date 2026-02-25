using System.IO;
using System.Windows;
using CursorTestApp.Helpers;
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

        Application.Current.DispatcherUnhandledException += (_, args) =>
        {
            var logDir = AppDomain.CurrentDomain.BaseDirectory;
            var logFileName = $"error_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";
            var logPath = Path.Combine(logDir, logFileName);

            try
            {
                var content = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 未處理的錯誤\r\n\r\n{ExceptionFormatHelper.ToDisplayString(args.Exception)}";
                File.WriteAllText(logPath, content);
                MessageBox.Show(
                    $"未處理的錯誤，詳情已寫入：\n{logFileName}",
                    "程式錯誤",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (Exception writeEx)
            {
                MessageBox.Show(
                    $"未處理的錯誤，且無法寫入日誌：{writeEx.Message}\n\n{args.Exception.Message}",
                    "程式錯誤",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            args.Handled = true; // 避免程式直接結束，可改為 false 讓程式結束
        };

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

