using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CursorTestApp.Helpers;
using CursorTestApp.Models;
using CursorTestApp.Services;

namespace CursorTestApp.ViewModels;

/// <summary>
/// 登入畫面 ViewModel。
/// </summary>
public sealed partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly ILogService _logService;
    private readonly INavigationService _navigationService;
    private readonly ILocalizationService _localizationService;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private LanguageOption? _selectedLanguage;

    /// <summary>
    /// 登入錯誤訊息（密碼錯誤或請輸入密碼），空白時不顯示。
    /// </summary>
    [ObservableProperty]
    private string _loginErrorMessage = string.Empty;

    /// <summary>
    /// 支援的語系清單。
    /// </summary>
    public ObservableCollection<LanguageOption> SupportedLanguages { get; } = new(SupportedCultures.Options);

    /// <summary>
    /// 登入標題（動態語系）。
    /// </summary>
    public LocalizedString LoginTitle { get; }

    /// <summary>
    /// 密碼輸入佔位文字（動態語系）。
    /// </summary>
    public LocalizedString PasswordPlaceholder { get; }

    /// <summary>
    /// Enter 按鈕文字（動態語系）。
    /// </summary>
    public LocalizedString EnterButton { get; }

    /// <summary>
    /// Cancel 按鈕文字（動態語系）。
    /// </summary>
    public LocalizedString CancelButton { get; }

    /// <summary>
    /// 離開按鈕 ToolTip（動態語系）。
    /// </summary>
    public LocalizedString ExitToolTip { get; }

    /// <summary>
    /// 建立 LoginViewModel。
    /// </summary>
    public LoginViewModel(
        IAuthService authService,
        ILogService logService,
        INavigationService navigationService,
        ILocalizationService localizationService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));

        LoginTitle = new LocalizedString(_localizationService, "LoginTitle");
        PasswordPlaceholder = new LocalizedString(_localizationService, "PasswordPlaceholder");
        EnterButton = new LocalizedString(_localizationService, "EnterButton");
        CancelButton = new LocalizedString(_localizationService, "CancelButton");
        ExitToolTip = new LocalizedString(_localizationService, "ExitToolTip");

        SelectedLanguage = SupportedCultures.ResolveFromSystem();
    }

    [RelayCommand]
    private void Login()
    {
        LoginErrorMessage = string.Empty;
        _logService.Append("正在驗證…");

        if (string.IsNullOrWhiteSpace(Password))
        {
            LoginErrorMessage = _localizationService.GetString("LoginErrorEmptyPassword") ?? "請輸入密碼。";
            _logService.Append(LoginErrorMessage);
            return;
        }

        var user = _authService.ValidatePassword(Password);
        if (user != null)
        {
            _logService.Append($"登入成功：{user.Username}");
            try
            {
                if (string.Equals(user.Username, "Admin", StringComparison.Ordinal))
                {
                    _logService.Append("導航至 Settings（Phase 7）");
                    _navigationService.NavigateToSettings();
                }
                else
                {
                    _logService.Append("導航至 Layout2");
                    _navigationService.NavigateToLayout2();
                }
            }
            catch (Exception ex)
            {
                _logService.Append($"導航失敗：{ex.Message}");
                LoginErrorMessage = ex.Message;
                MessageBox.Show("無法進入主畫面：\n\n" + Helpers.ExceptionFormatHelper.ToDisplayString(ex), "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            LoginErrorMessage = _localizationService.GetString("LoginErrorWrongPassword") ?? "密碼錯誤。";
            _logService.Append($"登入失敗：{LoginErrorMessage}");
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        Password = string.Empty;
        LoginErrorMessage = string.Empty;
        _logService.Append("已清除輸入");
    }

    partial void OnPasswordChanged(string value)
    {
        if (!string.IsNullOrEmpty(LoginErrorMessage))
            LoginErrorMessage = string.Empty;
    }

    [RelayCommand]
    private void Exit()
    {
        Application.Current.Shutdown();
    }

    partial void OnSelectedLanguageChanged(LanguageOption? value)
    {
        if (value != null)
        {
            _localizationService.SetCulture(value.Culture);
            _logService.Append($"語系已切換：{value.DisplayName}");
        }
    }
}
