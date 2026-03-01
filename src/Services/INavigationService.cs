namespace CursorTestApp.Services;

/// <summary>
/// 導航服務介面，用於切換 View。
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// 導航至主畫面（Layout1）。
    /// </summary>
    void NavigateToMain();

    /// <summary>
    /// 導航至登入畫面。
    /// </summary>
    void NavigateToLogin();

    /// <summary>
    /// 導航至 Layout2（生產資訊看板／排程）。
    /// </summary>
    void NavigateToLayout2();

    /// <summary>
    /// 導航至訂單製作（主內容區頁面，與 Login、Layout2 同）。
    /// </summary>
    void NavigateToOrderMaking();

    /// <summary>
    /// LogPanel 顯示與否變更時觸發。主畫面不需 Log，登入頁需要。
    /// </summary>
    event EventHandler<bool>? LogPanelVisibilityChanged;
}
