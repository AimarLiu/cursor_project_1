# 實作摘要 (Implementation Summary)

本文件記錄 Phase 1.2、Phase 1.3、Phase 2、Phase 3、Phase 4、Phase 4 (Layout2)、Phase 5.1–5.6 與 **Phase 7（Settings；Tab1 刀位安全已落地）** 的完成實作，供後續開發參考。

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

- **預設**：`%LocalAppData%\CursorTestApp\app.db`（`Environment.SpecialFolder.LocalApplicationData`），避免單檔發佈在另一台電腦執行時 BaseDirectory 指向解壓暫存目錄（可能唯讀）導致 crash。說明見 `QandA/crashAfterLoginOnOtherPC.md`。
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

### 6.6 應用程式圖示

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
- 兩 DataGrid：`CanUserSortColumns="False"`、`VirtualizingStackPanel.IsVirtualizing="True"`、`VirtualizationMode="Recycling"`；`SelectionMode="Single"`、`SelectionUnit="FullRow"`；**`FontSize="18"`**。
- **標題欄**：標題文字於 **Layout2View.OnLoaded** 由 code-behind 自 ViewModel 的 `LocalizedString`（RESX Layout2_OrderNo、Layout2_VersionNo 等）填入，避免 XAML 綁定在 Column 上無效。生產排程標題：生產訂單號、版號、受訂量、箱型、類別、客戶名稱、備註；生產完成訂單多「日期時間」。
- **標題列樣式**：`Layout2DataGridColumnHeaderStyle` — Background `#333333`（深灰）、Foreground White、FontSize 18、Padding 6,6、MinHeight 28。
- **欄寬**：約 1.5 倍 — 生產排程 135/135/105/75/75/*/*，生產完成訂單 210/135/135/105/75/75/*/*。
- **選取視覺**：`RowStyle` + `CellStyle`（Trigger IsSelected 設 #1976D2/White）；`DataGrid.Resources` 覆寫 `SystemColors.HighlightBrushKey`/`HighlightTextBrushKey`；CellStyle 設 `BorderThickness="0"`、`FocusVisualStyle="{x:Null}"`，僅整列高亮、無格子黑框。
- **頂部狀態列**：左欄 Width 400、MinWidth 360，右欄 MinWidth 420，避免長語系文字遮蓋右側圖示與 F2/F4/F6 按鈕。
- **左側生產資訊看板（ProductionInfoPanel）**：
  - 外層 Border 背景 `#3a4555`（較明亮藍灰）；欄位列上三列 `#455570`、下三列 `#3d4a62` 兩色區分。
  - 字體依語系：`Layout2ViewModel.LeftPanelFontSize` 綁定，英文 16、其餘 20；CultureChanged 時 OnPropertyChanged。
  - 「預估完成所需時間」與估算值（停車中／預估時間）拆成兩行顯示（StackPanel 垂直、最後一列 Height Auto），數值設 TextWrapping 避免長文字被遮蔽。

### Layout2 按鈕版面（Grid、圖左文居中）
- 頂部 F2/F4/F6、中區訂單上移/下移、底部返回/F7/F1/F11/關機：皆以 `Grid` 為按鈕內容，兩欄（`Auto` + `*`），圖片/圖示靠左、文字綁定 RESX 並置中；`MinWidth`/`Height` 統一適當尺寸（頂部 36、中區 40、底部 48）。

### 模擬生產（F1 排程管理、Phase 5 Dialog）
- **F1 排程管理**（Layout2 底部）：點擊開啟 **Phase 5 排程管理子頁**（ScheduleManageDialog），不直接啟動模擬。若已在模擬中則按鈕顯示「F1 生產中」+ 暫停圖示，再按可手動停止。
- **排程管理 Dialog**：標題「排單管理」、頂部目前版號（排程第一筆）、DataGrid 綁定同一 `ScheduleOrders`、底部 F4 分印（暫無功能）/ F2 前置排單 / F3 把全排量 / F1 離開。F2 或 F3 關閉 Dialog 並帶回 `SimulationMode.SmallBatch` 或 `FullBatch`，F1 離開帶回 `None`。
- **啟動模擬**：Layout2 依 Dialog 關閉時帶回之模式呼叫 `StartSimulationWithMode(mode)`。小量（F2）：僅第一筆訂單生產 5 個後自動停止；全量（F3）：每 30 秒產量 +1、完成筆移入完成訂單，排程清空或手動停止。車速 400–500、OPT/PLC 亮、預估時間更新同前。

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

## Phase 5：排程管理子頁（Dialog）

### 實作內容
| 項目 | 說明 | 檔案路徑 |
|------|------|----------|
| SimulationMode | 列舉 None / SmallBatch（F2 小量 5 個）/ FullBatch（F3 全量） | `Models/SimulationMode.cs` |
| ScheduleManageDialog | 排單管理 Window，標題/版號/**DataGrid**（與 Layout2 上區相同控制項與樣式，避免 whyDataGridleg）/四按鈕；標題列黃底黑字（DialogDataGridColumnHeaderStyle）；**底部區塊黃底** #F9A825、按鈕文字**粗體**深色 #111；F4 bear-footprint、F2 frying-pan、F3 process、F1 exit；F2/F3 關閉時 Tag=Mode、DialogResult=true | `Views/Layout2/ScheduleManageDialog.xaml(.cs)` |
| ScheduleManageDialogViewModel | 綁定 ScheduleOrders、FirstScheduleItem.VersionNo、四按鈕 Command，RequestClose(Mode?) | `ViewModels/ScheduleManageDialogViewModel.cs` |
| Layout2ViewModel | F1 改為開啟 Dialog；StartSimulationWithMode(mode)；OnSimProductionTick 依 SmallBatch/FullBatch 停止條件 | `ViewModels/Layout2ViewModel.cs` |
| Layout2View F1 按鈕 | IsSimulating 時圖示改 pause、文字改 F1ProducingToolTip（F1 生產中） | `Views/Layout2/Layout2View.xaml` |
| RESX | Layout2_F1Producing、ScheduleManageDialog_Title / VersionLabel / F4Print / F2PreSchedule / F3FullSchedule / F1Exit，五語系；日文（ja）排程管理 Dialog 標題與四按鈕以平假名為主（はいだんかんり、ぶんいん、ぜんちはいだん、ぜんりょうはいだん、でる） | `Resources/Resources.*.resx` |

### 程式碼要點
- Dialog 建構時注入 `ScheduleManageDialogViewModel(IProductionScheduleService, ILocalizationService)`，同一 `ScheduleOrders` 與 Layout2 共用。ViewModel 的 `RequestClose` 事件傳遞 `SimulationMode?`；Window 訂閱後設 `Tag = mode`、`DialogResult = (mode != null && mode != None)`、`Close()`。
- Layout2ViewModel.F1StartSchedule：若已模擬則 StopSimulation；否則 `new ScheduleManageDialog(vm) { Owner = MainWindow }.ShowDialog()`，若 `DialogResult == true && Tag is SimulationMode m` 則 `StartSimulationWithMode(m)`。
- 小量模式：`OnSimProductionTick` 內當 `CurrentQuantity >= 5` 時 `StopSimulation()` 並 log，不移入完成訂單。全量模式：維持原邏輯（達受訂量移入完成、排程清空或手動停止）。

---

## Phase 6：訂單製作（F7 訂單製作）

### 實作內容
| 項目 | 說明 | 檔案路徑 |
|------|------|----------|
| Orders 表 | 建立日期時間、生產訂單號、版號、受訂量、箱型、楞別、相位1～3、長度、寬度、客戶名稱、備註；DatabaseService 內 EnsureOrdersTable + SeedOrdersIfEmpty（隨機 100 筆） | `Services/DatabaseService.cs`、`Scripts/CreateOrdersTable.sql` |
| Order 模型 | 對應 Orders 表，INotifyPropertyChanged | `Models/Order.cs` |
| IOrderRepository / OrderRepository | FindByVersionNo、GetPage(10)、GetAll()、GetTotalCount、Insert、Update、Delete；4.4 不分頁時以 GetAll 供 DataGrid 捲軸顯示 | `Services/IOrderRepository.cs`、`Services/OrderRepository.cs` |
| IProductionScheduleService.AddOrderToSchedule | 將一筆 ScheduleOrderItem 加入排程（F2 加入排程用） | `Services/IProductionScheduleService.cs`、`ProductionScheduleService.cs` |
| OrderMakingView | **主內容區頁面**（UserControl，顯示於 Shell ContentHost）；頂部左:右 1:2、4.3:4.4 為 6:4；頂/底深灰、Phase6 整頁淺灰、左/右/4.5 白底；版號+F6、版號查詢+上下箭頭+預覽+送入版號位置；左側表單、右側 DataGrid **不分頁、捲軸**；版號/版號查詢找到訂單時定位選取並捲動至中央（RequestScrollToSelected），手動點選列不捲動；雙擊列帶出至左側與 4.5；4.5 相位 1色/2色/3色 標籤+輸入**上下排列**；底部 F4/F5/F2 靠左、**F1 離開最右**；右側 DataGrid 欄位標題 RESX、建檔日期 `yyyy/MM/dd HH:mm` | `Views/Layout2/OrderMakingView.xaml(.cs)` |
| OrderMakingDialogViewModel | 流程 3.0～3.4：初始反灰、版號搜尋有/無資料、版號查詢 DataGrid、預覽帶出、F5 儲存、F4 刪除、F2 加入排程並關閉；LocalizedString 多語系 | `ViewModels/OrderMakingDialogViewModel.cs` |
| Layout2ViewModel / ShellWindow | F7 改為導航至 OrderMakingView（主內容區）；ShellWindow.CreateOrderMakingView 建立 OrderMakingView + OrderMakingDialogViewModel；注入 IOrderRepository | `ViewModels/Layout2ViewModel.cs`、`ShellWindow.xaml.cs` |
| RESX | OrderMakingDialog_Title、VersionNo、F6Search、VersionNoQuery、Phase、Phase1Color/Phase2Color/Phase3Color、LengthLabel、WidthLabel、SubmitVersionPosition、F4Delete、F5CompleteEdit、F5AddOrder、F2AddToSchedule、F1Exit、ConfirmNewVersion、OrderSaved、ColCreatedAt；右側 DataGrid 欄位標題沿用 Layout2_VersionNo/CustomerName/BoxType/Category + OrderMakingDialog_ColCreatedAt；五語系 | `Resources/Resources.*.resx` |

### 程式碼要點
- **流程 3.0**：開啟時 IsLeftAndBottomEnabled、IsF4Enabled、IsF5Enabled、IsF2Enabled 皆 false；F5ButtonText 為「F5 完成編輯訂單」。
- **流程 3.1**：頂部左側輸入版號後 Enter 或 F6 搜尋 → FindByVersionNo；有資料則 LoadOrderToForm、解除反灰；無資料則 MessageBox 確認是否新增，確認後解除左/下/F5/F2、F5 改為「F5 新增訂單」。
- **儲存**：F5 依 CurrentOrderId 呼叫 Update 或 Insert，完成後顯示「此筆訂單:版號已儲存」並再次反灰。
- **F2 加入排程**：由目前表單組 ScheduleOrderItem，呼叫 _scheduleService.AddOrderToSchedule，_navigationService.NavigateToLayout2() 返回 Layout2。
- **4.4 不分頁**：RefreshOrdersPage 改為呼叫 GetAll() 填滿 OrdersPage，DataGrid 以捲軸顯示；上/下箭頭改為「同版號（版號查詢欄位）上一筆/下一筆」選取，不循環。
- **版號查詢（頂部右）**：Enter 時 QueryVersionNoInGrid，在 OrdersPage 找 VersionNo 並選取；找到後觸發 RequestScrollToSelected 讓 View 捲動至該列中央。
- **捲動時機**：僅在「版號搜尋」或「版號查詢」程式設定選取後觸發 RequestScrollToSelected；手動點選列不觸發，避免捲軸自動跑掉（見 LessonLearn/whyMoveWhenMouseClick.md）。

### 使用範例
```csharp
// ShellWindow 建立 IOrderRepository，NavigationService 註冊 CreateOrderMakingView
_orderRepository = new OrderRepository(databaseService);
_navigationService = new NavigationService(ContentHost, CreateMainView, CreateLoginView, CreateLayout2View, CreateOrderMakingView, CreateSettingsView);

// F7 由 Layout2 導航至訂單製作（主內容區頁面）
private UserControl CreateOrderMakingView()
{
    var vm = new OrderMakingDialogViewModel(_orderRepository, _scheduleService, _localizationService, _logService, _navigationService);
    return new OrderMakingView { DataContext = vm };
}
// Layout2ViewModel.F7OrderEdit() → _navigationService.NavigateToOrderMaking();
```

### 補充說明
- 種子 100 筆：CreatedAt 隨機 2024/01/01 13:00～2026/01/10 13:00，生產訂單號 4 碼、版號＝西元年+訂單號，箱型 E/S、楞別 A/AB/B/BC/C/E、客戶名隨機英文名。
- 送出版號位置按鈕與上下箭頭圖示（icons8-collapse/expand-arrow）可後續補上；預覽按鈕目前與 F6 共用圖示，可改為 icons8-package-search-96。

---

## Phase 6 整合與收尾（6.1–6.6 已完成）

### 6.1 綁定與流程
| 項目 | 說明 |
|------|------|
| 啟動畫面 | ShellWindow 啟動後 `NavigateToLogin()` 顯示 LoginView |
| DataContext | ShellWindow.CreateLoginView 將 LoginViewModel 設為 LoginView.DataContext |
| 流程 | 輸入密碼 → Enter → 比對資料庫 → 成功則 NavigateToLayout2() |

### 6.2 錯誤處理
| 項目 | 說明 | 檔案 |
|------|------|------|
| 資料庫連線失敗 | App.OnStartup 內 try/catch `Initialize()`，失敗時 MessageBox 後 `Shutdown(1)` | `App.xaml.cs` |
| 密碼錯誤提示 | LoginViewModel.LoginErrorMessage（RESX `LoginErrorWrongPassword`），LoginView 紅色 TextBlock 顯示 | `LoginViewModel.cs`、`LoginView.xaml` |
| 輸入為空提示 | 登入前檢查 `string.IsNullOrWhiteSpace(Password)`，顯示 RESX `LoginErrorEmptyPassword` | `LoginViewModel.cs` |
| 未處理例外 | `DispatcherUnhandledException` 註冊：完整例外（含 InnerException）以 `ExceptionFormatHelper.ToDisplayString` 寫入與執行檔同目錄的 `error_yyyy-MM-dd_HH-mm-ss.log`，MessageBox 僅提示「詳情已寫入：{檔名}」；寫檔失敗時 fallback 顯示簡短錯誤訊息 | `App.xaml.cs`、`Helpers/ExceptionFormatHelper.cs` |
| 登入後導航失敗 | `NavigateToLayout2()` 外層 try/catch，失敗時寫 Log、設 LoginErrorMessage、MessageBox 顯示 | `LoginViewModel.cs` |
| LogListBox 異機不一致 | 「ItemsControl 與其項目來源不一致」：LogService.Append 一律 `InvokeAsync(Loaded)`、LogListBox 設 `VirtualizingPanel.IsVirtualizing="False"`、LogPanel 內 ScrollIntoView 延後至 `DispatcherPriority.Loaded`；異機已驗證通過，詳見 `LessonLearn/whyItemSourceNotConsistant.md` | `LogService.cs`、`LogPanel.xaml`、`LogPanel.xaml.cs` |
| Converter | 字串非空 → Visible，空 → Collapsed | `Converters/StringNotEmptyToVisibilityConverter.cs`、`App.xaml` |

### 6.3 Log 服務整合
| 項目 | 說明 |
|------|------|
| 初始化時機 | ShellWindow 建構時建立 `LogService`，並注入 LoginViewModel / MainViewModel |
| 共用實例 | 同一 `_logService` 傳入各 ViewModel |
| 格式 | `[HH:mm:ss] 訊息內容`（LogService 已實作） |

### 6.4 程式品質
- 專案內無 `Console.WriteLine`
- 密碼以明文儲存於 SQLite（測試用）
- 註解與結構已整理

### 6.5 多國語系與字型
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
│   ├── CompletedOrderItem.cs
│   ├── SimulationMode.cs
│   └── Order.cs
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
│   ├── IOrderRepository.cs, OrderRepository.cs
│   ├── IRs485Service.cs, Rs485Service.cs, Rs485BackgroundService.cs
│   └── ...
├── ViewModels/
│   ├── ViewModelBase.cs
│   ├── LoginViewModel.cs
│   ├── MainViewModel.cs
│   ├── Layout2ViewModel.cs
│   ├── ScheduleManageDialogViewModel.cs
│   └── OrderMakingDialogViewModel.cs
├── Views/
│   ├── Login/
│   │   └── LoginView.xaml(.cs)
│   ├── Main/
│   │   └── MainView.xaml(.cs)
│   ├── Layout2/
│   │   ├── Layout2View.xaml(.cs)
│   │   ├── ScheduleManageDialog.xaml(.cs)
│   │   ├── OrderMakingView.xaml(.cs)
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
    ├── CreateScheduleTables.sql
    └── CreateOrdersTable.sql
```

---

## Phase 7：Settings（PLC／參數）頁 — **Tab1 刀位安全** + **Tab2 紙箱／DieCutter** + **Tab3 其他參數**

> **規格**：`TODO_Phase7.md`、`TODO_Phase7_Layout.md`。  
> **已完成**：`SettingsView`（6 分頁 + 底部 F 鍵）、Admin 分流、**Tab1** 16 鍵 **`KnifeSlot_*`**、**Tab2** 箱型刀選擇／`DieCutter.*`／車速、**Tab3** 通訊三列／部門 DataGrid／`Users` 密碼；**F2** 依序儲存 Tab1→Tab2→Tab3、**F3** 重載三者。  
> **待辦**：§7.6 **F2** 全頁 **單一 transaction 合併 Tab1～Tab3**（目前實作為分開交易），以及 **Scripts/*.sql** 獨立建表／種子腳本檔（表仍先由程式內 `DatabaseService` 內建表／種子，先行遷移保留）。

### 實作內容

| 項目 | 說明 | 檔案路徑 |
|------|------|----------|
| 導航 | **`NavigateToSettings()`**；**Admin** 登入 → Settings，其餘 → Layout2 | `INavigationService.cs`、`NavigationService.cs`、`LoginViewModel.cs` |
| Admin 種子 | **`EnsureAdminUserIfMissing`**：`0002`／`Admin`／`Admin` | `DatabaseService.cs` |
| Settings 主頁 | 頂部 **6 分頁** + 內容區 + 底部 **F1～F6** | `SettingsView.xaml`、`SettingsViewModel.cs` |
| Tab1 UI | 刀模 **PNG**、`ScrollViewer` **內容置中**；**E／J／K／F**、**A**、**H／I／B**；MAX／MIN **樣式**（藍／紅橘） | `Views/Settings/Tabs/SettingsTab1KnifeSafetyView.xaml` |
| Tab1 VM | **`SettingsTab1KnifeViewModel`**：16 欄位、整數／空白、`Min≤Max` 驗證 | `ViewModels/SettingsTab1KnifeViewModel.cs` |
| Tab1 資料 | **`PlcParameterValues`**；ProfileId=**1**；**`IKnifeSafetyParameterRepository`** | `DatabaseService.cs`、`KnifeSafetyParameterRepository.cs` |
| Tab2 UI | 左 C／DA7、中 E、中右 `DieCutter.*` 1～9、右 `DieCutter.*` 10～12+車速；刀組合 Combo 依箱型篩選；Tab2 外層 **`Border` + `Tab2Box`** | `Views/Settings/Tabs/SettingsTab2BoxParametersView.xaml` |
| Tab2 VM | **`SettingsTab2BoxViewModel`**：驗證、`TrySaveToDatabase`／`ReloadFromDatabase` | `ViewModels/SettingsTab2BoxViewModel.cs` |
| Tab2 資料 | **`KnifeCombinedOptions`** 等參照表；**`BoxParameters.SelectedKnifeCombinedOption`**（Profile 2～4）、**`DieCutter.*`**（Profile **5** 共用） | `DatabaseService.cs`、`BoxDieCutterSettingsRepository.cs`、`IBoxDieCutterSettingsRepository.cs` |
| Tab3 UI | 左部門 **`ListBox`**、中 **`DataGrid`**（元件名／Max／Min／Accurate；**All** 顯示部門欄）、右 **通訊**（印／看埠 + 糊車 PLC 串列參數）+ **`Users`** 密碼表 | `Views/Settings/Tabs/SettingsTab3OtherPlcParametersView.xaml` |
| Tab3 VM | **`SettingsTab3ViewModel`**：**`Tab3PlcCatalog`** 列映射、**`TrySaveToDatabase`**／**`ReloadFromDatabase`** | `ViewModels/SettingsTab3ViewModel.cs`、`Models/Tab3/Tab3PlcCatalog.cs` |
| Tab3 資料 | **`PlcCommChannelSettings`**；**`ComponentDisplayNameOverrides`**；**`PlcParameterValues`**（ProfileId=**6** **`OtherPlcGlobal`**，`OtherPlcParameters` 鍵）；**`Users`** | `DatabaseService.cs`（`Tab3DatabaseBootstrap`）、`PlcCommChannelRepository`、`ComponentDisplayNameRepository`、`OtherPlcParametersRepository`、`UserDirectoryRepository` |
| 模型 | **`KnifeComboDisplayItem`**、**`CarSpeedOptionRow`**、**`BoxDieCutterSnapshot`**、**`PlcCommChannelDto`** 等 | `Models/*.cs`、`Models/Tab3/*` |
| Shell | **`SettingsViewModel`** 注入 **Tab1／Tab2／Tab3** 所需 **Repository** | `ShellWindow.xaml.cs` |
| RESX | **`SettingsTab1_*`**、**`SettingsTab2_*`**、**`SettingsTab3_*`**（部門、欄位、Feed／Print／Other 元件預設名）、**`SettingsTab123_SaveOk`／`ReloadOk`** | `Resources/Resources*.resx` |

### 目錄結構示意（Phase 7 相關）

```
src/
├── Services/
│   ├── INavigationService.cs
│   ├── NavigationService.cs
│   ├── IKnifeSafetyParameterRepository.cs
│   ├── KnifeSafetyParameterRepository.cs
│   ├── IBoxDieCutterSettingsRepository.cs
│   ├── BoxDieCutterSettingsRepository.cs
│   ├── IPlcCommChannelRepository.cs
│   ├── PlcCommChannelRepository.cs
│   ├── IOtherPlcParametersRepository.cs
│   ├── OtherPlcParametersRepository.cs
│   ├── IComponentDisplayNameRepository.cs
│   ├── ComponentDisplayNameRepository.cs
│   ├── IUserDirectoryRepository.cs
│   ├── UserDirectoryRepository.cs
│   └── Tab3DatabaseBootstrap.cs
├── Models/
│   ├── KnifeSafetyParameterKeys.cs
│   ├── BoxDieCutterDefinitionKeys.cs
│   ├── BoxDieCutterSnapshot.cs
│   ├── KnifeComboDisplayItem.cs
│   ├── CarSpeedOptionRow.cs
│   └── Tab3/
│       ├── Tab3PlcCatalog.cs
│       ├── OtherPlcParameterProfileId.cs
│       └── PlcCommChannelDto.cs
├── ViewModels/
│   ├── LoginViewModel.cs
│   ├── SettingsViewModel.cs              # Tab1Knife + Tab2Box + Tab3Other + Footer F2/F3
│   ├── SettingsTab1KnifeViewModel.cs
│   ├── SettingsTab2BoxViewModel.cs
│   ├── SettingsTab3ViewModel.cs
│   ├── Tab3GridRowViewModel.cs
│   └── UserPasswordRowViewModel.cs
├── Views/
│   └── Settings/
│       ├── SettingsView.xaml             # Tab1～Tab3 子 VM；Tab2 外層 Border
│       └── Tabs/
│           ├── SettingsTab1KnifeSafetyView.xaml(.cs)
│           ├── SettingsTab2BoxParametersView.xaml(.cs)
│           └── SettingsTab3OtherPlcParametersView.xaml(.cs)
└── Resources/
    ├── Icons/Box_Measurement_3-removebg-preview.png
    └── Resources*.resx
```

### 程式碼要點

- **Tab1 置中**：外層 **`ScrollViewer`** 設 **`VerticalContentAlignment="Center"`**，內層 **`Border`** **`MaxWidth="1080"`**、**`HorizontalAlignment="Center"`**，避免寬螢幕時內容靠左。
- **F2／F3**：**F2** 使用 **同一個** `SqliteConnection + BeginTransaction()`，並呼叫 **`Tab1Knife.TrySaveToDatabase(connection, tx)`** → **`Tab2Box.TrySaveToDatabase(connection, tx)`** → **`Tab3Other.TrySaveToDatabase(connection, tx)`**；任一頁失敗則整體 rollback。**F3** 同時 **`Tab1Knife`／`Tab2Box`／`Tab3Other`** **`ReloadFromDatabase()`**。**F5／F6** 仍為 Log 占位。
- **Tab2 可見性**：與 Tab1 相同，子頁 **`DataContext`** 為子 VM，**`Visibility`** 由外層 **`Border`** 綁 **`SettingsViewModel.SelectedSettingsTabIndex`**，避免子 VM 無 **`SelectedSettingsTabIndex`** 導致不顯示。
- **Tab2 F3 刀組合**：**`RebuildComboLists`** 會 **`Clear`** 刀組合 **`ObservableCollection`**；若重載後選中 **`OptionId`** 與先前相同，**`ObservableProperty`** 可能不觸發，ComboBox 顯示空白。**`RefreshKnifeComboDisplayAfterItemsSourceRebuild`** 在對齊合法 Id 後，若有另一筆選項則短暫改選再還原，並 **`OnPropertyChanged`** 刀組合與車速。
- **PLC 表**：與 §7.5.5 對齊；日後可抽成 `Scripts/*.sql` 或擴充欄位。
- **Tab3**：**`Tab3PlcCatalog`** 定義 Feed／Print1～3／7／8／Other 列與 **`PlcParameterDefinitions.Key`** 前綴；**`ComponentDisplayNameOverrides.ReplaceAll`** 儲存覆寫；切換部門前 **`MergeGridIntoMaster`** 將 **`DataGrid`** 寫回記憶體字典。
- **Tab3 多國語 root cause（日文）**：`Resources.ja.resx` 內 Tab3 相關 key（分頁標題／欄位標題／GroupBox 標題／Feed 與部門元件名稱等）先前是英文佔位，導致切到日文時看起來像永遠走到 fallback；同時移除 `SettingsTab3OtherPlcParametersView.xaml` header/password TextBlock 的 `FallbackValue/TargetNullValue` workaround，回到純 RESX/`LocalizedString.Value`。
- **後續**：**`IPlcSettingsService.SaveSettingsShell`**、Tab4／5 — 見 **`TODO_Phase7.md`**。

### 使用範例（Tab1 資料）

```csharp
IKnifeSafetyParameterRepository repo = new KnifeSafetyParameterRepository(databaseService);
IReadOnlyDictionary<string, int?> values = repo.LoadAll();
// Key 如 KnifeSafetyParameterKeys.KnifeSlot_A_Min
```

### 使用範例（Tab2 資料）

```csharp
IBoxDieCutterSettingsRepository box = new BoxDieCutterSettingsRepository(databaseService);
BoxDieCutterSnapshot snap = box.Load();
box.Save(snap);
```

### 規格對照（尚未程式化之定案）

| 項目 | 說明 | 檔案路徑 |
|------|------|----------|
| 全頁 F2 單一 transaction、獨立 SQL 腳本 | 仍依主規 | `TODO_Phase7.md` §7.5～§7.7 |
| Tab4／5 內容 | 本階段留白 | `TODO_Phase7_Layout.md` |

---

## 後續 Phase 整合提示

- **Phase 4 主畫面**：已完成，MainView 共用 `ILocalizationService`，返回按鈕 ToolTip 已綁定 RESX
- **Phase 4 (Layout2)**：已完成，登入後導航至 Layout2，含排程/完成訂單、模擬生產、F4/F7/F11 Dialog；F1 改為開啟 Phase 5 排程管理 Dialog，模擬由 F2/F3 觸發（小量/全量）
- **Phase 5 排程管理子頁**：已完成，ScheduleManageDialog（排單管理）、F2 前置排單（小量 5 個）/ F3 把全排量（全量）/ F1 離開，與 Layout2 模擬邏輯整合
- **Phase 6 訂單製作**：已完成，Orders 表與種子 100 筆、OrderMakingView 主內容區頁面（4.1～4.6）、流程 3.0～3.4（搜尋/編輯/新增/刪除、F2 加入排程）、IOrderRepository、F7 由 Layout2 導航至訂單製作
- **Phase 6 整合與收尾（6.1–6.6）**：已完成；含綁定與流程、錯誤處理、Log 整合、程式品質、多國語系與字型、應用程式圖示（icons8-app-96）
- **Layout2 圖示**：左側頂部為 icons8-favorites-shield-5-stars-96（96×96）；異常狀態使用 WpfAnimatedGif 播放 red-alam.gif
- **Phase 6.7 以後**：發佈、Win 8.1 測試、SqliteViewer 小工具等見 TODO.md
- **發佈建議**：Demo／測試優先使用單一執行檔（`scripts\publish.ps1` 或 `dotnet publish -r win-x64 --self-contained true -p:PublishSingleFile=true`）；正式產品再考慮安裝程式。說明見 `QandA/howToPackApplication.md`。
- **異機執行**：資料庫預設改為 `%LocalAppData%\CursorTestApp\app.db`，並加上未處理例外與登入後導航 try-catch，避免單檔在另一台電腦登入後 crash。詳見 `QandA/crashAfterLoginOnOtherPC.md`。
- **LogListBox 異機**：LogPanel 綁定之 LogMessages 在異機曾出現「ItemsControl 與其項目來源不一致」；已以 InvokeAsync(Loaded)、關閉虛擬化、ScrollIntoView 延後至 Loaded 修正，異機驗證通過。詳見 `LessonLearn/whyItemSourceNotConsistant.md`。

---

*建立日期：2025-02-22*
*更新：Phase 5 排程管理子頁改為 DataGrid；Phase 6 整合與收尾；LogListBox 異機修正（2026-02-26）；Phase 6 訂單製作（F7）實作（2026-03-01）；Phase 6 重大修改（2026-02-28）：4.4 不分頁+捲軸、GetAll、上/下箭頭同版號導航、雙擊列帶出、F1 最右、版號/版號查詢定位選取並捲動至中央、RequestScrollToSelected 僅程式觸發、右側欄位 RESX 與建檔日期含時間、4.5 相位標籤上下排列、頂部輸入框焦點樣式與備註高度、LessonLearn whyTextBoxIsGrayonFocus / whyMoveWhenMouseClick；Phase 7 文件定案：`CorrugatedTypes`/Profile(0)、Tab3 RESX 附錄（2026-03-19）；Phase 7 Print 鍵名統一＋廢止對照＋盲點檢查表（2026-03-19）；**Phase 7 Settings 骨架**（SettingsView、Admin 分流、Admin 種子、底部 F 鍵 RESX+Icons，2026-03-22）；**Settings 左側垂直分頁 + 淡色配色**（2026-03-22）；**Settings 改為左窄 ListBox + 右大面積內容**（取代 TabControl 版面，2026-03-22）；**左欄貼齊無外框 + 直向 Tab 標題**（2026-03-22）；**主內容滿版留白移除、Tab 標題深藍直排一字一列（1.2× 字級）**（2026-03-22）；**Phase 7 Tab1 刀位安全**（`PlcParameter*`、`IKnifeSafetyParameterRepository`、置中版面、`SettingsTab1KnifeViewModel`、F2 儲存／F3 重載，2026-03-23）；**Phase 7 Tab2 紙箱參數**（`IBoxDieCutterSettingsRepository`、`SettingsTab2BoxViewModel`、三欄版面、`SettingsTab2_*` RESX、F2／F3 含 Tab1+Tab2，2026-03-23）；**Phase 7 Tab3 日文 RESX 修正（並移除 XAML fallback）**（2026-03-23）*
