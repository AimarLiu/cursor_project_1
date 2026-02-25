using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Threading;
using CursorTestApp.Services;

namespace CursorTestApp.Views.Shared;

/// <summary>
/// 右下角 Log 區域，顯示 LogMessages 集合。支援捲動與新增訊息時自動捲到底。
/// </summary>
public partial class LogPanel : UserControl
{
    private ILogService? _subscribedLogService;

    public LogPanel()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        DataContextChanged += OnDataContextChangedHandler;
    }

    private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        SubscribeToLogMessages();
    }

    private void OnUnloaded(object sender, System.Windows.RoutedEventArgs e)
    {
        UnsubscribeFromLogMessages();
    }

    private void OnDataContextChangedHandler(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        UnsubscribeFromLogMessages();
        SubscribeToLogMessages();
    }

    private void SubscribeToLogMessages()
    {
        if (DataContext is not ILogService logService)
            return;

        logService.LogMessages.CollectionChanged += OnLogMessagesCollectionChanged;
        _subscribedLogService = logService;
    }

    private void UnsubscribeFromLogMessages()
    {
        if (_subscribedLogService != null)
        {
            _subscribedLogService.LogMessages.CollectionChanged -= OnLogMessagesCollectionChanged;
            _subscribedLogService = null;
        }
    }

    private void OnLogMessagesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.NewItems?.Count > 0 && LogListBox.Items.Count > 0)
        {
            var lastItem = LogListBox.Items[LogListBox.Items.Count - 1];
            // 延後至 Layout 之後再捲動，避免在 CollectionChanged 內觸發 layout 導致異機「ItemsControl 與其項目來源不一致」
            LogListBox.Dispatcher.BeginInvoke(() =>
            {
                if (LogListBox.Items.Count > 0)
                    LogListBox.ScrollIntoView(LogListBox.Items[LogListBox.Items.Count - 1]);
            }, DispatcherPriority.Loaded);
        }
    }
}
