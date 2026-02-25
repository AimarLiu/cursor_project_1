# 如何打包／發布程式：安裝程式 vs 單一執行檔

## 一、兩種發布方式比較

| 項目 | **安裝程式（Installer）** | **單一執行檔（Single-file）** |
|------|---------------------------|--------------------------------|
| 使用者體驗 | 執行 setup → 下一步 → 完成，有「解除安裝」、開始功能表捷徑 | 一個 .exe，複製即用，可放隨身碟 |
| 檔案大小 | 較小（若採依賴 .NET 的安裝包）；若安裝包內含 runtime 則較大 | 單檔約 60–100MB+（self-contained 含 .NET runtime） |
| 前置需求 | 可由安裝程式一併安裝 .NET 8 Desktop Runtime、VC++ Redist | 若用 self-contained，**不需**使用者另裝 .NET |
| 製作難度 | 需另做安裝包（如 Inno Setup、WiX、MSIX） | `dotnet publish` 即可，較簡單 |
| 適用情境 | 正式產品、要給一般使用者、希望有「安裝/解除安裝」 | 內部/Demo、攜帶、不想碰安裝流程 |

---

## 二、建議

- **若用途是「客戶 Demo／測試」**：  
  **建議先用「單一執行檔」**  
  - 一個 .exe 就能跑，不用先裝 .NET、不用跑安裝程式，對方接受度通常較高。  
  - 可選 **self-contained + 單檔**，例如：  
    `dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true`

- **若之後要當正式產品給 end user**：  
  **再考慮改成「安裝程式」**  
  - 可一併安裝 .NET 8 Desktop Runtime、VC++ Redist。  
  - 有開始功能表、解除安裝、必要時可註冊預設開啟方式等，較像一般 Windows 軟體。

---

## 三、單一執行檔發布範例

```powershell
# 從專案目錄（src）執行
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

- 輸出目錄：`src\bin\Release\net8.0-windows\win-x64\publish\CursorTestApp.exe`
- 若目標還有 32 位元或 arm，可再加 `-r win-x86` 等分別建一次。

---

## 四、使用本專案腳本

專案提供 `scripts\publish.ps1`，可直接產生單一執行檔：

```powershell
.\scripts\publish.ps1
```

執行後 exe 位於 `src\bin\Release\net8.0-windows\win-x64\publish\CursorTestApp.exe`。
