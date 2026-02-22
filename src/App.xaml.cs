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

        IDatabaseService databaseService = new DatabaseService();
        databaseService.Initialize();
    }
}

