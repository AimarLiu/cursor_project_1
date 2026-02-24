# Lesson Learned：DataGrid 標題欄看不到字、選取卡頓或無顯示

## 問題描述

在 Layout2 畫面的兩個 DataGrid（生產排程、生產完成訂單）上：

1. **標題欄的字無法看見／高度太小**：欄位標題列沒有顯示文字、與背景難以分辨，或標題列高度被壓扁導致完全看不到字。
2. **選取完全沒有作用和反應**：點選列時沒有明顯的「已選取」視覺回饋，或選取無任何反應。

## 原因分析

### 標題欄看不到字

- **DataContext 綁定錯誤**：`DataGridColumn` 不在 WPF 的 Visual Tree 裡，因此 `Header="{Binding OrderNoHeader.Value}"` 的 DataContext **不是** DataGrid 的 DataContext（即 ViewModel）。Binding 找不到 `OrderNoHeader`，結果標題為空。
- **前景色與背景對比不足**：若使用深色主題（例如 WPF-UI Dark），標題列若未特別設定 `Foreground`，預設可能是深色字，在深色背景上幾乎看不見。
- **標題列高度不足**：未設定 `MinHeight` 或足夠 `Padding` 時，標題列可能被壓得很扁，文字無法完整顯示。

### 選取卡頓／無顯示

- **選取視覺不明顯**：沒有為「選取列」定義專用樣式時，WPF 預設的選取色在深色背景（如 `#40000000`）下對比不足，使用者會覺得「選了像沒選」。
- **主題覆蓋 RowStyle**：使用 WPF-UI 等主題時，其 DataGrid 的 ControlTemplate 可能覆蓋或優先於自訂的 `RowStyle`，導致僅設 Row 選取樣式仍看不到選取效果；改由 **Cell 層級**（`CellStyle`）設定選取視覺較可靠。
- **系統選取色被主題取代**：若未覆寫 `SystemColors.HighlightBrushKey`／`HighlightTextBrushKey`，主題的預設選取色可能與背景相近或無法顯示。
- **選取模式未明確**：未設定 `SelectionMode`、`SelectionUnit` 時，在某些情況下可能影響選取反應或視覺更新。
- **虛擬化**：啟用 `VirtualizingStackPanel.IsVirtualizing="True"` 時，若未配合適當的 Row 樣式或 VirtualizationMode，可能加重卡頓感。

## 解決方案

### 標題欄可見

1. **標題文字改由 code-behind 填入（推薦）**  
   `DataGridColumn` 不在 Visual Tree，XAML 的 `Header="{Binding DataContext.XXX, ElementName=DataGrid}"` 在部分環境仍不顯示。改為在 **Layout2View.OnLoaded** 中，依 ViewModel 的 `LocalizedString`（RESX Layout2_OrderNo、Layout2_VersionNo 等）設定每個欄的 `Header`：
   - 生產排程：`ScheduleColOrderNo.Header = vm.OrderNoHeader.Value`（生產訂單號）、VersionNo、OrderQuantity、BoxType、Category、CustomerName、Remarks。
   - 生產完成訂單：`CompletedColDateTime.Header = vm.DateTimeHeader.Value`（日期時間），其餘同上。
   - XAML 中為每個 `DataGridTextColumn` 設 `x:Name`，不設 `Header` 綁定。

2. **標題列樣式（深灰底、白字、字體 18）**  
   `Layout2DataGridColumnHeaderStyle` 設定：
   - **`Background="#333333"`**（深灰色，避免白底看不到字）
   - `Foreground="White"`
   - `Padding="6,6"`、`MinHeight="28"`、`VerticalContentAlignment="Center"`
   - **`FontSize="18"`**、`FontWeight="SemiBold"`  
   兩個 DataGrid 皆設定 `ColumnHeaderStyle="{StaticResource Layout2DataGridColumnHeaderStyle}"`。

### 選取視覺與卡頓

1. **自訂 Row 選取樣式**  
   新增 `DataGridRow` 的 Style（例如 `Layout2DataGridRowStyle`）：
   - Trigger `IsSelected="True"`：`Background="#1976D2"`、`Foreground="White"`。  
   兩個 DataGrid 皆設定 `RowStyle="{StaticResource Layout2DataGridRowStyle}"`。

2. **Cell 層級選取樣式（必要，避免主題覆蓋）**  
   新增 `DataGridCell` 的 Style（例如 `Layout2DataGridCellStyle`），在 Trigger `IsSelected="True"` 時設定：
   - `Background="#1976D2"`、`Foreground="White"`。  
   兩個 DataGrid 皆設定 `CellStyle="{StaticResource Layout2DataGridCellStyle}"`。  
   如此選取視覺由儲存格層級強制套用，較不易被 WPF-UI 等主題的 Row 模板覆蓋。

3. **覆寫系統選取色**  
   在**每個 DataGrid** 的 `DataGrid.Resources` 內加入：
   - `SolidColorBrush x:Key="{x:Static SystemColors.HighlightBrushKey}" Color="#1976D2"`
   - `SolidColorBrush x:Key="{x:Static SystemColors.HighlightTextBrushKey}" Color="White"`  
   讓 WPF 內建選取邏輯也使用同一組顏色，避免主題取代後選取仍不明顯。

4. **明確選取模式**  
   設定 `SelectionMode="Single"`、`SelectionUnit="FullRow"`。

5. **虛擬化**  
   維持 `VirtualizingStackPanel.IsVirtualizing="True"`，並設定 `VirtualizingStackPanel.VirtualizationMode="Recycling"`，有助減少虛擬化造成的卡頓。

6. **選取僅整列高亮、不要格子黑框**  
   在 `Layout2DataGridCellStyle` 中設 `BorderThickness="0"`、`FocusVisualStyle="{x:Null}"`，並在 `IsSelected` Trigger 內設 `BorderThickness="0"`、`BorderBrush="Transparent"`，避免每個儲存格出現黑框。

7. **欄位字體與欄寬**  
   - 兩個 DataGrid 皆設 **`FontSize="18"`**，標題列樣式亦設 `FontSize="18"`。
   - 欄寬約為原設定 1.5 倍：生產排程 135/135/105/75/75/*/*，生產完成訂單 210/135/135/105/75/75/*/*。

## 補充說明

- 若選取仍感覺延遲，可檢查 ViewModel 中 `SelectedScheduleItem` 的 setter 是否在選取當下做了較重的工作（例如即時寫入資料庫、大量計算）；必要時可改為非同步或延後處理，避免阻塞 UI 執行緒。
- 與選取時序相關的另一起案例（F2 生產完成時先清除選取再移除項目）見 [MoveToCompletedFail.md](MoveToCompletedFail.md)。
