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
- [ ] 安裝 **.NET 8 SDK**
  - 下載：https://dotnet.microsoft.com/download/dotnet/8.0
  - 驗證：`dotnet --version`
- [ ] 安裝 **Visual Studio 2022**
  - 選「.NET 桌面開發」
  - 勾選「Windows 桌面應用程式」工作負載
- [ ] （可選）安裝 **Visual Studio Code** + C# Dev Kit

### 0.2 發佈設定（Framework-dependent）
- [ ] 發佈指令：`dotnet publish -c Release`
- [ ] 輸出位於 `bin/Release/net8.0-windows/publish/`
- [ ] 使用者端須已安裝 **.NET 8 Desktop Runtime**（見上方 Distribution 套件）

### 0.3 Fluent Design 套件
- [ ] 安裝 **WPF-UI**
  - NuGet：`dotnet add package WPF-UI`
  - 官網：https://github.com/lepoco/wpfui
  - 視覺降級可接受（Acrylic 等在 Win 8.1 可能退化為純色）
- [ ] （可選）**FluentIcons.Wpf**
  - NuGet：`dotnet add package FluentIcons.Wpf`
- [ ] 在 `App.xaml` 中引入 WPF-UI 資源字典與主題

### 0.4 資料庫與其他套件
- [ ] 安裝 **Microsoft.Data.Sqlite**
  - NuGet：`dotnet add package Microsoft.Data.Sqlite`
- [ ] （可選）**CommunityToolkit.Mvvm**
  - NuGet：`dotnet add package CommunityToolkit.Mvvm`

---

## Phase 1：專案建立與架構

### 1.1 建立專案
- [ ] 建立 WPF 專案：`dotnet new wpf -n CursorTestApp -o src`
- [ ] 設定目標框架：`<TargetFramework>net8.0-windows</TargetFramework>`
- [ ] 建立解決方案結構
  ```
  CursorTestApp.sln
  src/
    CursorTestApp/           # 主專案（UI、Views、ViewModels）
      Properties/
      Resources/             # RESX 語系檔
  ```

### 1.2 MVVM 架構
- [ ] 建立 ViewModel 基底（`ViewModelBase`）與 `INotifyPropertyChanged`
- [ ] 考慮使用 **CommunityToolkit.Mvvm** 或手寫 ViewModel
- [ ] 定義專案資料夾：`Views`、`ViewModels`、`Models`、`Services`、`Helpers`

### 1.3 多國語系結構
- [ ] 建立 `Resources.resx`（預設語系，建議英文）
- [ ] 建立 `Resources.ja.resx`（日語）、`Resources.zh-TW.resx`（繁中）、`Resources.pt.resx`（葡）、`Resources.th.resx`（泰）
- [ ] 實作 `ILocalizationService`，支援 `CultureInfo` 切換
- [ ] 字串一律使用 `Resources.XXX`，勿寫死在 XAML / Code

---

## Phase 2：資料庫

### 2.1 SQLite 設定
- [ ] 設計使用者表 `Users`：Id, Username, Password（**明文**即可，本專案為測試用）
- [ ] 實作 `IDatabaseService` / `IAuthService` 介面
- [ ] 實作密碼比對邏輯（直接字串比對）
- [ ] 提供初始化與建表腳本

### 2.2 初始資料
- [ ] 建立至少一筆測試帳號供登入驗證

---

## Phase 3：登入畫面（Login View）

### 3.1 整體佈局
- [ ] 建立 `LoginView.xaml`，使用 WPF-UI 的 `ui:UiWindow` 或 `ui:FluentWindow`
- [ ] 視窗符合螢幕大小：`WindowState="Maximized"`
- [ ] **右下角 Log 區域**（所有頁面共用）：
  - [ ] 固定於視窗右下角，約 25% 寬 × 15% 高（可調整）
  - [ ] 使用 `TextBox` 或 `RichTextBox`，設為 `IsReadOnly`，`ScrollViewer.VerticalScrollBarVisibility="Auto"`
  - [ ] 半透明深色背景、淺色字、等寬字型 **Courier New**（Win 8.1 內建）
  - [ ] 實作 `ILogService` / `ILogOutput`，透過 ViewModel 綁定 `LogMessages` 集合
  - [ ] 所有關鍵操作（登入嘗試、驗證結果、導航等）寫入 Log

### 3.2 中央登入區塊
- [ ] 使用 WPF-UI 的 `ui:TextBox`、`ui:PasswordBox`、`ui:Button` 達成 Fluent 風格
- [ ] 畫面正中央包含：
  - [ ] 密碼輸入（`PasswordBox`）
  - [ ] Enter 按鈕
  - [ ] Cancel 按鈕（清除密碼）
- [ ] 視覺排版：垂直堆疊、居中、適當間距

### 3.3 背景設計
- [ ] **主背景**：深色漸層（例如 #1a1a2e → #16213e → #0f3460）
- [ ] **裝飾層**：半透明圓形或柔和形狀（Blur + Opacity）
  - 左上角大圓、右下角中圓，製造景深
- [ ] 或採用 **WPF-UI Acrylic**（若支援）：毛玻璃效果（Win 8.1 上可能降級為純色）
- [ ] **中央卡片**：登入區塊外層加圓角矩形、半透明白/深色背景，與背景形成層次

### 3.4 ViewModel
- [ ] `LoginViewModel`
  - [ ] `Password`（綁定至 PasswordBox）
  - [ ] `LoginCommand`（Enter）
  - [ ] `CancelCommand`（清除密碼）
  - [ ] 注入 `ILogService`，記錄「正在驗證…」「登入成功/失敗」
  - [ ] 登入成功時觸發導航；失敗時寫入 Log 並可選顯示 Snackbar

### 3.5 Cancel 功能
- [ ] Cancel 按鈕執行後清空密碼輸入欄位，並於 Log 輸出「已清除輸入」

---

## Phase 4：主畫面與導航

### 4.1 空白主畫面
- [ ] 建立 `MainView.xaml`（登入成功後的空白頁面）
- [ ] 沿用與登入頁相同的背景風格，保持一致性
- [ ] **右下角 Log 區域**：與登入頁共用同一區塊，持續顯示 Log
  - [ ] 建議將 Log 區抽出為 `UserControl`（如 `LogPanel.xaml`），於各 View 中重複使用
- [ ] 中央可放簡單標題或歡迎文字

### 4.2 導航機制
- [ ] 實作導航服務（`INavigationService`）
  - [ ] `NavigateTo<TView>()` 或 `NavigateTo(string viewName)`
- [ ] 使用 `Frame`、`ContentControl` + DataTemplate，或 `Window` 切換
- [ ] 登入成功 → 寫入 Log「導航至主畫面」→ 切換至 MainView
- [ ] 決定是否可返回登入頁（例如登出後）

---

## Phase 5：整合與收尾

### 5.1 綁定與流程
- [ ] 將 `LoginView` 設為啟動畫面
- [ ] 確認 DataContext 正確綁定至 `LoginViewModel`
- [ ] 測試流程：輸入密碼 → Enter → 比對資料庫 → 成功跳轉
- [ ] 測試 Cancel 清除密碼

### 5.2 錯誤處理
- [ ] 資料庫連線失敗處理
- [ ] 密碼錯誤提示
- [ ] 輸入為空時的提示（可選）

### 5.3 Log 服務整合
- [ ] 確保 `ILogService` 於 App 啟動時初始化
- [ ] 所有 View 共享同一 Log 實例（單例或 DI）
- [ ] Log 格式建議：`[HH:mm:ss] 訊息內容`

### 5.4 程式品質
- [ ] 移除不必要的 `Console.WriteLine`，改寫入 Log
- [ ] 密碼以明文儲存於 SQLite（本專案為測試用）
- [ ] 基本程式碼整理與註解

### 5.5 多國語系與字型
- [ ] 所有 UI 字串使用 RESX 綁定
- [ ] 預設字型設為 `Segoe UI, Meiryo UI, Microsoft JhengHei, Leelawadee UI`
- [ ] 驗證五種語系（ja, zh-TW, pt, en, th）顯示正常

### 5.6 發佈與 Distribution 套件
- [ ] 執行 `dotnet publish -c Release`
- [ ] 撰寫安裝指引，列出使用者須安裝的套件（.NET 8 Desktop Runtime、VC++ Redistributable）

### 5.7 Windows 8.1 模擬測試
- [ ] 依上方「Windows 8.1 模擬測試」章節進行 VM 測試
- [ ] 驗證在 Win 8.1 上安裝必要 Distribution 套件後可正常執行

---

## 快速檢查清單

| 項目 | 狀態 |
|------|------|
| .NET 8 SDK 安裝 | ⬜ |
| 使用者端 Distribution 套件清單 | ⬜ |
| 預設字型（Win 8.1 相容） | ⬜ |
| 多國語系（ja, zh-TW, pt, en, th） | ⬜ |
| WPF-UI（Fluent Design）套件 | ⬜ |
| SQLite 與資料表（明文密碼） | ⬜ |
| 專案建立與 MVVM 架構 | ⬜ |
| 登入畫面 UI + 背景設計 | ⬜ |
| 右下角 Log 區域 | ⬜ |
| 登入邏輯與資料庫驗證 | ⬜ |
| Cancel 清除密碼 | ⬜ |
| 導航至主畫面 | ⬜ |
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
