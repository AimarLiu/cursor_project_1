# ItemsControl 與其項目來源不一致（Why ItemSource Not Consistent）

## 問題來源

來自未處理例外 log（例：`exception_messages/error_2026-02-25_15-57-55.log`），錯誤訊息為：

- **外層**：`ItemsControl 與其項目來源不一致。如需詳細資訊，請參閱內部例外狀況。`
- **發生位置**：`System.Windows.Controls.ItemContainerGenerator.Verify()`，在 `VirtualizingStackPanel` 的 Measure/Layout 過程中觸發。

## 從 Log 看到的重點

### 1. 外層例外

- 型別：ItemsControl 的產生器（ItemContainerGenerator）在驗證時發現與綁定的集合狀態不符。
- StackTrace 顯示發生在 **layout/measure** 流程中：`VirtualizingStackPanel.MeasureOverride` → `ScrollViewer` → `Grid` → … → `ContextLayoutManager.UpdateLayout()`，代表是在 UI 排版、量測子項目時偵測到不一致。

### 2. 內部例外（真正原因線索）

- **控制項**：名稱為 `LogListBox` 的 `System.Windows.Controls.ListBox`，當時 `Items.Count: 5`。
- **偵測到的差異**：**累積計數 4 ≠ 實際計數 5**  
  （累積計數 = 上次重設時的計數 + 新增數 - 移除數；產生器認為是 4，但集合實際有 5 筆。）
- **標記為最可能來源**（星號）：`System.Collections.ObjectModel.ObservableCollection<string>`，即綁定到 `LogListBox` 的 `LogMessages` 集合。
- **常見原因**：  
  **(a)** 變更集合或其計數但未正確引發對應的 `CollectionChanged`；  
  **(b)** 在**非 UI 執行緒**修改集合，或**(c)** 在 **layout/measure 進行中**修改集合，導致產生器與集合不同步。

### 3. 本專案對應關係

- `LogListBox`：`Views/Shared/LogPanel.xaml` 中的 `ListBox`，`ItemsSource="{Binding LogMessages}"`。
- `LogMessages`：`LogService.LogMessages`，型別為 `ObservableCollection<string>`，由 `ILogService.Append()` 間接呼叫 `LogMessages.Add(line)` 更新。

因此，**任何在非 UI 執行緒或在不當時機對 `LogMessages` 做 Add/Remove**，都可能觸發此例外。

## 解法（本專案採用的修正）

### 第一版：僅確保在 UI 執行緒

- **檔案**：`src/Services/LogService.cs`
- **作法**：在 `Append` 中若不在 UI 執行緒則以 `InvokeAsync(Normal)` 派送，否則直接 `Add`。
- **現象**：本機不再發生，但**異機仍會出現同樣錯誤**，推測與異機 CPU/時序不同，導致「在 layout/measure 進行中」執行到 `Add` 的機率較高。

### 第二版（異機仍發生時的加強防護）

1. **LogService.cs**  
   - **一律**以 `Dispatcher.InvokeAsync(() => LogMessages.Add(line), DispatcherPriority.Loaded)` 排程，不再在當前呼叫中直接 `Add`。  
   - `Loaded` 在 layout 之後執行，避免在 measure/layout 過程中修改集合。

2. **LogPanel.xaml**  
   - 在 `ListBox` 上設定 **`VirtualizingPanel.IsVirtualizing="False"`**。  
   - 關閉虛擬化後不再使用 `VirtualizingStackPanel`，產生器與集合的同步較單純，異機較不易出現計數不一致。

3. **LogPanel.xaml.cs**  
   - 在 `OnLogMessagesCollectionChanged` 內，**延後** `ScrollIntoView`：  
     `Dispatcher.BeginInvoke(..., DispatcherPriority.Loaded)`，在回呼中再執行 `ScrollIntoView(Items[Items.Count - 1])`。  
   - 避免在 `CollectionChanged` 處理中同步觸發 layout，導致與 WPF 內部狀態不同步。

## 除錯用（可選）

- 對 `LogListBox.ItemContainerGenerator` 設定 `PresentationTraceSources.SetTraceLevel(..., High)`，在每次 `CollectionChanged` 後執行偵測，較易在異機抓到發生時機（會變慢，僅建議除錯用）。

## 驗證結果

第二版修正（InvokeAsync(Loaded) + 關閉虛擬化 + ScrollIntoView 延後至 Loaded）已於**異機驗證通過**，本機與異機皆不再出現「ItemsControl 與其項目來源不一致」。

## 總結

| 項目 | 說明 |
|------|------|
| 錯誤名稱 | ItemsControl 與其項目來源不一致 |
| 本專案觸發控制項 | `LogListBox`（ListBox），綁定 `LogMessages`（ObservableCollection\<string\>） |
| 直接原因 | 對綁定集合的修改未在 UI 執行緒或在不當時機（如 layout 中）進行，導致產生器計數與實際不符 |
| 修正 | (1) LogService.Append 一律以 `InvokeAsync(Loaded)` 延後 Add；(2) LogListBox 關閉虛擬化；(3) LogPanel 的 ScrollIntoView 延後至 Loaded |
| 參考 log | `exception_messages/error_2026-02-25_15-57-55.log` |
