# 實作摘要 (Implementation Summary)

本文件記錄 Phase 1.2、Phase 1.3、Phase 2、Phase 3 與 Phase 4 的完成實作，供後續開發參考。

---

## Phase 1.2：MVVM 架構

### 實作內容

| 項目 | 說明 | 檔案路徑 |
|------|------|----------|
| ViewModel 基底 | 繼承 `CommunityToolkit.Mvvm.ObservableObject`，提供 `INotifyPropertyChanged` | `src/ViewModels/ViewModelBase.cs` |
| 專案資料夾結構 | Views、ViewModels、Models、Services、Helpers | `src/` 目錄下 |

### 程式碼要點

- **ViewModelBase**：所有 ViewModel 繼承自 `ViewModelBase`，自動具備屬性變更通知能力
- **依賴**：使用 NuGet `CommunityToolkit.Mvvm`（已於 Phase 0 安裝）
- **風格**：遵循 C# 規範，採用 file-scoped namespace

### 使用範例

```csharp
public class LoginViewModel : ViewModelBase
{
    // 屬性變更會自動通知 UI
}
```

---

## Phase 1.3：多國語系結構

### 實作內容

| 項目 | 說明 | 檔案路徑 |
|------|------|----------|
| 預設 RESX（英文） | 預設語系，建議英文 | `src/Resources/Resources.resx` |
| 日文 | ja | `src/Resources/Resources.ja.resx` |
| 繁體中文 | zh-TW | `src/Resources/Resources.zh-TW.resx` |
| 葡萄牙文 | pt | `src/Resources/Resources.pt.resx` |
| 泰文 | th | `src/Resources/Resources.th.resx` |
| ILocalizationService | 介面：`SetCulture`、`GetString`、`CultureChanged` | `src/Services/ILocalizationService.cs` |
| LocalizationService | 實作，使用 `ResourceManager` | `src/Services/LocalizationService.cs` |
| Resources 存取類別 | 強型別 `GetString`、`AppName` | `src/Resources/Resources.Designer.cs` |

### RESX 資源基底名稱

- **ResourceManager 基底名稱**：`CursorTestApp.Resources.Resources`
- 文化代碼與檔名對應：`Resources.{culture}.resx`（如 `Resources.ja.resx`）

### 使用範例

```csharp
// 透過 ILocalizationService 切換語系
ILocalizationService localizationService = new LocalizationService();
localizationService.SetCulture(new CultureInfo("zh-TW"));
string text = localizationService.GetString("AppName");

// 或使用強型別 Resources 類別
string appName = CursorTestApp.Resources.Resources.AppName;
string custom = CursorTestApp.Resources.Resources.GetString("KeyName", culture);
```

### 新增語系字串

1. 在各 `Resources.{culture}.resx` 新增對應 `<data name="KeyName">` 區塊
2. 於 `Resources.Designer.cs` 新增屬性（可選）或直接使用 `GetString("KeyName")`

---

## Phase 2：資料庫

### 實作內容

| 項目 | 說明 | 檔案路徑 |
|------|------|----------|
| User 模型 | Id, Username, Password（明文） | `src/Models/User.cs` |
| IDatabaseService | `Initialize()`、`DatabasePath` | `src/Services/IDatabaseService.cs` |
| DatabaseService | SQLite 建表、種子資料 | `src/Services/DatabaseService.cs` |
| IAuthService | `ValidatePassword`、`GetUserByUsername` | `src/Services/IAuthService.cs` |
| AuthService | 密碼直接字串比對 | `src/Services/AuthService.cs` |
| 建表腳本 | 參考用 SQL | `src/Scripts/CreateUsersTable.sql` |

### Users 資料表結構

```sql
CREATE TABLE IF NOT EXISTS Users (
    Id TEXT PRIMARY KEY,
    Username TEXT NOT NULL UNIQUE,
    Password TEXT NOT NULL
);
```

### 測試帳號

| Id | Username | Password |
|----|----------|----------|
| 0001 | aimarliu | aimarliu |

### 初始化流程

1. `App.OnStartup` 建立 `DatabaseService` 並呼叫 `Initialize()`
2. 若 `app.db` 不存在，建立於應用程式目錄
3. 執行建表 SQL
4. 若 Users 表為空，自動插入測試帳號

### 使用範例

```csharp
// 驗證密碼
IDatabaseService dbService = new DatabaseService();
IAuthService authService = new AuthService(dbService);

User? user = authService.ValidatePassword("aimarliu");
if (user != null)
{
    // 登入成功，user.Username == "aimarliu"
}

// 依使用者名稱查詢
User? u = authService.GetUserByUsername("aimarliu");
```

### 資料庫路徑

- **預設**：`{AppDomain.BaseDirectory}app.db`
- **自訂**：`new DatabaseService("C:\custom\path\app.db")`

### 依賴套件

- `Microsoft.Data.Sqlite`（已於 Phase 0 安裝）

---

## Phase 3：登入畫面

### 實作內容

| 項目 | 說明 | 檔案路徑 |
|------|------|----------|
| ShellWindow | FluentWindow 主殼，承載 ContentControl 與 LogPanel | `src/ShellWindow.xaml` |
| LoginView | 登入畫面，中央卡片 + PasswordBox + Enter/Cancel | `src/Views/LoginView.xaml` |
| MainView | 主畫面（登入成功後，目前為空白歡迎頁） | `src/Views/MainView.xaml` |
| LogPanel | 右下角 Log 區域，綁定 LogMessages | `src/Views/LogPanel.xaml` |
| ILogService | `LogMessages`、`Append` | `src/Services/ILogService.cs` |
| LogService | 格式 `[HH:mm:ss] 訊息內容` | `src/Services/LogService.cs` |
| INavigationService | `NavigateToMain`、`NavigateToLogin` | `src/Services/INavigationService.cs` |
| NavigationService | 以 ContentControl 切換 View | `src/Services/NavigationService.cs` |
| LoginViewModel | Password、LoginCommand、CancelCommand、ExitCommand、語系切換 | `src/ViewModels/LoginViewModel.cs` |
| LanguageOption | 語系選項模型（CultureName、DisplayName） | `src/Models/LanguageOption.cs` |
| SupportedCultures | 支援語系清單、`ResolveFromSystem()` 對應系統語系 | `src/Helpers/SupportedCultures.cs` |
| LocalizedString | 可綁定語系字串包裝類，訂閱 CultureChanged 自動發送 PropertyChanged | `src/Helpers/LocalizedString.cs` |

### 動態語系切換（方法二：LocalizedString）

控制項文字須以 Data Binding 綁定 `LocalizedString.Value`，不可寫死在 XAML。`LocalizedString` 訂閱 `ILocalizationService.CultureChanged`，語系變更時自動發送 `PropertyChanged("Value")`，UI 即時更新。

| 資源鍵 | 說明 | 範例翻譯 |
|--------|------|----------|
| LoginTitle | 登入標題 | en: Login, ja: ログイン, zh-TW: 登入 |
| PasswordPlaceholder | 密碼佔位 | en: Enter password, zh-TW: 請輸入密碼 |
| EnterButton | 登入按鈕 | en: Enter, pt: Entrar |
| CancelButton | 取消按鈕 | en: Cancel, th: ยกเลิก |
| ExitToolTip | 離開按鈕提示 | en: Exit, ja: 終了 |

### 3.2.1 語系切換（Login 方框下方）

| 項目 | 說明 |
|------|------|
| 位置 | Login 卡片下方，ComboBox 綁定 `SupportedLanguages`、`SelectedLanguage` |
| 預設語系 | `SupportedCultures.ResolveFromSystem()` 對應系統語系 |
| 動態切換 | `OnSelectedLanguageChanged` 呼叫 `ILocalizationService.SetCulture` |
| **即時更新** | 使用 `LocalizedString` 包裝類訂閱 `CultureChanged`，綁定 `{Binding X.Value}` |

### 3.2.2 右上角 X 離開按鈕

| 項目 | 說明 |
|------|------|
| 位置 | 登入畫面右上角，`HorizontalAlignment="Right"` `VerticalAlignment="Top"` |
| 指令 | `ExitCommand` → `Application.Current.Shutdown()` |

### LogPanel 捲動與自動捲到底

| 項目 | 說明 |
|------|------|
| 捲動 | ListBox 設 `IsHitTestVisible="True"`，允許滾輪、捲軸操作 |
| 自動捲到底 | 訂閱 `LogMessages.CollectionChanged`，新增項目時呼叫 `ScrollIntoView(lastItem)` |
| 實作位置 | `Views/Shared/LogPanel.xaml`、`LogPanel.xaml.cs` |

### 程式碼要點

- **背景**：深色漸層 `#1a1a2e` → `#16213e` → `#0f3460`，裝飾圓形 Blur
- **中央卡片**：圓角 12、半透明白背景 `#40FFFFFF`
- **WPF-UI PasswordBox**：繼承 TextBox，直接綁定 `Text="{Binding Password}"`
- **導航**：`ContentHost` + `NavigationService` 切換 LoginView / MainView
- **LocalizationService**：建構時可傳入初始語系，預設使用 `ResolveFromSystem()`

### 使用範例

```csharp
// ShellWindow 建立服務與導航
_logService = new LogService();
_navigationService = new NavigationService(ContentHost, mainFactory, loginFactory);
LogPanel.DataContext = _logService;

// LoginViewModel 登入流程
var user = _authService.ValidatePassword(Password);
if (user != null)
{
    _logService.Append("登入成功");
    _navigationService.NavigateToMain();
}
```

### 測試帳號

密碼 `aimarliu` 可通過驗證（見 Phase 2）。

### 5.6 應用程式圖示

| 項目 | 說明 | 檔案 |
|------|------|------|
| 來源圖檔 | `icons8-app-48.png` | `src/Resources/icons8-app-48.png` |
| 視窗 Icon | ShellWindow 使用 pack URI 綁定 PNG | `ShellWindow.xaml` Icon 屬性 |
| exe 圖示 | 建置前 ConvertPngToIco Target 將 PNG 轉為 ICO | `Resources/icons8-app-48.ico` |
| csproj | `ApplicationIcon` 指向 `.ico` | `CursorTestApp.csproj` |

### 後續整合

- **Phase 4**：已完成（見下）
- **Phase 5.5**：LoginView / MainView 字串已改為 RESX 綁定

---

## Phase 4：主畫面（依 howTostructrePages.md 作法一）【已完成】

### 目錄結構
依作法一，Views 底下依頁面分子資料夾：
```
Views/
├── Login/
│   ├── LoginView.xaml(.cs)
│   └── ...
├── Main/
│   ├── MainView.xaml(.cs)
│   └── ...
└── Shared/
    └── LogPanel.xaml(.cs)    ← 共用元件
```

### 實作內容
| 項目 | 說明 | 檔案路徑 |
|------|------|----------|
| MainView | 主畫面，中央 MainPageTitle、左下角 ⬅ 返回 | `Views/Main/MainView.xaml` |
| MainViewModel | MainPageTitle（LocalizedString）、NavigateBackCommand | `ViewModels/MainViewModel.cs` |
| LogPanel | 移至 Shared/ 共用 | `Views/Shared/LogPanel.xaml` |
| INavigationService | 新增 LogPanelVisibilityChanged 事件 | `Services/INavigationService.cs` |

### howTostructrePages.md 實作檢查
- [x] NavigationService 中登入／主頁型別：`LoginView`、`MainView` 正確
- [x] csproj 對 Views 檔案的引用：SDK 自動包含
- [x] 專案內 namespace 引用：`CursorTestApp.Views.Login`、`CursorTestApp.Views.Main`、`CursorTestApp.Views.Shared`
- [x] LogPanel 共用性：置於 Shared/，主畫面不需 Log 時隱藏

### Phase 5.5 多國語系整合（主畫面補強）
- [x] MainView 返回按鈕 ToolTip 改為 RESX 綁定（`BackToLoginToolTip`）
- [x] App.xaml 預設 FontFamily：`Segoe UI, Meiryo UI, Microsoft JhengHei UI, Leelawadee UI`
- [x] 支援五種語系：en, ja, zh-TW, pt, th

---

## 專案結構總覽

```
src/
├── ShellWindow.xaml(.cs)
├── Models/
│   ├── User.cs
│   └── LanguageOption.cs
├── Resources/
│   ├── Resources.resx
│   ├── Resources.ja.resx ~ th.resx
│   └── Resources.Designer.cs
├── Services/
│   ├── IDatabaseService.cs, DatabaseService.cs
│   ├── IAuthService.cs, AuthService.cs
│   ├── ILogService.cs, LogService.cs
│   ├── INavigationService.cs, NavigationService.cs
│   ├── ILocalizationService.cs, LocalizationService.cs
│   └── ...
├── ViewModels/
│   ├── ViewModelBase.cs
│   ├── LoginViewModel.cs
│   └── MainViewModel.cs
├── Views/
│   ├── Login/
│   │   └── LoginView.xaml(.cs)
│   ├── Main/
│   │   └── MainView.xaml(.cs)
│   └── Shared/
│       └── LogPanel.xaml(.cs)
├── Helpers/
│   ├── SupportedCultures.cs
│   └── LocalizedString.cs
└── Scripts/
    └── CreateUsersTable.sql
```

---

## 後續 Phase 整合提示

- **Phase 4 主畫面**：已完成，MainView 共用 `ILocalizationService`，返回按鈕 ToolTip 已綁定 RESX
- **Phase 5.3 Log**：已整合，`ILogService` 於 ShellWindow 建立並注入 LoginViewModel
- **Phase 5.5 多國語系**：已完成，LoginView / MainView 字串已改為 RESX 綁定，App.xaml 已設定預設 FontFamily

---

*建立日期：2025-02-22*
