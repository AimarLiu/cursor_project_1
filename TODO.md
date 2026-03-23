# Windows 桌面應用程式開發 - Todo List

## 應用程式規格
- **用途**：客戶Demo及測試 Cursor 實際使用效果
- **技術棧**：C#, **.NET 8 (LTS)**, WPF, WPF-UI（Fluent Design）, MVVM, SQLite
- **目標平台**：**Windows 8.1** 及以上（.NET 8 官方支援 Windows 10 1607+；Win 8.1 需額外驗證）
- **部署方式**：**Framework-dependent**（需使用者安裝 .NET 8 Desktop Runtime）
- **功能**：登入驗證 → 正確則跳轉Layer2
- **特色**：右下角 Log、Fluent Design、自訂背景、**多國語系**（日、繁中、葡、英、泰）

---

## 平台與部署說明

| 項目 | 說明 |
|------|------|
| .NET 8 (LTS) | 長期支援，支援至 2026 年 11 月 |
| 官方支援 OS | Windows 10 1607+、Windows 11、Windows Server 2016+ |
| 部署方式 | Framework-dependent，發佈後使用者須安裝 **.NET 8 Desktop Runtime** |

---

## 使用者端需安裝的 Distribution 套件

為讓應用程式（含 WPF-UI）正確運行於 **Windows 8.1 及以上**，使用者須預先安裝以下元件：

| 套件 | 用途 | 下載連結 |
|------|------|----------|
| **.NET 8 Desktop Runtime** | WPF 與 .NET 8 執行必要 | https://dotnet.microsoft.com/download/dotnet/8.0 |
| **Visual C++ Redistributable (2015-2022) x64** | WPF-UI 及部分原生相依使用 | https://aka.ms/vs/17/release/vc_redist.x64.exe |
| **Universal CRT** | 部分 Win 8.1 環境需額外安裝 | 隨 Windows Update 或獨立安裝包 |

- [ ] 於安裝指引或 README 中列出以上套件
- [ ] （可選）提供批次檔或安裝腳本，協助使用者一次安裝

---

## Windows 8.1 模擬測試

> ⚠️ .NET 8 官方支援為 Windows 10 1607+，Windows 8.1 未列為支援。以下測試用於相容性驗證；若執行失敗，可視為預期。

- [ ] 使用 **Hyper-V** 或 **VMware** 建立 Windows 8.1 虛擬機器
- [ ] 在 VM 內依序安裝：
  1. .NET 8 Desktop Runtime
  2. Visual C++ Redistributable 2015-2022 x64
  3. 必要時安裝 Universal CRT / KB2999226
- [ ] 複製發佈後的 exe 至 VM，執行並驗證：
  - [ ] 應用程式能啟動
  - [ ] WPF-UI 控制項顯示正常（可接受視覺降級）
  - [ ] 登入流程與導航正常
- [ ] 記錄 Win 8.1 上的已知問題或限制

---

## 設計規範摘要

### 預設字型（Windows 8.1 內建、多語系相容）
| 語言 | 建議字型 | 備註 |
|------|----------|------|
| 英文 / 葡萄牙文 | Segoe UI | Win 8.1 預設 |
| 日文 | Meiryo / Meiryo UI | 日文專用 |
| 繁體中文 | Microsoft JhengHei | 繁中專用 |
| 泰文 | Leelawadee UI | 泰文專用 |
| **預設 Fallback** | `Segoe UI, Meiryo UI, Microsoft JhengHei, Leelawadee UI` | 依序選用系統有的字型 |

- [ ] 在 `App.xaml` 或全域 ResourceDictionary 設定預設 `FontFamily`
- [ ] 避免使用 Consolas、Cascadia 等 Win 8.1 可能未內建的字型
- [ ] Log 區若需等寬，可用 **Courier New**（Win 8.1 內建）

### 多國語系（i18n）
- [ ] 支援語系：**ja**（日）、**zh-TW**（繁中）、**pt**（葡）、**en**（英）、**th**（泰）
- [ ] **預設語系**：系統目前語系，可隨時動態切換
- [ ] 使用 RESX 資源檔：`Resources.resx`（預設）、`Resources.ja.resx`、`Resources.zh-TW.resx`、`Resources.pt.resx`、`Resources.th.resx`
- [ ] 或使用 `ResourceDictionary` + 語系切換（如 `xml:lang` + 動態載入）
- [ ] 實作 `ILocalizationService` / `CultureInfo` 切換

### 背景
| 元素 | 設計 |
|------|------|
| 主色 | 深色漸層 `#1a1a2e` → `#16213e` → `#0f3460` |
| 裝飾 | 半透明圓形、Blur、左上/右下製造景深 |
| 中央卡片 | 圓角矩形、半透明白/深色、層次感 |
| 備選 | WPF-UI Acrylic 毛玻璃 |

### Log 區域（右下角）
| 屬性 | 規格 |
|------|------|
| 位置 | 固定視窗右下角，約 25% 寬 × 15% 高 |
| 控制項 | TextBox/RichTextBox，ReadOnly |
| 樣式 | 半透明深色背景、淺色字、**Courier New** 等寬（Win 8.1 內建） |
| 格式 | `[HH:mm:ss] 訊息內容` |

---

## Phase 0：環境設定

### 0.1 必要軟體安裝
- [x] 安裝 **.NET 8 SDK**
  - 下載：https://dotnet.microsoft.com/download/dotnet/8.0
  - 驗證：`dotnet --version`
- [x] 安裝 **Visual Studio 2026 community**
  - 選「.NET 桌面開發」
  - 勾選「Windows 桌面應用程式」工作負載
- [ ] （可選）安裝 **Visual Studio Code** + C# Dev Kit

### 0.2 發佈設定（Framework-dependent）
- [ ] 發佈指令：`dotnet publish -c Release`
- [ ] 輸出位於 `bin/Release/net8.0-windows/publish/`
- [ ] 使用者端須已安裝 **.NET 8 Desktop Runtime**（見上方 Distribution 套件）

### 0.3 Fluent Design 套件
- [x] 安裝 **WPF-UI**
  - NuGet：`dotnet add package WPF-UI`
  - 官網：https://github.com/lepoco/wpfui
  - 視覺降級可接受（Acrylic 等在 Win 8.1 可能退化為純色）
- [x] （可選）**FluentIcons.Wpf**
  - NuGet：`dotnet add package FluentIcons.Wpf`
- [x] 在 `App.xaml` 中引入 WPF-UI 資源字典與主題

### 0.4 資料庫與其他套件
- [x] 安裝 **Microsoft.Data.Sqlite**
  - NuGet：`dotnet add package Microsoft.Data.Sqlite`
- [x] （可選）**CommunityToolkit.Mvvm**
  - NuGet：`dotnet add package CommunityToolkit.Mvvm`

### 0.5 動態圖套件 WpfAnimatedGif
- [x] 安裝 **WpfAnimatedGif**
  - NuGet：`dotnet add package WpfAnimatedGif`（本專案使用 2.0.2）
  - 用途：WPF 內建 `Image` + `BitmapImage` 僅顯示 GIF 第一幀，此套件可播放 GIF 動畫
- [x] 於 Layout2 異常狀態圖示使用：RS485 錯誤時顯示 `red-alam.gif` 動畫，平時顯示 `alarm.png` 靜態圖
- [x] XAML：`xmlns:gif="http://wpfanimatedgif.codeplex.com"`，以 `ImageBehavior.AnimatedSource` / `SetAnimatedSource` 綁定 GIF

---

## Phase 1：專案建立與架構

### 1.1 建立專案
- [x] 建立 WPF 專案：`dotnet new wpf -n CursorTestApp -o src`
- [x] 設定目標框架：`<TargetFramework>net8.0-windows</TargetFramework>`
- [x] 建立解決方案結構
  ```
  CursorTestApp.sln
  src/
    CursorTestApp/           # 主專案（UI、Views、ViewModels）
      Properties/
      Resources/             # RESX 語系檔
  ```

### 1.2 MVVM 架構
- [x] 建立 ViewModel 基底（`ViewModelBase`）與 `INotifyPropertyChanged`
- [x] 考慮使用 **CommunityToolkit.Mvvm** 或手寫 ViewModel
- [x] 定義專案資料夾：`Views`、`ViewModels`、`Models`、`Services`、`Helpers`

### 1.3 多國語系結構
- [x] 建立 `Resources.resx`（預設語系，建議英文）
- [x] 建立 `Resources.ja.resx`（日語）、`Resources.zh-TW.resx`（繁中）、`Resources.pt.resx`（葡）、`Resources.th.resx`（泰）
- [x] 實作 `ILocalizationService`，支援 `CultureInfo` 切換
- [x] 字串一律使用 `Resources.XXX`，勿寫死在 XAML / Code

### 1.5 Ignore 檔建立
- [x] 建立 `.gitignore`，忽略 `bin/`、`obj/`
- [x] 建立 `.cursorignore`，忽略 `bin/`、`obj/`

---

## Phase 2：資料庫

### 2.1 SQLite 設定
- [x] 設計使用者表 `Users`：Id, Username, Password（**明文**即可，本專案為測試用）
- [x] 實作 `IDatabaseService` / `IAuthService` 介面
- [x] 實作密碼比對邏輯（直接字串比對）
- [x] 提供初始化與建表腳本（`Scripts/CreateUsersTable.sql`）

### 2.2 初始資料
- [x] 建立至少一筆測試帳號供登入驗證（0001 / aimarliu / aimarliu）

---

## Phase 3：登入畫面（Login View）

### 3.1 整體佈局
- [x] 建立 `LoginView.xaml`，使用 WPF-UI 的 `ui:UiWindow` 或 `ui:FluentWindow`
- [x] 視窗符合螢幕大小：`WindowState="Maximized"`
- [x] **右下角 Log 區域**（所有頁面共用）：
  - [x] 固定於視窗右下角，約 25% 寬 × 15% 高（可調整）
  - [x] 使用 `ListBox` 綁定 `LogMessages`，`ScrollViewer.VerticalScrollBarVisibility="Auto"`
  - [x] 半透明深色背景、淺色字、等寬字型 **Courier New**（Win 8.1 內建）
  - [x] 實作 `ILogService`，透過 ViewModel 綁定 `LogMessages` 集合
  - [x] 所有關鍵操作（登入嘗試、驗證結果、導航等）寫入 Log
  - [x] `IsHitTestVisible="True"` 以允許捲動（滾輪、捲軸）
  - [x] 新增訊息時自動捲到底（訂閱 `CollectionChanged`，呼叫 `ScrollIntoView`）

### 3.2 中央登入區塊
- [x] 使用 WPF-UI 的 `ui:PasswordBox`、`ui:Button` 達成 Fluent 風格
- [x] 畫面正中央包含：
  - [x] 密碼輸入（`PasswordBox`）
  - [x] Enter 按鈕
  - [x] Cancel 按鈕（清除密碼）
- [x] 視覺排版：垂直堆疊、居中、適當間距

### 3.2.1 語系切換（Login 方框下方）
- [x] 在 Login 方框**下方**放置語系切換 ComboBox
- [x] 支援語系：ja、zh-TW、pt、en、th
- [x] **預設語系**：系統目前語系（`SupportedCultures.ResolveFromSystem()`）
- [x] 使用者選擇後**立即動態切換**介面語系（呼叫 `ILocalizationService.SetCulture`）
- [x] **動態切換**：使用 `LocalizedString` 包裝類，訂閱 `CultureChanged`，控制項以 Binding 綁定 `.Value`
- [x] 選項顯示：以該語系顯示名稱（日本語、繁體中文、Português、English、ไทย）

### 3.2.2 右上角離開按鈕
- [x] 登入畫面**右上角**放置 **X** 圖示按鈕
- [x] 點擊後關閉並結束應用程式（`Application.Current.Shutdown()`）

### 3.3 背景設計
- [x] **主背景**：深色漸層（#1a1a2e → #16213e → #0f3460）
- [x] **裝飾層**：半透明圓形或柔和形狀（Blur + Opacity）
  - [x] 左上角大圓、右下角中圓，製造景深
- [x] **中央卡片**：登入區塊外層加圓角矩形、半透明白色背景，與背景形成層次

### 3.4 ViewModel
- [x] `LoginViewModel`
  - [x] `Password`（綁定至 PasswordBox）
  - [x] `LoginCommand`（Enter）
  - [x] `CancelCommand`（清除密碼）
  - [x] 注入 `ILogService`，記錄「正在驗證…」「登入成功/失敗」
  - [x] 登入成功時觸發導航；失敗時寫入 Log

### 3.5 Cancel 功能
- [x] Cancel 按鈕執行後清空密碼輸入欄位，並於 Log 輸出「已清除輸入」

---

## Phase 4：主畫面與導航

### 4.1 主畫面
- [x] 建立 `MainView.xaml`（登入成功後），依作法一置於 `Views/Main/`
- [x] 沿用與登入頁相同的背景風格，保持一致性
- [x] **主畫面不需 Log 區域**（NavigateToMain 時隱藏 LogPanel）
- [x] **中央區域**：顯示「This is main page」文字，依當下所選語系動態切換（`LocalizedString` / RESX `MainPageTitle`）
- [x] **左下角返回按鈕**：Unicode 字元 **⬅** 返回登入頁面（`NavigateBackCommand`）

### 4.2 導航機制
- [x] 實作導航服務（`INavigationService`）
- [x] 使用 `ContentControl` 切換 View
- [x] 登入成功 → 寫入 Log「導航至主畫面」→ 切換至 MainView
- [x] 左下角 ⬅ 按鈕 → 返回登入頁（呼叫 `INavigationService.NavigateToLogin`）

---

## Phase 4 (Layout2)：主畫面2與導航

> **概述**：自動化紙箱製作工廠操作介面。含 Background Service 讀寫 RS485（COM1: 9600,8,N,1），與「模擬生產狀態」邏輯（排程自動執行、車速／產量／預估時間更新）。登入成功後可導航至本畫面，與原 Phase 4 主畫面並存（ShellWindow 可切換）。

### 整體架構
- **頂部**：左側狀態列（生產資訊看板標題、OPT/PLC LED、圖示）+ 右側訂單管理導航列（生產排程標題 LED、F2/F4/F6 按鈕）
- **主體**：左側生產資訊看板 + 右側訂單面板（生產排程 DataGrid、調單、生產完成訂單 DataGrid）
- **底部**：控制列（返回、F7 訂單製作、F1 排程管理、異常圖示、F11 狀態顯示、關機）
- **視覺**：明亮淡色系

### 左側頂部狀態列
| 區塊 | 項目 | 說明 | 可用控制項或圖示 | 功能模擬 |
|------|------|------|------|------|
| 左側頂部 | 標題 | 生產資訊看板 | 預設 | 無 |
| 左側頂部 | 生產運作狀態 | "OPT"字樣，附LED圖示(紅燈與白燈) | Ellipse + Trigger | 收到RS485傳來的資料時會恆亮紅燈 ex: "ACTIVE+" |
| 左側頂部 | PLC訊號狀態 | "PLC"字樣，附LED圖示(綠燈與白燈) | Ellipse + Trigger | 有收到RS485訊號時綠燈閃爍 |
| 左側頂部 | 圖示 | png 圖示 | Image，icons8-favorites-shield-5-stars-96.png | 無 |

### 右側頂部訂單管理導航列
| 區塊 | 項目 | 說明 | 可用控制項或圖示 | 功能模擬 |
|------|------|------|------|------|
| 右側頂部 | 標題 | "生產排程"字樣，附 LED 圖示（綠燈與紅燈） | Ellipse + Trigger | 當 Layout2 處於「模擬生產狀態」時紅燈（由 Phase 5 Dialog F2/F3 觸發後啟動），待機時綠燈 |
| 右側頂部 | F2 生產完成 | 按鈕，附圖示（使用 icons8-production-finished-80.png） | Button | 將生產排程 DataGrid 所選項目移入生產完成訂單 DataGrid，並自排程移除；完成訂單多「日期時間」欄，為插入當下時間 |
| 右側頂部 | F4 預覽 | 按鈕，附圖示（使用 icons8-search-property-80.png） | Button | 開啟 Dialog（確定/取消），輸入搜尋條件後於生產排程 DataGrid 搜尋並選取該筆 |
| 右側頂部 | F6 撤單 | 按鈕，附圖示（使用 icons8-keep-clean-96.png） | Button | 刪除生產排程 DataGrid 所選項目，下方項目上移 |

### 左側面板：生產資訊看板（使用 Grid）
| 欄位 | 範例值 | 功能模擬 |
|------|-------|-------|
| 生產訂單 | 從生產排程 DataGrid 第一筆取得生產訂單號 | 無 |
| 生產版號 | 從生產排程 DataGrid 第一筆取得版號 | 無 |
| 車速 | 0 | RS485 顯示值（例 RPM:500）；模擬生產狀態時亂數 400–500，每秒更新 |
| 目前產量 | 0 | 模擬生產狀態時每 30 秒 +1 |
| 受訂量 | 從生產排程 DataGrid 第一筆取得 | 無 |
| 預估完成所需時間 | 「停車中」或預估時間（模擬生產狀態下依已生產數量動態更新） | 模擬時每 30 秒一筆可更新 |

### 右側訂單面板

#### 上區：生產排程（DataGrid）
- **表格欄位**：生產訂單號、版號、受訂量、箱型、類別、客戶名稱、備註
- **範例資料**：0008 / 20260218 / 10 / E / A / Aimar / (空)；0010 / 20260220 / 300 / E / A / Mason / (空)；0011 / 20260226 / 800 / E / A / Jenny / (空)
- **額外設定**：取消可排序功能，啟用虛擬化（VirtualizingStackPanel.IsVirtualizing="True"）

#### 中區：調單
| 位置 | 項目 | 說明 | 可用控制項或圖示 | 功能模擬 |
|------|-------|-------|-------|-------|
| 左側 | 標題 | "調單"字樣 | 預設 | 無 |
| 中間 | 訂單上移 | 上箭頭按鈕（使用 icons8-arrowup-40.png） | Button | 所選項目與上一筆對調 |
| 右側 | 訂單下移 | 下箭頭按鈕（使用 icons8-arrowdown-40.png） | Button | 所選項目與下一筆對調 |


#### 下區：生產完成訂單（DataGrid）
- **表格欄位**：日期時間、生產訂單號、版號、受訂量、箱型、類別、客戶名稱、備註
- **範例資料**：2026/01/10 13:01:12、0004、20260110、600、E、A、Aimar、(空)；2026/02/11 11:52:42、0005、20260110、199、E、A、Mason、(空)
- **額外設定**：取消可排序功能，啟用虛擬化（VirtualizingStackPanel.IsVirtualizing="True"）

### 底部控制列
| 位置 | 項目 | 說明 | 可用控制項或圖示 | 功能模擬 |
|------|------|------|------|------|
| 最左 | ⬅ 按鈕 | 返回登入頁 | 預設 | 呼叫 `INavigationService.NavigateToLogin` |
| 左 | F7 訂單製作 | 按鈕，附圖示（使用 icons8-packing-96.png） | Button | 開啟新空白頁面（含右上 X 關閉） |
| 中左 | F1 排程管理 | 按鈕，附圖示（使用 icons8-schedule-96.png） | Button | 先進入Phase 5的排程管理子頁， 若進入「模擬生產狀態」圖示變為icons8-pause-squared-96.png且文字變為"F1 生產中"（RESX，多語系），直到停止或手動按下，圖示切回原本的icons8-schedule-96.png與文字"F1 排程管理" |
| 中右 | 異常狀態圖示 | 警示三角（red-alam.gif） | Image 或 WpfAnimatedGif | 平時 alarm.png；RS485 錯誤時改為 alarm.gif（如 "ERROR+"） |
| 右 | F11 狀態顯示 | 按鈕，附圖示（使用 icons8-warning-96.png） | Button | 開啟 Dialog 頁面，含右上方 X |
| 最右 | 關機 | 按鈕，附圖示（使用 icons8-shutdown-96.png） | Button | 無 |

### 待實作項目（依 Layout2）

#### 整合
- [x] 將 `Resources\*.png` 移動至 `Resources\Icons\`
- [x] 自 `Resources\Icons\` 取得指定 png 圖示並綁定、顯示於上述各控制項
- [x] Layout2 文字改為 RESX Localization，支援動態語系切換

#### 結構與導航
- [x] 新增 Layout2 主畫面 View，登入成功後導航至 Layout2（並存 Phase 4 主畫面）
- [x] 擴充 `INavigationService`：`NavigateToLayout2`
- [x] ShellWindow / ContentHost 可切換 Login / Main(Layout1) / Layout2

#### 資料庫表單
- [x] 依生產排程項目欄位建立資料庫表（生產訂單號、版號、受訂量、箱型、類別、客戶名稱、備註）
- [x] 依生產完成訂單項目欄位建立資料庫表（含日期時間欄）

#### 資料與模型
- [x] 生產排程項目模型（生產訂單號、版號、受訂量、箱型、類別、客戶名稱、備註）；自 DB 載入（`IScheduleOrderRepository`）
- [x] 生產完成訂單項目模型（含日期時間欄）；自 DB 載入
- [x] 排程與完成訂單的集合（`ObservableCollection`）及選取項、調單（上移/下移）、F2/F6 等指令邏輯

#### UI 殼層與左側
- [x] Layout2 主畫面：左窄右寬版面（Grid）
- [x] 左側頂部狀態列：標題、OPT/PLC LED（Ellipse + BoolToLedBrushConverter）、圖示；左欄 400/MinWidth 360、右欄 MinWidth 420，避免長語系遮蓋右側圖示與按鈕
- [x] 左側生產資訊看板：Grid 平均分配、字體依語系（英文 16／其餘 20）；兩色區分上下欄位（#455570／#3d4a62）、外層 #3a4555；預估完成標題與估算值分兩行顯示、TextWrapping 防遮蔽

#### 右側訂單面板
- [x] 右側頂部：生產排程標題與 LED、F2 生產完成、F4 預覽、F6 撤單（含指令與圖示）
- [x] 上區：生產排程 DataGrid（欄位、選取、F2/F4/F6/調單 操作）；取消可排序、啟用虛擬化；標題由 code-behind 多語系填入、字體 18、標題列深灰、欄寬 1.5 倍、選取僅整列高亮
- [x] 中區：調單（訂單上移、訂單下移）按鈕與圖示
- [x] 下區：生產完成訂單 DataGrid（含日期時間欄）；取消可排序、啟用虛擬化；同上標題/字體/欄寬/選取設定
- [x] Layout2 按鈕以 Grid 調整適當長寬高：圖片靠左、文字居中（頂部: F2 生產完成/F4 預覽/F6 撤單、中區: 調單、底部: 返回/F7 訂單製作/F1 排程管理/F11 狀態顯示/關機）

#### 底部與 Dialog
- [x] 底部控制列：返回、F7 訂單製作、F1 排程管理、異常圖示、F11 狀態顯示、關機
- [x] F7 訂單製作：新空白視窗（含關閉 X）
- [x] F4 預覽：搜尋 Dialog（確定/取消），搜尋生產排程並選取
- [x] F11 狀態顯示：狀態 Dialog 視窗（含關閉 X）

#### 模擬生產與 RS485
- [x] **模擬生產狀態**（由 Phase 5 Dialog 之 **F2 前置排單**或 **F3 把全排量** 觸發後，關閉 Dialog 返回 Layout2 時啟動）：車速模擬 400–500、每 30 秒目前產量 +1、預估時間更新、完成項移入生產完成訂單並自排程移除。**停止條件**：F2（小量）＝僅第一筆訂單生產 5 個後自動停止；F3（全量）＝依序生產至排程清空或使用者手動停止。Layout2 需能接收 Dialog 關閉時帶回之模式（小量／全量）並據此驅動模擬。
- [x] **F1 排程管理按鈕狀態**：模擬生產狀態運行中時，按鈕圖示改為 icons8-pause-squared-96.png、文字改為「F1 生產中」（RESX）；按下可手動停止。停止後圖示與文字還原為 icons8-schedule-96.png 與「F1 排程管理」。 
- [x] RS485 服務介面與實作（Rs485Service 模擬用，OPT/PLC LED、車速、HasError 綁定；實體 COM 可後續接上）
- [x] Hosted Service 或 BackgroundWorker 整合實體 RS485 與 UI 更新（`Rs485BackgroundService` 讀取 COM1 並更新 Rs485Service；無 COM 時 F1 模擬仍可更新）

### 可能新增的檔案與目錄結構

```
src/
├── Views/
│   ├── Login/                    # 既有
│   ├── Main/                     # 既有 Phase 4 主畫面
│   ├── Layout2/                  # 新增：Layout2 主畫面
│   │   ├── Layout2View.xaml
│   │   ├── Layout2View.xaml.cs
│   │   └── ProductionInfoPanel.xaml(.cs)   # (必須)左側生產資訊看板 UserControl
│   └── Shared/                   # 既有
├── ViewModels/
│   └── Layout2ViewModel.cs       # Layout2 主 ViewModel
├── Models/
│   ├── ScheduleOrderItem.cs      # 生產排程一筆（訂單號、版號、受訂量、箱型、類別、客戶、備註）
│   └── CompletedOrderItem.cs    # 生產完成訂單一筆（含日期時間）
├── Services/
│   ├── INavigationService.cs     # 擴充 NavigateToLayout2 等
│   ├── IProductionScheduleService.cs  # (必須) 排程/完成訂單 操作
│   ├── IRs485Service.cs          # RS485 讀寫介面
│   └── Rs485Service.cs           # RS485 實作 (COM1: 9600,8,N,1)
├── Resources/
│   └── Icons/                  # icons8-*.png、alarm.png、alarm.gif
└── ...
```

- **Layout2View**：左側面板 + 右側訂單面板（兩顆 DataGrid + 調單）+ 頂部兩列 + 底部控制列。
- **圖示**：集中放在 `Resources/Icons/`，XAML 以 pack URI 或資源鍵參考。

---

## Phase 5：排程管理子頁（Dialog）

> **概述**：由 Layout2 **F1 排程管理**按鈕開啟的「排單管理」對話框，顯示目前排單版號與生產訂單列表。在此 Dialog 可執行 **F2 前置排單**（小量模擬）或 **F3 把全排量**（全量模擬），兩者皆會關閉 Dialog、返回 Layout2 並**啟動模擬生產狀態**；若僅 **F1 離開**則只關閉 Dialog，不啟動模擬。

### F1 排程管理、F2 前置排單、F3 全排單與模擬生產狀態之關係

| 位置 | 按鍵 | 行為 |
|------|------|------|
| **Layout2 底部** | F1 排程管理 | 開啟本 Phase 5「排程管理子頁」Dialog（排單管理）。若 Layout2 已處於模擬生產中，則按鈕顯示為「F1 生產中」+ 暫停圖示，可手動停止。 |
| **Dialog 內** | F2 前置排單 | 關閉 Dialog、返回 Layout2，並**啟動模擬生產狀態（小量）**：僅生產**第一筆**訂單 **5 個**後自動停止；車速／產量／預估時間依 Phase 4 模擬邏輯。 |
| **Dialog 內** | F3 把全排量 | 關閉 Dialog、返回 Layout2，並**啟動模擬生產狀態（全量）**：依序生產**每筆**訂單直到**全部完成**或使用者手動停止；車速／產量／預估時間依 Phase 4 模擬邏輯。 |
| **Dialog 內** | F1 離開 | 僅關閉 Dialog、返回 Layout2，**不**啟動模擬生產。 |

- **模擬生產狀態**（實作於 Phase 4 Layout2）：車速模擬 400–500、每 30 秒目前產量 +1、完成項移入生產完成訂單並自排程移除、預估完成時間更新；**停止條件**由從 Dialog 所選模式決定——F2 為「第一筆做 5 個即停」，F3 為「全部做完或手動停」。

### 控制項與 Layout 配置

| 區塊 | 項目 | 說明 | 控制項／備註 |
|------|------|------|--------------|
| 標題列 | 對話框標題 | 「排單管理」 | Window / Dialog Title |
| 頂部 | 版號說明 | 標籤「您目前正要排單的版號:」 | TextBlock(優先) / Label |
| 頂部 | 目前版號 | 唯讀顯示目前排單版號（如 20260218） | TextBlock(優先) 或 TextBox ReadOnly |
| 中央 | 資料表格 | 生產訂單列表，支援垂直捲動 | DataGrid(優先) 或 ListView/GridView |
| 表格欄位 | 生產訂單 | 欄標題可為黃底 | 欄位 1 |
| 表格欄位 | 版號 | 資料可為數字，文字或特殊符號（ex: 0004） | 欄位 2 |
| 表格欄位 | 受訂量 | 資料為數字（ex: 10） | 欄位 3 |
| 表格欄位 | 箱型 | 資料為英文字（如 E） | 欄位 4 |
| 表格欄位 | 楞別 | 資料為英文字（如 A、B） | 欄位 5 |
| 表格欄位 | 客戶名稱 |  資料為英文，數字或特殊符號 | 欄位 6 |
| 表格欄位 | 備註 | 資料為英文，數字或特殊符號 | 欄位 7 |
| 底部 | F4 分印 | 按鈕 + 圖示（icons8-bear-footprint-80.png） | Button |
| 底部 | F2 前置排單 | 按鈕 + 圖示（icons8-frying-pan-96.png） | Button |
| 底部 | F3 把全排量 | 按鈕 + 圖示（icons8-process-80.png） | Button |
| 底部 | F1 離開 | 按鈕 + 圖示（icons8-exit-96.png） | Button |

### 5.1 對話框與版號區
- [x] 新增排程管理子頁為 **Dialog/Window**（由 Layout2 F1 排程管理進入）
- [x] 標題列顯示「排單管理」（RESX，多語系）
- [x] 頂部區：標籤「您目前正要排單的版號:」+ 唯讀目前版號顯示（資料來源：依規格訂定，排程第一筆版號）（RESX，多語系）

### 5.2 資料表格（生產訂單列表）
- [x] 中央區使用 **DataGrid**（與 Layout2 上區生產排程相同控制項）顯示生產訂單列表，欄位：**生產訂單、版號、受訂量、箱型、楊別、客戶名稱、備註**（RESX，多語系）
- [x] 欄標題樣式：title 整行背景黃色、文字黑色；樣式設定參考 Layout2 生產排程並避免 whyDataGridleg（標題 code-behind 填入、Row/Cell 選取樣式、系統選取色、虛擬化 Recycling）
- [x] 支援**垂直捲動**；資料可為數字或文字，版號、客戶名稱和備註欄可有特殊字
- [x] 綁定資料來源（現有排程資料）

### 5.3 底部按鈕列
- [x] 底部區塊背景**黃色**（#F9A825），按鈕文字**粗體**、深色（#111）以利辨識
- [x] 底部四顆按鈕：**F4 分印**、**F2 前置排單**、**F3 把全排量**、**F1 離開**，每顆附左圖示與置中文字（RESX，多語系）
- [x] **F1 離開**：關閉 Dialog 並返回 Layout2，不啟動模擬
- [x] **F2 前置排單**：關閉 Dialog、返回 Layout2，並以**小量模式**啟動模擬生產狀態（僅第一筆訂單生產 5 個後自動停止）；與 Phase 4 模擬邏輯對接（車速、每 30 秒 +1、完成項移入完成訂單）
- [x] **F3 把全排量**：關閉 Dialog、返回 Layout2，並以**全量模式**啟動模擬生產狀態（依序生產至全部完成或手動停止）；與 Phase 4 模擬邏輯對接
- [x] **F4 分印**：後續 Phase 補齊，暫無功能

### 5.4 與 Layout2／排程資料與模擬邏輯整合
- [x] 進入時機：由 Layout2 **F1 排程管理**按鈕開啟此 Dialog（見 Phase 4 底部控制列）
- [x] 目前版號與表格資料與 `IProductionScheduleService`、`IScheduleOrderRepository` 對接（可先沿用現有排程／完成訂單資料）
- [x] 擴充 Phase 4「模擬生產狀態」：支援**兩種停止條件**——(1) 小量模式（由 F2 觸發）：第一筆訂單生產 5 個即停止；(2) 全量模式（由 F3 觸發）：依序生產至排程清空或手動停止。Layout2 需能接收從 Dialog 帶回的模式參數並據此驅動模擬

---

## Phase 6：訂單製作（F7 訂單製作）— 含查詢／編輯／新增

> **概述**：由 Layout2 **F7 訂單製作**開啟的「訂單製作」頁面。用於查詢版號、編輯既有訂單、新增訂單、預覽板號圖形，以及顯示版號列表與相位設定。

### 開啟時機
- 按下 **Layer2（Layout2）** 的 **F7 訂單製作** 按鈕時開啟本頁面（Stage 6 Layout）。

### 頁面呈現規範
- **頁面類型名稱**：**主內容區頁面**（與 Login、Layout2 相同）。訂單製作顯示於 **Shell 的 ContentHost**，不另開獨立視窗、不覆蓋 Windows 工作列／導覽列；由 F7 切換至本頁、F1 離開返回 Layout2。
- **背景與文字**：整體背景使用**白色與淺藍色系**搭配；**文字使用黑色**。
- **反灰狀態**：被反灰的區塊（左側輸入區、下側 4.5）**背景色為淺灰**（如 #E0E0E0）；**左側輸入區解除反灰時背景改為白色，下側 4.5解除反灰時背景改為白色**。
- **4.1 搜尋與 4.2 預覽**：**4.1 F6 搜尋**按鈕與 **4.2 預覽**按鈕為**兩個不同按鈕**，須使用不同圖示或明顯區隔。
- **4.5 下側** : 預覽區 | 相位 | 楞別圖 比例改為: 5:1:4
- **4.5 下側** : 預覽區的寬度顯示／短輸入框放在右側中間
- **4.5 下側** : 預覽區的長度顯示／短輸入框放在下側中間
- **楞別圖片**：圖片直接填滿該Grid分割區域，不做border，圖片可以接受失真
- **版面比例**：**4.3 左側輸入區**與 **4.4 右側數據顯示區**寬度比例為 **6（左）: 4（右）**（ColumnDefinitions 使用 `6*`、`4*`）。
- **頂部與底部輸入框**: 背景使用白色,字體黑色。
- **底部按鈕順序**：**F4 最左** → **F5 左** → **F2 中左** → **F1 最右**。

### 1. 介面說明
- **頂部左側**操作區、**左側輸入區**與**下側箱型預覽區**（含相位）依流程切換「可編輯／反灰」；底部按鈕（F4、F5、F2）依流程可點擊或反灰，**F1 離開**除外、始終可點擊。

### 2. 資料表 Orders 與種子資料

#### 2.1 表單 Orders
| 欄位 | 說明 | 型別建議 |
|------|------|----------|
| 建立日期時間 | 訂單建立時間 | DateTime |
| 生產訂單號 | 生產訂單編號 | 字串（4 碼（0100~0999）） |
| 版號 | 版號 | 字串 （例：西元年+序號）|
| 受訂量 | 數量 | 數值 |
| 箱型 | E 或 S 擇一 | 字串 |
| 楞別 | A, AB, B, BC, C, E 擇一 | 字串 |
| 相位1 | 1色相位 | 數值（或字串；種子為數值範圍見 2.2） |
| 相位2 | 2色相位 | 數值（或字串；種子為數值範圍見 2.2） |
| 相位3 | 3色相位 | 數值（或字串；種子為數值範圍見 2.2） |
| 長度 | 楞別長度 | 數值 |
| 寬度 | 楞別寬度 | 數值 |
| 客戶名稱 | 客戶名稱 | 字串（可空） |
| 備註 | 備註 | 字串（可空） |

#### 2.2 範例／種子資料（隨機產生 100 筆）
| 欄位 | 範例規則 |
|------|----------|
| 建立日期時間 | Random(2024/01/01 13:00:00 ~ 2026/01/10 13:00:00) |
| 生產訂單號 | 4 碼（0100~0999） |
| 版號 | 西元年 + 生產訂單號（例：2025001） |
| 受訂量 | Random(10 ~ 1000) |
| 箱型 | 擇一：E, S |
| 楞別 | 擇一：A, AB, B, BC, C, E |
| 相位1 | Random(50 ~ 150) |
| 相位2 | Random(200 ~ 299) |
| 相位3 | Random(300 ~ 399) |
| 長度 | Random(1000 ~ 5000) |
| 寬度 | Random(1000 ~ 5000) |
| 客戶名稱 | 隨機產生一個英文名 |
| 備註 | 空字串 |

- [x] 建立 `Orders` 表（含上述欄位）及建表腳本（如 `Scripts/CreateOrdersTable.sql`）。
- [x] 實作種子資料：隨機產生 100 筆符合上表規則的資料寫入 `Orders`。

### 3. 流程邏輯（查詢 → 有資料編輯／無資料新增）

#### 3.0 初始狀態（F7 訂單製作剛開啟）
- 開啟 Stage 6 頁面時：
  - **2. 左側輸入與下側箱型預覽區（含相位）**：**反灰**（不可編輯）。
  - **F5 完成編輯訂單(預設)/F5 新增訂單** 按鈕：**反灰**（不可點擊）。
  - **F4 刪除** 按鈕：**反灰**（不可點擊）。

#### 3.1 搜尋版號
- 使用者在 **1. 頂部操作區左側** 輸入 **版號**，按下 **Enter** 或 **F6 搜尋**。
- 以版號查詢資料庫 **Orders**：
  - **若找到資料**：
    - 將該筆資料帶出至 **2. 左側輸入、下側箱型預覽區的長度寬度及相位**（生產訂單、受訂量、箱型、楞別、相位1、相位2、相位3、長度、寬度、客戶名稱、備註等）。
    - 並定位到右側數據顯示區該筆資料並選取，被選取的資料須定位到右側數據顯示區的中間
    - **2. 左側輸入與下方箱型預覽區** **解除反灰**，可編輯。
    - **F5 完成編輯訂單** **解除反灰**，可點擊。
    - **F4 刪除** **解除反灰**，可點擊。
    - **F2 加入排程/離開** **解除反灰**，可點擊。
    - 使用者可修改後按下 **F5 完成編輯訂單** 寫入 **Orders**，並跳出 **提示 Dialog**：「此筆訂單:"版號"已儲存」（例：20260228）（**確認** 按鈕）。
  - **若沒有找到資料**：
    - 跳出 **提示 Dialog**：「是否新增此筆新的版號？」（**確認**、**取消** 按鈕）。
    - **若使用者按取消**：關閉 Dialog，維持目前狀態（左側及下側仍反灰、F2、F4、F5 仍反灰）。
    - **若使用者按確認**：
      - **2. 左側輸入與下側箱型預覽區** **解除反灰**，可輸入新訂單資料。
      - **F5 完成編輯訂單** 按鈕文字改為 **「F5 新增訂單」** 且 **解除反灰**。
      - **F2 加入排程/離開** **解除反灰**，可點擊。
      - 使用者填寫完成後按下 **F5 新增訂單** 寫入 **Orders**；儲存完成後：
        - **F5 新增訂單** 文字改回 **「F5 完成編輯訂單」** 並保持解除反灰、可點擊。
        - **F4 刪除** **解除反灰**，可點擊。

#### 3.2 頂部操作區右側：版號查詢
- 使用者在 **頂部操作區右側**「版號查詢」欄位輸入版號，按下 **Enter**。
- 以版號查詢右側數據顯示區 **DataGrid**，找到對應版號並選取該列；若有重複版號，選取下一筆對應的版號（不重頭循環）。

#### 3.3 預覽
- 按下 **預覽** 按鈕後，將右側 **DataGrid 目前選取的那一筆** 帶出至 **左側輸入區** 與 **下側箱型預覽區**（生產訂單、受訂量、箱型、楞別、相位1～3、長度、寬度、客戶名稱、備註等）；該區依流程可編輯（見 3.1）。

#### 3.4 F2 加入排程／離開
- 將目前編輯的這筆訂單加入 **Layout2 上區生產排程（DataGrid）**（即 Phase 4 上區；與 Phase 5.2 生產訂單列表為**同一排程資料來源**），並離開訂單製作頁面回到 Layout2。


### 4. Layout 配置與控制項（RESX，多語系支持）

#### 4.1. 頂部操作區左側
| 位置／項目 | 說明 | 控制項建議 |
|------------|------|------------|
| 版號 | 標籤「版號:」+ 輸入／顯示欄位（例：500*600），背景綠色；**搜尋時以此欄位查詢 Orders** | Label + TextBox，背景綠色 |
| F6 搜尋 | **獨立按鈕**，附圖示（見 5. 圖示對應）；觸發以版號查詢 Orders；與 4.2 預覽為不同按鈕 | Button |

#### 4.2. 頂部操作區右側
| 位置／項目 | 說明 | 控制項建議 |
|------------|------|------------|
| 版號查詢 | 標籤「版號查詢」+ 輸入／顯示欄位（寬度約為一般欄位 1.2 倍）；查詢 DataGrid 並選取對應列（見 3.2） | Label + TextBox |
| 上箭頭 | 導航按鈕（版號重複時移到上一筆，不循環）；按鈕寬度需足夠避免圖示被裁切 | Button + 上箭頭圖示（icons8-collapse-arrow-96.png） |
| 下箭頭 | 導航按鈕（版號重複時移到下一筆，不循環）；按鈕寬度需足夠避免圖示被裁切 | Button + 下箭頭圖示（icons8-expand-arrow-96.png） |
| 預覽 | **獨立按鈕**（與 4.1 F6 搜尋不同），將 DataGrid 選取列帶出至左側與下側（見 3.3）；附圖示（icons8-package-search-96.png） | Button + 圖示 |
| 送入版號位置 | 按鈕，附圖示 | Button + 圖示（ icons8-insert-96.png） |

#### 4.3. 左側輸入區
- **輸入／顯示欄位**：預設**白底**、**文字黑色**；**可輸入狀態**（取得焦點時）改為**黃底**，以利辨識。
| 項目 | 說明 | 控制項建議 |
|------|------|------------|
| 生產訂單 | 標籤「生產訂單號:」+ 顯示／單行輸入 | Label + TextBlock/TextBox |
| 受訂量 | 標籤「受訂量:」+ 顯示／單行輸入，預設 0 | Label + TextBlock/TextBox （綁定數字） |
| 楞別 | 標籤「楞別」+ 選項（A, AB, B, BC, C, E、其他等）；與 Orders 欄位、右側 DataGrid 一致（若 UI 顯示「類別」則為同一欄位） | Label + ComboBox |
| 箱型 | 標籤「箱型」+ 選項（E / S，如 RadioButton） | Label + RadioButton（E型、S最佳化型） |
| 客戶名稱 | 標籤「客戶名稱」+ 單行輸入框 | Label + TextBox |
| 備註 | 標籤「備註」+ 多行文字輸入框 | Label + TextBox（AcceptReturn, TextWrapping） |

- **狀態**：依流程 **可編輯** 或 **反灰**（見 3.0、3.1）。

#### 4.4. 右側數據顯示區
| 項目 | 說明 | 控制項建議 |
|------|------|------------|
| 版號列表 | DataGrid，資料來源為 Orders，**不分頁、捲軸**；版號/版號查詢找到時定位選取並捲動至中央；雙擊列帶出至左側與 4.5；手動點選列不觸發捲動 | DataGrid，可排序、捲軸 |
| 欄位 | 版號、客戶名稱、箱型、楞別、建檔日期（含時間）；欄位標題 RESX 多語系 | DataGrid 欄位定義 |

#### 4.5. 下方箱型預覽區與相位（分左、中、右三個區塊）

| 區塊 | 項目 | 說明 | 控制項建議 |
|------|------|------|------------|
| **左側** | 箱型圖形預覽區 | 棕色矩形區域，箱型尺寸／佈局預覽（置中、佔左側區塊寬度約 1/2）；矩形**右邊線**上為寬度顯示／短輸入框，**下邊線**上為長度顯示／短輸入框 | Canvas 或 Border + 自繪／Shape（Rectangle、Line）+ TextBox |
| **中間** | 相位（標題） | 標籤「相位」 | TextBlock/Label |
| **中間** | 1色相位 | 標籤 + 短輸入框 | Label + TextBox |
| **中間** | 2色相位 | 標籤 + 短輸入框 | Label + TextBox |
| **中間** | 3色相位 | 標籤 + 短輸入框 | Label + TextBox |
| **右側** | 楞別圖形說明區 | 楞別示意圖（type_of_cardboard.png） | Image |

- **版面**：左（預覽矩形）｜中（相位標題 + 1／2／3 色相位輸入）｜右（楞別圖示）；依流程 **可編輯** 或 **反灰**（見 3.0、3.1）。區塊內輸入欄位同 4.3：預設白底黑字，可輸入狀態（焦點）黃底。

**4.5 規格補充**
- **區塊寬度比例**：左 : 中 : 右 ＝ **5 : 2 : 3**（ColumnDefinitions 使用 `5*`、`2*`、`3*`）；三區塊以 **Grid** 排版，必要時設定 MinWidth；區塊內不再細分。
- **左側區塊**：箱型圖形預覽區之**預覽矩形**於左側區塊內**置中**，寬度佔左側區塊約 **1/2**（或固定最大寬度，如 400px），長寬由 ViewModel 綁定長度／寬度數值依比例繪製；右邊線、下邊線之寬度／長度 TextBox 緊貼矩形邊緣。
- **中間區塊**：相位標題在上，1／2／3 色相位由上而下排列（各一列 Label + TextBox），垂直堆疊、對齊。
- **右側區塊**：楞別圖形說明區以 **Image** 顯示 type_of_cardboard.png，**fit 分配後之右側區塊大小**（Stretch 填滿區塊、維持比例）。
- **4.5 區域高度**：此區塊整體高度為**頁面視窗的 3/8**（Row 比例 3*，與主內容區 5* 合計 8 等分）。
- **響應式**：視窗縮放時，三區塊可依比例縮放；左側預覽矩形維持「佔左側 1/2 寬、置中」，避免超出區塊邊界。

#### 4.6. 底部功能提示
| 項目 | 位置 | 說明 | 控制項建議 |
|------|------|------|------------|
| F4 刪除 | 最左 | 圖示 + 文字「F4 刪除」；依流程可點擊或反灰（見 3.0、3.1） | Button + Image（icons8-delete-96.png） |
| F5 完成編輯訂單／F5 新增訂單 | 左 | 依流程切換文字與啟用狀態（見 3.1）；附圖示（icons8-edit-80.png / icons8-add-96.png） | Button |
| F2 加入排程／離開 | 左 | 圖示 + 文字「F2 加入排程/離開」（見 3.4） | Button + Image（icons8-submit-document-96.png） |
| F1 離開 | **最右** | 圖示 + 文字「F1 離開」；始終可點擊；**靠右對齊** | Button + Image（icons8-exit-96.png） |

- **狀態**：F4／F5／F2 依流程 **可點擊** 或 **反灰**（見 3.0、3.1）；F1 始終可點擊。

### 5. 圖示與 Resources/Icons 對應
| 用途 | 規格表建議檔名 |
|------|----------------|
| F6 搜尋 | icons8-search-in-list-96.png |
| 上箭頭 | icons8-collapse-arrow-96.png |
| 下箭頭 | icons8-expand-arrow-96.png |
| 預覽（與 4.1 F6 搜尋為不同按鈕） | icons8-package-search-96.png |
| 送入版號位置 | icons8-insert-96.png |
| F4 刪除 | icons8-delete-96.png |
| F5 編輯／新增 | icons8-edit-80.png（完成編輯）/ icons8-add-96.png（新增訂單） |
| F2 加入排程／離開 | icons8-submit-document-96.png |
| F1 離開 | icons8-exit-96.png |
| 楞別圖形說明區 | type_of_cardboard.png |

- 實作時以 **目錄現有檔** 為準；若日後補齊規格表檔名，可改回指定檔。

### 7. 整體版面結構摘要
- **頂部左側**：版號（搜尋 Orders）、F6 搜尋。
- **頂部右側**：版號查詢（查 DataGrid 選取）、上／下箭頭、預覽、送入版號位置。
- **左側**：生產訂單、受訂量、楞別、箱型、客戶名稱、備註；**依流程可編輯或反灰**。
- **右側**：版號列表 DataGrid（Orders，可排序、分頁每頁十筆）。
- **下側（4.5，左中右三區塊）**：左＝箱型圖形預覽區（長度／寬度）、中＝相位（1／2／3 色）、右＝楞別圖形說明區（type_of_cardboard.png）；**依流程可編輯或反灰**。
- **底部**：F4 刪除、F5 完成編輯訂單／F5 新增訂單、F2 加入排程／離開、F1 離開（依流程切換按鈕啟用狀態，見 3.0、3.1）。

### 8.x 待實作項目（訂單製作）

#### 資料表與種子
- [x] 建立 **Orders** 表與建表腳本（`Scripts/CreateOrdersTable.sql`），欄位：建立日期時間、生產訂單號、版號、受訂量、箱型、楞別、相位1、相位2、相位3、長度、寬度、客戶名稱、備註。
- [x] 實作種子：隨機產生 100 筆寫入 Orders（規則見 2.2；於 DatabaseService.SeedOrdersIfEmpty）。

#### 結構與導航
- [x] 新增「訂單製作」Dialog/Window，由 Layout2 **F7 訂單製作** 開啟。
- [x] 標題、標籤與按鈕文字使用 RESX 多語系（例：「訂單製作」、「F5 完成編輯訂單」、「F5 新增訂單」、「F4 刪除」、「F2 加入排程/離開」、「F1 離開」）。

#### 流程與狀態
- [x] **3.0 初始**：開啟時左側輸入區、下側箱型預覽區（含相位）反灰；F4 刪除、F5 完成編輯訂單／F5 新增訂單、F2 加入排程／離開 皆反灰；F1 離開始終可點擊。
- [x] **3.1 搜尋**：頂部左側輸入版號後 Enter 或 F6 搜尋 → 查詢 **Orders**。
  - [x] 有資料：帶出至左側與下側預覽區（含相位），**並定位到右側 DataGrid 該筆、選取並捲動至中央**；解除反灰；F4、F5、F2 解除反灰；F5 完成編輯訂單儲存後顯示「此筆訂單:版號已儲存」Dialog。
  - [x] 無資料：顯示 Dialog「是否新增此筆新的版號？」（確認／取消）；確認後左側與下側解除反灰、F5 改為「F5 新增訂單」並解除反灰；F5 新增訂單儲存後 F5 文字改回「F5 完成編輯訂單」、F4 解除反灰。
- [x] **3.2 版號查詢**：頂部右側「版號查詢」輸入版號 + Enter → 查詢右側 DataGrid、選取對應列並捲動至中央；上／下箭頭為**同版號**上一筆/下一筆、不循環。
- [x] **3.3 預覽**：預覽按鈕或**雙擊 DataGrid 列**將選取列帶出至左側與下側。
- [x] **3.4 F2 加入排程／離開**：將目前編輯筆加入 Layout2 上區生產排程（與 Phase 5.2 生產訂單列表同一排程來源），並離開訂單製作頁面。

#### 頂部操作區
- [x] **左側**：版號輸入／顯示（綠底）、F6 搜尋（圖示 icons8-search-in-list-96.png）；按鈕與標籤 RESX。
- [x] **右側**：版號查詢欄位（灰底）、上／下箭頭、預覽、送入版號位置；RESX。

#### 左側輸入區
- [x] 表單：生產訂單、受訂量、楞別（ComboBox）、箱型（E／S RadioButton）、客戶名稱、備註（多行）；綁定 ViewModel，與 Orders 讀寫連動。
- [x] 依流程控制 **IsEnabled／反灰**（見 3.0、3.1）。

#### 右側數據顯示區
- [x] DataGrid：資料來源 Orders，**不分頁、捲軸**；欄位版號、客戶名稱、箱型、楞別、建檔日期（含時間 `yyyy/MM/dd HH:mm`）；欄位標題 RESX 多語系；版號/版號查詢找到時定位選取並捲動至中央；雙擊列帶出至左側與 4.5；手動點選列不觸發捲動（RequestScrollToSelected 僅程式觸發）。
- [x] 與 **IOrderRepository** 對接（GetAll、查詢、選取列）；上/下箭頭為同版號上一筆/下一筆、不循環。

#### 下側箱型預覽區與相位（4.5，左中右三區塊）
- [x] 三區塊寬度比例 **左 : 中 : 右 ＝ 5 : 1 : 4**，使用 **Grid**（ColumnDefinitions 5*、1*、4*）；區塊內不再細分；必要時 MinWidth。
- [x] **左側區塊**：箱型圖形預覽區（棕色矩形、長度／寬度綁定 ViewModel）。
- [x] **中間區塊**：相位標題 + 1色、2色、3色相位 **標籤與短輸入框上下排列**（垂直堆疊），綁定 ViewModel；1色/2色/3色 RESX 多語系。
- [x] **右側區塊**：楞別圖形說明區（type_of_cardboard.png，Stretch Fill）。
- [x] 依流程控制 **IsEnabled／反灰**（見 3.0、3.1）。

#### 底部與快捷鍵
- [x] F4 刪除、F5 完成編輯訂單／F5 新增訂單、F2 加入排程／離開、F1 離開；圖示與 RESX 見本 Phase 6「4. Layout 配置與控制項」之「4.6. 底部功能提示」。
- [x] 鍵盤 Enter（頂部版號）觸發 F6 搜尋、Enter（版號查詢）觸發 DataGrid 選取。

#### 資料與模型
- [x] 訂單模型 **Order**（對應 Orders：建立日期時間、生產訂單號、版號、受訂量、箱型、楞別、相位1～3、長度、寬度、客戶名稱、備註）。
- [x] **IOrderRepository**：依版號查詢、新增、更新、刪除 Orders；**GetAll()** 供 4.4 不分頁捲軸列表；GetPage 保留。

### 9. 可能新增的檔案與目錄（Phase 6）
```
src/
├── Scripts/
│   ├── CreateOrdersTable.sql        # Orders 建表（欄位見 2.1）
│   └── SeedOrders.sql               # (必做) 種子 100 筆
├── Views/
│   └── Layout2/
│       └── OrderMakingView.xaml(.cs)     # 訂單製作主內容區頁面（頂部左右、左側輸入、右側 DataGrid、下側預覽與相位、底部 F4/F5/F2/F1）
├── ViewModels/
│   └── OrderMakingDialogViewModel.cs     # 流程 3.0～3.4、反灰／解除反灰、F5 雙態、與 IOrderRepository 及 Layout2 生產排程（同 Phase 5.2 排程來源）整合
├── Models/
│   └── Order.cs                     # 對應 Orders 表（建立日期時間、生產訂單號、版號、受訂量、箱型、楞別、相位1～3、長度、寬度、客戶名稱、備註）
└── Services/
    ├── IOrderRepository.cs          # 依版號查詢、新增、更新、刪除；分頁列表（每頁十筆）
    └── OrderRepository.cs           # SQLite 實作
```
- **資源**：圖示檔（icons8-search-in-list-96.png、icons8-edit-80.png、icons8-add-96.png、icons8-delete-96.png、icons8-submit-document-96.png、icons8-exit-96.png、icons8-collapse-arrow-96.png、icons8-expand-arrow-96.png、icons8-package-search-96.png）及 4.5 右側區塊楞別示意圖（**type_of_cardboard.png**）置於專案 Resources 或 Assets。
- **整合**：F2 加入排程／離開 需與 **Layout2 上區生產排程（DataGrid）**（與 Phase 5.2 生產訂單列表同一排程來源）對接，將目前訂單寫入排程並關閉訂單製作頁面。

---

## Phase 8：整合與收尾

### 8.1 綁定與流程
- [x] 將 `LoginView` 設為啟動畫面（ShellWindow 啟動後 `NavigateToLogin()` 顯示 LoginView）
- [x] 確認 DataContext 正確綁定至 `LoginViewModel`（ShellWindow.CreateLoginView 設定）
- [x] 測試流程：輸入密碼 → Enter → 比對資料庫 → 成功跳轉
- [x] 測試 Cancel 清除密碼

### 8.2 錯誤處理
- [x] 資料庫連線失敗處理（App.OnStartup  try/catch，MessageBox 後 Shutdown）
- [x] 密碼錯誤提示（LoginViewModel.LoginErrorMessage + RESX，LoginView 顯示）
- [x] 輸入為空時的提示（RESX `LoginErrorEmptyPassword`，登入時檢查並顯示）
- [x] 未處理例外（App.DispatcherUnhandledException，完整內容寫入與執行檔同目錄的 `error_yyyy-MM-dd_HH-mm-ss.log`，MessageBox 僅提示已寫入之檔名）
- [x] 登入後導航失敗（NavigateToLayout2 外層 try/catch，失敗時 MessageBox + LoginErrorMessage，見 `QandA/crashAfterLoginOnOtherPC.md`）
- [x] 資料庫預設路徑改為 `%LocalAppData%\CursorTestApp\app.db`（單檔發佈在另一台電腦執行時避免 BaseDirectory 唯讀導致 crash）

### 8.3 Log 服務整合
- [x] 確保 `ILogService` 於 App 啟動時初始化（ShellWindow 建構時建立並注入）
- [x] 所有 View 共享同一 Log 實例（單例或 DI）（同一 _logService 傳入 LoginViewModel / MainViewModel）
- [x] Log 格式建議：`[HH:mm:ss] 訊息內容`（LogService 已實作）
- [x] LogListBox「ItemsControl 與其項目來源不一致」異機修正：LogService.Append 一律 `InvokeAsync(Loaded)`、LogListBox 關閉虛擬化、ScrollIntoView 延後至 Loaded；詳見 `LessonLearn/whyItemSourceNotConsistant.md`

### 8.4 程式品質
- [x] 移除不必要的 `Console.WriteLine`，改寫入 Log（專案內無 Console.WriteLine）
- [x] 密碼以明文儲存於 SQLite（本專案為測試用）
- [x] 基本程式碼整理與註解

### 8.5 多國語系與字型
- [x] Login 畫面字串使用 RESX 綁定（`LocalizedString` 實現動態切換）
- [x] 其餘 UI 字串使用 RESX 綁定（MainView 主畫面標題、返回 ToolTip；登入錯誤訊息 RESX）
- [x] 預設字型設為 `Segoe UI, Meiryo UI, Microsoft JhengHei, Leelawadee UI`（App.xaml）
- [x] 驗證五種語系（ja, zh-TW, pt, en, th）顯示正常（需手動驗證）

### 8.6 應用程式圖示
- [x] 使用 `Resources/Icons/icons8-app-96.png` 作為應用程式圖示來源
- [x] 設定視窗 Icon（pack URI 綁定 PNG）
- [x] 設定發佈後 exe 圖示（Resources/Icons/icons8-app-96.ico）

### 8.7 發佈與 Distribution 套件
- [ ] 執行 `dotnet publish -c Release`
- [ ] 撰寫安裝指引，列出使用者須安裝的套件（.NET 8 Desktop Runtime、VC++ Redistributable）
- **建議**：Demo／測試優先使用**單一執行檔**（self-contained + PublishSingleFile），見 `QandA/howToPackApplication.md`；正式產品再考慮安裝程式（Inno Setup、WiX、MSIX）。
- [ ] 使用 `scripts\publish.ps1` 產生單一 exe（可選）

### 8.8 Windows 8.1 模擬測試
- [ ] 依上方「Windows 8.1 模擬測試」章節進行 VM 測試
- [ ] 驗證在 Win 8.1 上安裝必要 Distribution 套件後可正常執行

### 8.9 SQLite 資料庫檢視小工具
- [x] 使用 **Microsoft.Data.Sqlite** 撰寫小工具，可檢視資料庫內的資料
- [x] 指定 `.db` 檔路徑後，瀏覽其表與資料（列出所有表、選取表後顯示內容）
- [x] 程式碼放置於 `tools/` 資料夾

---

## 快速檢查清單

| 項目 | 狀態 |
|------|------|
| .NET 8 SDK 安裝 | ⬜ |
| 使用者端 Distribution 套件清單 | ⬜ |
| 預設字型（Win 8.1 相容） | ⬜ |
| 多國語系（ja, zh-TW, pt, en, th） | 🟨 結構完成，待 Phase 8.5 整合 |
| WPF-UI（Fluent Design）套件 | ✅ |
| SQLite 與資料表（明文密碼） | ✅ |
| 專案建立（Phase 1.1） | ✅ |
| MVVM 架構（Phase 1.2） | ✅ |
| 登入畫面 UI + 背景設計 | ✅ |
| 右下角 Log 區域 | ✅ |
| 登入邏輯與資料庫驗證 | ✅ |
| Cancel 清除密碼 | ✅ |
| 導航至主畫面 | ✅ |
| 登入畫面語系切換（Login 下方） | ✅ |
| 登入畫面右上角 X 離開按鈕 | ✅ |
| 應用程式圖示（icons8-app-96.png） | ✅ |
| Phase 5 排程管理子頁（排單管理 Dialog） | ✅ |
| Phase 6 訂單製作（F7，含查詢／編輯／新增） | ✅ |
| Windows 8.1 模擬測試（VM） | ⬜ |
| 端對端測試 | ⬜ |

---

## 建議執行順序

1. **Phase 0**：先完成環境與套件安裝  
2. **Phase 1**：建立專案與 MVVM 結構  
3. **Phase 2**：完成 SQLite 與驗證邏輯  
4. **Phase 3**：實作登入畫面與 ViewModel  
5. **Phase 4**：實作導航與主畫面（含 Layout2）  
6. **Phase 5**：排程管理子頁（Dialog）  
7. **Phase 6**：訂單製作（F7，單一 Layout 含查詢／編輯／新增、Orders 表）  
8. **Phase 8**：整合測試與收尾  

---

*建立日期：2025-02-21*
