# tools

## 小工具結構（tools 資料夾）

```
tools/
└── SqliteViewer/
    ├── SqliteViewer.csproj
    └── Program.cs
```

- 使用 **Microsoft.Data.Sqlite** 讀取 SQLite 資料庫
- 指定 `.db` 檔路徑後，可瀏覽所有表（排除 `sqlite_*` 系統表）與各表內容

---

## 使用方式

```powershell
# 指定 .db 路徑執行
dotnet run -- "C:\path\to\app.db"

# 或先建置再執行
cd tools\SqliteViewer
dotnet build
dotnet run -- src\bin\Debug\net8.0-windows\app.db
```

若未提供路徑參數，程式會提示輸入 `.db` 檔案路徑。

---

## 解決方案

`SqliteViewer` 專案已納入 `CursorTestApp.slnx` 的 `tools/` 資料夾下，與主專案一同建置。
