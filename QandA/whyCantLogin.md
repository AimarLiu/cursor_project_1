# 為什麼無法登入成功進入主頁面

以下為可能原因與排查方式。

---

## 可能原因

### 1. 密碼輸入錯誤

**測試帳號**：Username=aimarliu，**Password=aimarliu**

畫面只有密碼輸入框，需輸入 **`aimarliu`**（全小寫）。若輸入其他內容（例如使用者名稱、大小寫錯誤、空白），會驗證失敗。

---

### 2. Password 綁定未正確更新

`LoginView.xaml` 使用 `ui:PasswordBox` 綁定 `Text`：

```xml
<ui:PasswordBox Text="{Binding Password, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
```

若綁定未生效，ViewModel 的 `Password` 會一直是空字串，導致登入失敗。

**驗證方式**：在 `LoginViewModel.Login()` 開頭加一行：

```csharp
_logService.Append($"嘗試登入，密碼長度：{Password.Length}");
```

- 若 Log 顯示 `密碼長度：0`，代表綁定可能有問題
- 若顯示 `密碼長度：7`（aimarliu 為 7 字元），代表綁定正常

---

### 3. 資料庫未初始化或無種子資料

`App.OnStartup` 會呼叫 `DatabaseService.Initialize()`，若發生異常，可能未正確建表或插入測試帳號。

**驗證方式**：

1. 確認 `app.db` 存在：`src\bin\Debug\net8.0-windows\app.db`
2. 使用 DB Browser 或 sqlite3 查詢：

   ```sql
   SELECT * FROM Users;
   ```

   應至少有一筆：Id=0001, Username=aimarliu, Password=aimarliu

---

### 4. 按 Enter 時綁定尚未同步

若 `UpdateSourceTrigger` 設定不當，按 Enter 時最後一個字元可能尚未寫回 ViewModel。

本專案已設定 `UpdateSourceTrigger=PropertyChanged`，一般應無此問題。若仍有疑慮，可嘗試在失去焦點後再按 Enter 測試。

---

### 5. 導航或 ContentHost 設定問題

`NavigationService.NavigateToMain()` 會設定 `ContentHost.Content`，若 `ContentHost` 或版面配置有誤，主畫面可能看不到。

`ShellWindow.xaml` 中 `ContentHost` 設有 `Grid.RowSpan="2" Grid.ColumnSpan="2"`，理論上會覆蓋整個視窗。

---

## 建議排查步驟

1. **確認密碼**：輸入 `aimarliu`（全小寫）並按 Enter
2. **確認資料庫**：執行應用程式後檢查 `app.db` 是否存在，並查詢 `Users` 表
3. **確認 Log**：若 LogPanel 顯示「登入成功」與「導航至主畫面」，表示驗證與導航邏輯正常，問題可能在 MainView 顯示
4. **測試 Password 綁定**：加 `_logService.Append($"嘗試登入，密碼長度：{Password.Length}");` 觀察輸入後的長度

---

## 對照表

| 現象 | 可能原因 |
|------|----------|
| Log 顯示「密碼錯誤」且密碼長度為 0 | Password 綁定未更新 |
| Log 顯示「密碼錯誤」且密碼長度正確 | 密碼不符或資料庫無對應紀錄 |
| Log 顯示「登入成功」但畫面沒變 | 導航或 MainView 顯示問題 |
| 無 Log 訊息 | LoginCommand 未觸發 |
| app.db 不存在 | 資料庫未初始化 |
| Users 表為空 | 種子資料未插入 |
