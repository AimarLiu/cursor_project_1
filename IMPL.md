# 實作摘要 (Implementation Summary)

本文件記錄 Phase 1.2、Phase 1.3、Phase 2、Phase 3、Phase 4、Phase 4 (Layout2) 與 Phase 5.1–5.6 的完成實作，供後續開發參考。

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

1. `App.OnStartup` 建立 `DatabaseService` 並呼叫 `Initialize()`（含 try/catch，失敗時 MessageBox 後 Shutdown）
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
| LoginView | 登入畫面，中央卡片 + PasswordBox + Enter/Cancel | `src/Views/Login/LoginView.xaml` |
| MainView | 主畫面（登入成功後） | `src/Views/Main/MainView.xaml` |
| LogPanel | 右下角 Log 區域，綁定 LogMessages | `src/Views/Shared/LogPanel.xaml` |
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
| LoginErrorWrongPassword | 登入失敗（密碼錯誤） | en: Wrong password., zh-TW: 密碼錯誤。 |
| LoginErrorEmptyPassword | 登入失敗（未輸入密碼） | en: Please enter password., zh-TW: 請輸入密碼。 |

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

| 項目 | 說明 | 檔案路徑 |
|------|------|----------|
| 來源圖檔 | `icons8-app-96.png` 作為應用程式圖示來源 | `src/Resources/Icons/icons8-app-96.png` |
| 視窗 Icon | ShellWindow 使用 pack URI 綁定 PNG | `ShellWindow.xaml` 屬性 `Icon="pack://application:,,,/CursorTestApp;component/Resources/Icons/icons8-app-96.png"` |
| exe 圖示 | 建置/發佈前 ConvertPngToIco Target 將 PNG 轉為 ICO，輸出至 `Resources/` 以利嵌入 | `Resources/icons8-app-96.ico` |
| csproj | `ApplicationIcon` 指向 `Resources\icons8-app-96.ico`；Target 讀取 `Resources\Icons\icons8-app-96.png` 並寫入上述 ico | `CursorTestApp.csproj` |

**補充說明**：建置前需將 `icons8-app-96.png` 置於 `src/Resources/Icons/`。MSBuild 的 `ConvertPngToIco` 在 `BeforeBuild` 與 `BeforePublish` 執行，產生 `src/Resources/icons8-app-96.ico`，供 exe 圖示嵌入。發佈後若 exe 圖示未更新，可先執行 `dotnet clean` 再重新建置/發佈。

### 後續整合

- **Phase 4**：已完成（見下）
- **Phase 5**：5.1–5.6 已完成（見下）

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

## Phase 4 (Layout2)：主畫面2與導航【已完成】

### 概述
登入成功後導航至 Layout2（生產資訊看板／排程／完成訂單）。與 Phase 4 主畫面（Layout1）並存，ShellWindow 可切換 Login / Main / Layout2。

### 實作內容
| 項目 | 說明 | 檔案路徑 |
|------|------|----------|
| Layout2View | 左窄右寬、頂部狀態列、左側看板、右側兩 DataGrid + 調單、底部控制列 | `Views/Layout2/Layout2View.xaml(.cs)` |
| ProductionInfoPanel | 左側生產資訊看板（Grid 平均高度、字體 15、綁定排程第一筆與車速/產量/預估時間） | `Views/Layout2/ProductionInfoPanel.xaml(.cs)` |
| Layout2ViewModel | 排程/完成訂單、選取項、F2/F6/調單/F1 模擬生產、RS485 狀態、RESX LocalizedString | `ViewModels/Layout2ViewModel.cs` |
| ScheduleOrderItem | 生產排程一筆 | `Models/ScheduleOrderItem.cs` |
| CompletedOrderItem | 生產完成訂單一筆（含 CompletedAt） | `Models/CompletedOrderItem.cs` |
| IProductionScheduleService | 排程/完成訂單集合與操作 | `Services/IProductionScheduleService.cs` |
| ProductionScheduleService | 自 `IScheduleOrderRepository` 載入；空表時種子範例 | `Services/ProductionScheduleService.cs` |
| IScheduleOrderRepository / ScheduleOrderRepository | 自 SQLite 載入 ScheduleOrders、CompletedOrders | `Services/IScheduleOrderRepository.cs`、`ScheduleOrderRepository.cs` |
| IRs485Service / Rs485Service | OPT/PLC LED、車速、HasError（UI 綁定） | `Services/IRs485Service.cs`、`Rs485Service.cs` |
| Rs485BackgroundService | 背景讀取 COM1（9600,8,N,1），解析 ACTIVE+/PLC+/RPM/ERROR+ 並更新 Rs485Service | `Services/Rs485BackgroundService.cs` |
| F4SearchDialog | F4 預覽搜尋（確定/取消） | `Views/Layout2/F4SearchDialog.xaml(.cs)` |
| INavigationService | 新增 NavigateToLayout2 | `Services/INavigationService.cs` |
| BoolToLedBrushConverter | OPT 紅/灰、PLC 綠/灰 LED | `Converters/BoolToLedBrushConverter.cs` |
| Resources/Icons | 圖示集中目錄（pack URI component/Resources/Icons/xxx） | `Resources/Icons/*.png`、`alarm.png`、`red-alam.gif` |
| WpfAnimatedGif | 異常狀態 GIF 動畫（NuGet 2.0.2） | `CursorTestApp.csproj`；`Layout2View.xaml(.cs)` |
| CreateScheduleTables.sql | ScheduleOrders、CompletedOrders 建表與種子參考 | `Scripts/CreateScheduleTables.sql` |

### 導航與服務
- 登入成功：`LoginViewModel` 呼叫 `NavigateToLayout2()`。
- ShellWindow：`DatabaseService.Initialize()` 後建立 `ScheduleOrderRepository`、`ProductionScheduleService(repository)`、`Rs485Service`、`Rs485BackgroundService`；Loaded 時啟動 RS485 背景讀取，Closed 時停止。
- Layout2 底部「返回」呼叫 `NavigateToLogin()`。

### 資料庫與排程載入
- `DatabaseService.Initialize()` 內呼叫 `EnsureScheduleTables`、`SeedScheduleDataIfEmpty`（空表時寫入範例排程／完成訂單）。
- `ProductionScheduleService(IScheduleOrderRepository?)`：傳入 repository 時自 DB 載入排程與完成訂單；若兩者皆空則寫入種子資料。

### Layout2 RESX 語系
- `Resources.*.resx` 新增 Layout2_* 鍵（標題、按鈕 ToolTip、DataGrid 欄位名、左側看板標籤、停車中/約 N 秒、Dialog 標題等），五語系 en/ja/zh-TW/pt/th。
- `Layout2ViewModel` 注入 `ILocalizationService`，以 `LocalizedString` 暴露上述字串，View / ProductionInfoPanel 綁定 `{Binding XXX.Value}`；語系切換時預估時間字串依 Culture 更新。

### DataGrid 與左側看板
- 兩 DataGrid：`CanUserSortColumns="False"`、`VirtualizingStackPanel.IsVirtualizing="True"`。
- 左側生產資訊看板：外層 Grid、6 列 `Height="*"` 平均分配，欄位字體 15、VerticalAlignment.Center。

### Layout2 按鈕版面（Grid、圖左文居中）
- 頂部 F2/F4/F6、中區訂單上移/下移、底部返回/F7/F1/F11/關機：皆以 `Grid` 為按鈕內容，兩欄（`Auto` + `*`），圖片/圖示靠左、文字綁定 RESX 並置中；`MinWidth`/`Height` 統一適當尺寸（頂部 36、中區 40、底部 48）。

### 模擬生產（F1 排程管理）
- F1 啟動：`IsSimulating = true`，OPT/PLC 亮，車速每秒亂數 400–500，每 30 秒目前產量 +1 並將排程第一筆移入完成訂單；排程清空時自動停止。
- 再按 F1 可手動停止。

### RS485
- `Rs485BackgroundService`：背景執行緒嘗試開啟 COM1（9600,8,N,1），讀取文字行並解析 `ACTIVE+`/`ACTIVE-`、`PLC+`/`PLC-`、`RPM:123`、`ERROR+`/`ERROR-`，以 `Dispatcher.InvokeAsync` 更新 `Rs485Service`，UI 即時反映。若 COM 無法開啟則靜默結束，F1 模擬仍可直接更新 `Rs485Service`。

### Layout2 圖示與異常動畫
| 項目 | 說明 | 檔案路徑 |
|------|------|----------|
| 左側頂部圖示 | `icons8-favorites-shield-5-stars-96.png`，顯示尺寸 48×48 | `Layout2View.xaml` |
| 異常狀態圖示 | 平時 `alarm.png`；RS485 錯誤時改為 `red-alam.gif` 動畫 | `Layout2View.xaml`、`Layout2View.xaml.cs` |
| GIF 動畫 | 使用 **WpfAnimatedGif**，`ImageBehavior.SetAnimatedSource` 播放 GIF，`RepeatBehavior.Forever` | `Layout2View.xaml`（xmlns:gif）、`Layout2View.xaml.cs`（using WpfAnimatedGif） |
| 切換邏輯 | `UpdateAlarmImage(bool hasError)`：有錯誤時設 AnimatedSource 為 GIF；無錯誤時先 `SetAnimatedSource(null)`、再 `Source = null`、再 `Source = PNG` 以確保靜態圖顯示 | `Layout2View.xaml.cs` |

**補充說明**：所有圖示皆來自 `src/Resources/Icons/`；WPF 內建 `Image`+`BitmapImage` 僅顯示 GIF 第一幀，故異常動畫需依賴 WpfAnimatedGif 套件。

---

## Phase 5：整合與收尾（5.1–5.6 已完成）

### 5.1 綁定與流程
| 項目 | 說明 |
|------|------|
| 啟動畫面 | ShellWindow 啟動後 `NavigateToLogin()` 顯示 LoginView |
| DataContext | ShellWindow.CreateLoginView 將 LoginViewModel 設為 LoginView.DataContext |
| 流程 | 輸入密碼 → Enter → 比對資料庫 → 成功則 NavigateToLayout2() |

### 5.2 錯誤處理
| 項目 | 說明 | 檔案 |
|------|------|------|
| 資料庫連線失敗 | App.OnStartup 內 try/catch `Initialize()`，失敗時 MessageBox 後 `Shutdown(1)` | `App.xaml.cs` |
| 密碼錯誤提示 | LoginViewModel.LoginErrorMessage（RESX `LoginErrorWrongPassword`），LoginView 紅色 TextBlock 顯示 | `LoginViewModel.cs`、`LoginView.xaml` |
| 輸入為空提示 | 登入前檢查 `string.IsNullOrWhiteSpace(Password)`，顯示 RESX `LoginErrorEmptyPassword` | `LoginViewModel.cs` |
| Converter | 字串非空 → Visible，空 → Collapsed | `Converters/StringNotEmptyToVisibilityConverter.cs`、`App.xaml` |

### 5.3 Log 服務整合
| 項目 | 說明 |
|------|------|
| 初始化時機 | ShellWindow 建構時建立 `LogService`，並注入 LoginViewModel / MainViewModel |
| 共用實例 | 同一 `_logService` 傳入各 ViewModel |
| 格式 | `[HH:mm:ss] 訊息內容`（LogService 已實作） |

### 5.4 程式品質
- 專案內無 `Console.WriteLine`
- 密碼以明文儲存於 SQLite（測試用）
- 註解與結構已整理

### 5.5 多國語系與字型
| 項目 | 說明 |
|------|------|
| Login / Main | 字串已使用 RESX + LocalizedString |
| 登入錯誤訊息 | LoginErrorWrongPassword、LoginErrorEmptyPassword 已加入五語系 RESX |
| 預設字型 | App.xaml 設定 DefaultFontFamily + TextElement / Control Style |
| 驗證 | 五種語系（ja, zh-TW, pt, en, th）需手動驗證顯示 |

---

## 專案結構總覽

```
src/
├── ShellWindow.xaml(.cs)
├── Models/
│   ├── User.cs
│   ├── LanguageOption.cs
│   ├── ScheduleOrderItem.cs
│   └── CompletedOrderItem.cs
├── Resources/
│   ├── Icons/          (*.png, alarm.png, red-alam.gif)
│   ├── Resources.resx
│   ├── Resources.ja.resx ~ th.resx
│   └── Resources.Designer.cs
├── Services/
│   ├── IDatabaseService.cs, DatabaseService.cs
│   ├── IAuthService.cs, AuthService.cs
│   ├── ILogService.cs, LogService.cs
│   ├── INavigationService.cs, NavigationService.cs
│   ├── ILocalizationService.cs, LocalizationService.cs
│   ├── IProductionScheduleService.cs, ProductionScheduleService.cs
│   ├── IScheduleOrderRepository.cs, ScheduleOrderRepository.cs
│   ├── IRs485Service.cs, Rs485Service.cs, Rs485BackgroundService.cs
│   └── ...
├── ViewModels/
│   ├── ViewModelBase.cs
│   ├── LoginViewModel.cs
│   ├── MainViewModel.cs
│   └── Layout2ViewModel.cs
├── Views/
│   ├── Login/
│   │   └── LoginView.xaml(.cs)
│   ├── Main/
│   │   └── MainView.xaml(.cs)
│   ├── Layout2/
│   │   ├── Layout2View.xaml(.cs)
│   │   ├── ProductionInfoPanel.xaml(.cs)
│   │   └── F4SearchDialog.xaml(.cs)
│   └── Shared/
│       └── LogPanel.xaml(.cs)
├── Helpers/
│   ├── SupportedCultures.cs
│   └── LocalizedString.cs
├── Converters/
│   ├── StringNotEmptyToVisibilityConverter.cs
│   └── BoolToLedBrushConverter.cs
└── Scripts/
    ├── CreateUsersTable.sql
    └── CreateScheduleTables.sql
```

---

## 後續 Phase 整合提示

- **Phase 4 主畫面**：已完成，MainView 共用 `ILocalizationService`，返回按鈕 ToolTip 已綁定 RESX
- **Phase 4 (Layout2)**：已完成，登入後導航至 Layout2，含排程/完成訂單、模擬生產、F4/F7/F11 Dialog
- **Phase 5.1–5.6**：已完成；含綁定與流程、錯誤處理、Log 整合、程式品質、多國語系與字型、應用程式圖示（icons8-app-96）
- **Layout2 圖示**：左側頂部為 icons8-favorites-shield-5-stars-96（96×96）；異常狀態使用 WpfAnimatedGif 播放 red-alam.gif
- **Phase 5.7 以後**：發佈、Win 8.1 測試、SqliteViewer 小工具等見 TODO.md

---

*建立日期：2025-02-22*
*更新：Phase 5.1–5.6 實作摘要；Phase 4 (Layout2) 圖示與異常 GIF 動畫（WpfAnimatedGif）、左側頂部圖示*
