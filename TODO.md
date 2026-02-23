# Windows 桌面應用程式開發 - Todo List

## 應用程式規格
- **用途**：測試 Cursor 實際使用效果
- **技術棧**：C#, **.NET 8 (LTS)**, WPF, WPF-UI（Fluent Design）, MVVM, SQLite
- **目標平台**：**Windows 8.1** 及以上（.NET 8 官方支援 Windows 10 1607+；Win 8.1 需額外驗證）
- **部署方式**：**Framework-dependent**（需使用者安裝 .NET 8 Desktop Runtime）
- **功能**：登入驗證 → 正確則跳轉空白頁面
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

### 左側頂部狀態列
| 區塊 | 項目 | 說明 | 可用控制項或圖示 | 功能模擬 |
|------|------|------|------|------|
| 左側頂部 | 標題 | 生產資訊看板 | 預設 | 無 |
| 左側頂部 | 生產運作狀態 | "OPT"字樣，附LED圖示(紅燈與白燈) | Ellipse + Trigger | 收到RS485傳來的資料時會恆亮紅燈 ex: "ACTIVE+" |
| 左側頂部 | PLC訊號狀態 | "PLC"字樣，附LED圖示(綠燈與白燈) | Ellipse + Trigger | 有收到RS485訊號時綠燈閃爍 |
| 左側頂部 | 圖示 | png 圖示 | icons8-good-quality-80.png | 無 |

### 右側頂部訂單管理導航列
| 區塊 | 項目 | 說明 | 可用控制項或圖示 | 功能模擬 |
|------|------|------|------|------|
| 右側頂部 | 標題 | "生產排程"字樣，附 LED 圖示（綠燈與紅燈） | Ellipse + Trigger | 按下底部「排程管理」進入「模擬生產狀態」時紅燈，待機時綠燈 |
| 右側頂部 | F2 生產完成 | 按鈕，附圖示 | Button，icons8-production-finished-80.png | 將生產排程 DataGrid 所選項目移入生產完成訂單 DataGrid，並自排程移除；完成訂單多「日期時間」欄，為插入當下時間 |
| 右側頂部 | F4 預覽 | 按鈕，附圖示 | Button，icons8-search-property-80.png | 開啟 Dialog（確定/取消），輸入搜尋條件後於生產排程 DataGrid 搜尋並選取該筆 |
| 右側頂部 | F6 撤單 | 按鈕，附圖示 | Button，icons8-keep-clean-96.png | 刪除生產排程 DataGrid 所選項目，下方項目上移 |

### 左側面板：生產資訊看板
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

#### 中區：調單
| 位置 | 項目 | 說明 | 可用控制項或圖示 | 功能模擬 |
|------|-------|-------|-------|-------|
| 左側 | 標題 | "調單"字樣 | 預設 | 無 |
| 中間 | 訂單上移 | 上箭頭按鈕 | Button，icons8-arrowup-40.png | 所選項目與上一筆對調 |
| 右側 | 訂單下移 | 下箭頭按鈕 | Button，icons8-arrowdown-40.png | 所選項目與下一筆對調 |


#### 下區：生產完成訂單（DataGrid）
- **表格欄位**：日期時間、生產訂單號、版號、受訂量、箱型、類別、客戶名稱、備註
- **範例資料**：2026/01/10 13:01:12、0004、20260110、600、E、A、Aimar、(空)；2026/02/11 11:52:42、0005、20260110、199、E、A、Mason、(空)

### 底部控制列
| 位置 | 項目 | 說明 | 可用控制項或圖示 | 功能模擬 |
|------|------|------|------|------|
| 最左 | ⬅ 按鈕 | 返回登入頁 | 預設 | 呼叫 `INavigationService.NavigateToLogin` |
| 左 | F7 訂單製作 | 按鈕，附圖示 | Button，icons8-packing-96.png | 開啟新空白頁面（含右上 X 關閉） |
| 中左 | F1 排程管理 | 按鈕，附圖示 | Button，icons8-schedule-96.png | 進入「模擬生產狀態」：自生產排程 DataGrid 第一筆起依序執行至全部完成或手動停止；完成項移入生產完成訂單 DataGrid 並自排程移除，注意兩表排序 |
| 中右 | 異常狀態圖示 | 警示三角 | alarm.png / alarm.gif | 平時 alarm.png；RS485 錯誤時改為 alarm.gif（如 "ERROR+"） |
| 右 | F11 狀態顯示 | 按鈕，附圖示 | Button，icons8-warning-96.png | 開啟 Dialog 頁面，含右上方 X |
| 最右 | 關機 | 按鈕，附圖示 | Button，icons8-shutdown-96.png | 無 |

### 待實作項目（依 Layout2）

#### 結構與導航
- [ ] 新增 Layout2 主畫面 View，登入成功後導航至 Layout2（並存 Phase 4 主畫面）
- [ ] 擴充 `INavigationService`：`NavigateToLayout2`、必要時 `NavigateToOrderEdit` / `NavigateToStatusDialog`
- [ ] ShellWindow / ContentHost 可切換 Login / Main(Layout1) / Layout2

#### 資料與模型
- [ ] 定義生產排程項目模型（生產訂單號、版號、受訂量、箱型、類別、客戶名稱、備註）
- [ ] 定義生產完成訂單項目模型（含日期時間欄）
- [ ] 排程與完成訂單的集合（`ObservableCollection`）及選取項、調單（上移/下移）、F2/F6 等指令所需邏輯

#### UI 殼層與左側
- [ ] Layout2 主畫面：左窄右寬版面（Grid 或 DockPanel）
- [ ] 左側頂部狀態列：標題、OPT/PLC LED（Ellipse + 資料綁定或 Trigger）、圖示
- [ ] 左側生產資訊看板：生產訂單、生產版號、車速、目前產量、受訂量、預估完成所需時間（綁定至排程第一筆與 RS485/模擬資料）

#### 右側訂單面板
- [ ] 右側頂部：生產排程標題與 LED、F2 生產完成、F4 預覽、F6 撤單（含指令與圖示）
- [ ] 上區：生產排程 DataGrid（欄位、選取、F2/F4/F6/調單 操作）
- [ ] 中區：調單（訂單上移、訂單下移）按鈕與圖示
- [ ] 下區：生產完成訂單 DataGrid（含日期時間欄）

#### 底部與 Dialog
- [ ] 底部控制列：返回、F7 訂單製作、F1 排程管理、異常圖示、F11 狀態顯示、關機
- [ ] F7 訂單製作：新空白頁（含關閉 X）
- [ ] F4 預覽：搜尋 Dialog（確定/取消），搜尋生產排程並選取
- [ ] F11 狀態顯示：狀態 Dialog（含關閉 X）

#### 模擬生產與 RS485
- [ ] 「模擬生產狀態」邏輯：F1 啟動後依排程第一筆起執行、車速（模擬 400–500）、每 30 秒產量 +1、預估時間更新、完成項移入完成訂單
- [ ] RS485 服務（COM1: 9600,8,N,1）：Background 讀寫、OPT/PLC LED、車速、異常（alarm.gif）等資料來源
- [ ] Hosted Service 或 BackgroundWorker 整合 RS485 與 UI 更新

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

## Phase 5：整合與收尾

### 5.1 綁定與流程
- [x] 將 `LoginView` 設為啟動畫面（ShellWindow 啟動後 `NavigateToLogin()` 顯示 LoginView）
- [x] 確認 DataContext 正確綁定至 `LoginViewModel`（ShellWindow.CreateLoginView 設定）
- [x] 測試流程：輸入密碼 → Enter → 比對資料庫 → 成功跳轉
- [x] 測試 Cancel 清除密碼

### 5.2 錯誤處理
- [x] 資料庫連線失敗處理（App.OnStartup  try/catch，MessageBox 後 Shutdown）
- [x] 密碼錯誤提示（LoginViewModel.LoginErrorMessage + RESX，LoginView 顯示）
- [x] 輸入為空時的提示（RESX `LoginErrorEmptyPassword`，登入時檢查並顯示）

### 5.3 Log 服務整合
- [x] 確保 `ILogService` 於 App 啟動時初始化（ShellWindow 建構時建立並注入）
- [x] 所有 View 共享同一 Log 實例（單例或 DI）（同一 _logService 傳入 LoginViewModel / MainViewModel）
- [x] Log 格式建議：`[HH:mm:ss] 訊息內容`（LogService 已實作）

### 5.4 程式品質
- [x] 移除不必要的 `Console.WriteLine`，改寫入 Log（專案內無 Console.WriteLine）
- [x] 密碼以明文儲存於 SQLite（本專案為測試用）
- [x] 基本程式碼整理與註解

### 5.5 多國語系與字型
- [x] Login 畫面字串使用 RESX 綁定（`LocalizedString` 實現動態切換）
- [x] 其餘 UI 字串使用 RESX 綁定（MainView 主畫面標題、返回 ToolTip；登入錯誤訊息 RESX）
- [x] 預設字型設為 `Segoe UI, Meiryo UI, Microsoft JhengHei, Leelawadee UI`（App.xaml）
- [x] 驗證五種語系（ja, zh-TW, pt, en, th）顯示正常（需手動驗證）

### 5.6 應用程式圖示
- [x] 使用 `Resources/icons8-app-48.png` 作為應用程式圖示來源
- [x] 設定視窗 Icon（pack URI 綁定 PNG）
- [x] 設定發佈後 exe 圖示（建置前自動將 PNG 轉為 ICO）

### 5.7 發佈與 Distribution 套件
- [ ] 執行 `dotnet publish -c Release`
- [ ] 撰寫安裝指引，列出使用者須安裝的套件（.NET 8 Desktop Runtime、VC++ Redistributable）

### 5.8 Windows 8.1 模擬測試
- [ ] 依上方「Windows 8.1 模擬測試」章節進行 VM 測試
- [ ] 驗證在 Win 8.1 上安裝必要 Distribution 套件後可正常執行

### 5.9 SQLite 資料庫檢視小工具
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
| 多國語系（ja, zh-TW, pt, en, th） | 🟨 結構完成，待 Phase 5.5 整合 |
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
| 應用程式圖示（icons8-app-48.png） | ✅ |
| Windows 8.1 模擬測試（VM） | ⬜ |
| 端對端測試 | ⬜ |

---

## 建議執行順序

1. **Phase 0**：先完成環境與套件安裝  
2. **Phase 1**：建立專案與 MVVM 結構  
3. **Phase 2**：完成 SQLite 與驗證邏輯  
4. **Phase 3**：實作登入畫面與 ViewModel  
5. **Phase 4**：實作導航與主畫面  
6. **Phase 5**：整合測試與收尾  

---

*建立日期：2025-02-21*
