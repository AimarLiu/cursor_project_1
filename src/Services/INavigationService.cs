namespace CursorTestApp.Services;

/// <summary>
/// 導航服務介面，用於切換 View。
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// 導航至主畫面（登入成功後）。
    /// </summary>
    void NavigateToMain();

    /// <summary>
    /// 導航至登入畫面。
    /// </summary>
    void NavigateToLogin();

    /// <summary>
    /// LogPanel 顯示與否變更時觸發。主畫面不需 Log，登入頁需要。
    /// </summary>
    event EventHandler<bool>? LogPanelVisibilityChanged;
}
