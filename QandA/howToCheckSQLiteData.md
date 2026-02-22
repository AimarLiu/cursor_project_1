# 如何手動確認 SQLite 有資料

本專案資料庫為 **`app.db`**，主要表為 **`Users`**（Id, Username, Password）。以下為幾種手動確認方式。

---

## 1. 找到 app.db 的位置

- **Visual Studio / Rider 執行**：`app.db` 會建立在專案輸出目錄  
  例如：`c:\Users\aimar\cursor_test\src\bin\Debug\net8.0-windows\app.db`
- **dotnet run**：同上，位於 `bin\Debug\net8.0-windows\` 下  
  （需至少執行過一次程式，資料庫才會建立）

---

## 2. 使用 sqlite3 指令列工具

若已安裝 sqlite3 CLI（例如透過 [sqlite.org](https://sqlite.org/download.html) 或 Chocolatey）：

```bash
# 切換到 app.db 所在目錄後執行
sqlite3 app.db "SELECT * FROM Users;"
```

或進入互動模式：

```bash
sqlite3 app.db
```

進入後執行：

```sql
.tables
SELECT * FROM Users;
.quit
```

---

## 3. 使用 DB Browser for SQLite（圖形介面）

1. 下載安裝：[DB Browser for SQLite](https://sqlitebrowser.org/)
2. 開啟程式 → Open Database → 選擇 `app.db`
3. 切到 **Browse Data** 分頁，選擇 `Users` 表即可檢視  
   （預設種子：Id=0001, Username=aimarliu, Password=aimarliu）

---

## 4. 使用 .NET 快速查詢（無須額外安裝）

專案已引用 `Microsoft.Data.Sqlite`，可在程式中或撰寫小工具查詢。路徑需指向實際的 `app.db`：

```csharp
using Microsoft.Data.Sqlite;

// 路徑需對應實際 app.db 位置（如 bin/Debug/net8.0-windows/app.db）
using var conn = new SqliteConnection("Data Source=src/bin/Debug/net8.0-windows/app.db");
conn.Open();
using var cmd = conn.CreateCommand();
cmd.CommandText = "SELECT * FROM Users";
using var r = cmd.ExecuteReader();
while (r.Read())
{
    Console.WriteLine($"{r["Id"]} | {r["Username"]} | {r["Password"]}");
}
```

---

## 快速對照

| 方式 | 需要 | 適合 |
|------|------|------|
| **sqlite3** | 安裝 sqlite3 CLI | 指令列、腳本 |
| **DB Browser** | 安裝 DB Browser | 圖形介面、手動查改 |
| **.NET + Sqlite** | 無（專案已有） | 程式中或小工具查詢 |
