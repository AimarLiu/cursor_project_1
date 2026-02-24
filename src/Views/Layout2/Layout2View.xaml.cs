using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using CursorTestApp.ViewModels;
using WpfAnimatedGif;

namespace CursorTestApp.Views.Layout2;

/// <summary>
/// Layout2 主畫面（生產資訊看板、排程、完成訂單）。
/// </summary>
public partial class Layout2View : UserControl
{
    public Layout2View()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is Layout2ViewModel vm && vm.Rs485 != null)
        {
            vm.Rs485.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(Services.IRs485Service.HasError))
                    UpdateAlarmImage(vm.Rs485.HasError);
            };
        }
    }

    private void UpdateAlarmImage(bool hasError)
    {
        const string gifUri = "pack://application:,,,/CursorTestApp;component/Resources/Icons/red-alam.gif";
        const string pngUri = "pack://application:,,,/CursorTestApp;component/Resources/Icons/alarm.png";

        if (hasError)
        {
            ImageBehavior.SetAnimatedSource(AlarmImage, null);
            var bitmap = new System.Windows.Media.Imaging.BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(gifUri, UriKind.Absolute);
            bitmap.EndInit();
            ImageBehavior.SetAnimatedSource(AlarmImage, bitmap);
            ImageBehavior.SetRepeatBehavior(AlarmImage, RepeatBehavior.Forever);
        }
        else
        {
            ImageBehavior.SetAnimatedSource(AlarmImage, null);
            AlarmImage.Source = null;
            AlarmImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(pngUri, UriKind.Absolute));
        }
    }

    private void BtnF4_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is not Layout2ViewModel vm) return;
        var dialog = new F4SearchDialog();
        if (dialog.ShowDialog() != true) return;
        vm.SearchAndSelectSchedule(dialog.Keyword ?? "");
    }
}
