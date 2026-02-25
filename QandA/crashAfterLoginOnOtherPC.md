# 在另一台電腦執行 exe：登入後程式 crash 的建議

## 可能原因

1. **資料庫路徑（最常見）**  
   單檔發佈時，`AppDomain.CurrentDomain.BaseDirectory` 會指向解壓暫存目錄。在另一台電腦上該目錄可能唯讀或權限不同，寫入 `app.db` 時失敗，或登入後讀寫排程表時拋出例外導致 crash。

2. **未捕捉的例外**  
   登入成功後會導航至 Layout2、建立排程服務並讀取 DB。任一環節拋出例外若未處理，會導致程式直接結束。

3. **缺少 VC++ Redist 或執行環境差異**  
   若為 framework-dependent 發佈，另一台未安裝 .NET 8 Desktop Runtime 會無法執行；若為 self-contained 單檔，則較少見。WPF-UI 等依賴的 VC++ 在部分環境也可能導致問題。

## 建議作法

### 1. 資料庫改到使用者可寫入目錄（建議實作）

將 `app.db` 預設放在 `%LocalAppData%\CursorTestApp\`（或專案名稱），不要放在 exe 所在目錄或 BaseDirectory：

- 單檔發佈時 BaseDirectory 為解壓暫存，可能唯讀。
- `Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)` 在每台電腦都是使用者可寫入。

專案已可改為：未傳入自訂路徑時，使用 `LocalApplicationData\CursorTestApp\app.db`。

### 2. 加上未處理例外處理，方便查錯

在 `App.xaml.cs` 的 `OnStartup` 中註冊：

- `Application.Current.DispatcherUnhandledException`  
  可將例外訊息寫入檔案或顯示 MessageBox，再決定是否結束程式。  
  這樣在另一台電腦 crash 時，至少能看到錯誤內容（例如路徑無效、權限不足、找不到 DLL 等）。

### 3. 登入後導航時加 try-catch

在 `LoginViewModel.Login()` 中，`NavigateToLayout2()` 前後用 try-catch 包住，失敗時：

- 寫入 Log（若已有 Log 機制），並  
- 以 MessageBox 顯示錯誤訊息，避免直接 crash。

可一併記錄 `ex.ToString()` 方便事後查原因。

### 4. 確認另一台電腦環境

- 若為 **self-contained 單檔**：理論上不需安裝 .NET；若仍 crash，用上述 2、3 取得錯誤訊息。
- 若為 **framework-dependent**：另一台須已安裝 [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) 與 [VC++ Redistributable](https://aka.ms/vs/17/release/vc_redist.x64.exe)。

### 5. 取得 crash 時的錯誤訊息

- 在開發機用 2、3 加上例外處理並重建 exe，再到另一台執行；crash 時會彈出或寫入錯誤內容。
- 或於另一台在「命令提示字元」執行 exe，有時會印出例外到主控台。

---

## 小結

優先建議：**將資料庫預設路徑改為 `LocalApplicationData\CursorTestApp\app.db`**，並**加上 DispatcherUnhandledException 與登入後導航的 try-catch**，再重新產生單一執行檔到另一台測試；若仍 crash，依錯誤訊息再排查。
