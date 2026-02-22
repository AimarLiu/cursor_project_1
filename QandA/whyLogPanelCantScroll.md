# 為何 LogPanel 無法捲動、訊息過長時無法看到最後一筆

## 無法捲動的主要原因：`IsHitTestVisible="False"`

`LogPanel.xaml` 中的 ListBox 設定了：

```xml
<ListBox x:Name="LogListBox"
         IsHitTestVisible="False"
         ... />
```

`IsHitTestVisible="False"` 會讓 ListBox **完全不接收任何滑鼠／觸控輸入**，包括：

- 滾輪捲動
- 拖曳捲軸
- 點擊捲軸

因此使用者無法透過捲動檢視舊訊息。

---

## 無法看到最新訊息：缺少自動捲到底

`LogService.Append()` 會將新訊息加入 `LogMessages`，但 ListBox 預設**不會**在新增項目時自動捲到底。若訊息過多，新訊息會在下方，使用者無法即時看到最後一筆。

---

## 解法／建議

### 1. 允許捲動：調整 `IsHitTestVisible`

若當初設為 `False` 是為了讓點擊穿透到背後的視窗，需要改回 `True` 才能捲動，或在只允許滾輪捲動、不接受點擊等需求下，用 Attached Behavior 等方式處理。

一般情況下，若要讓使用者能捲動，可改為：

```xml
IsHitTestVisible="True"
```

### 2. 新增訊息時自動捲到底

訂閱 `LogMessages.CollectionChanged`，在新增項目時將 ListBox 捲到最底部，例如：

```csharp
// 在 LogPanel.xaml.cs 中，Load 後訂閱
var logService = DataContext as ILogService;
if (logService != null)
{
    logService.LogMessages.CollectionChanged += (s, e) =>
    {
        if (e.NewItems?.Count > 0)
        {
            LogListBox.ScrollIntoView(LogListBox.Items[LogListBox.Items.Count - 1]);
        }
    };
}
```

或取得 ListBox 內部的 ScrollViewer 後呼叫 `ScrollToEnd()`。

---

## 對照表

| 現象 | 原因 | 解法 |
|------|------|------|
| 無法捲動（滾輪、捲軸皆無效） | `IsHitTestVisible="False"` | 改為 `True`（或依需求用 Attached Behavior） |
| 新訊息出現時看不到最後一筆 | 未自動捲到底 | 訂閱 `CollectionChanged`，呼叫 `ScrollIntoView` 或 `ScrollToEnd` |
