# Lesson Learned：Settings Tab3 DataGrid 標題文字不顯示

## 問題描述

在 Phase 7 的 `Settings -> Tab3` 中，`DataGrid` 的標題列會出現「**背景有顯示，但欄位標題文字完全不見**」的情況。  
此問題在切換語系（尤其日文）時更容易被觀察到，容易誤判成 RESX 或 FallbackValue 問題。

## 初期誤判與排查過程

1. 先懷疑是 `Foreground` 與背景色衝突（白字/深灰底）  
   - 已明確指定 `DataGridColumnHeader` 與 `TextBlock` 前景色，問題仍存在。

2. 先前曾使用 `FallbackValue` / `TargetNullValue` 當保底顯示  
   - 雖可暫時看到字，但屬 workaround，且會掩蓋真正 binding 失效原因。

3. 檢查 `Resources.ja.resx`  
   - 確認有一部分 key 的確是英文佔位（這是另一個多語系問題），但**不是本次「標題完全空白」的唯一根因**。

4. 嘗試調整 `DataGridTextColumn.Header` 的綁定來源  
   - `RelativeSource AncestorType=UserControl`、`AncestorType=DataGrid`、`ElementName=Root` 都嘗試過，仍有不穩定或失效情況。

## Root Cause（最終確認）

`DataGridTextColumn` 與其 `Header` 內容不是一般視覺樹中穩定可預期的綁定場景。  
在 Tab3 這個頁面（搭配主題樣式與動態語系）下，**把多語系字串直接綁在 `DataGridTextColumn.Header` 內的 `TextBlock` 會有解析不穩定問題**，導致標題內容為空字串/未渲染。

簡單說：  
**不是字體顏色問題，也不只是 RESX 值問題；核心是「Column Header 的 XAML Binding 不穩定」。**

## 最終解法（採用 Layout2 同策略）

改為與 `Layout2View` 一致的「最穩定做法」：

1. **不在 Tab3 的 Header 裡做複雜 Binding**  
2. 在 `SettingsTab3OtherPlcParametersView.xaml.cs` 的 `Loaded` 階段，直接把欄位標題字串指派給欄位 `Header`
3. 訂閱 `LocalizedString.PropertyChanged`（`Value`）以支援語系切換後即時刷新標題

## 為什麼這個解法可靠

- 避開 `DataGridTextColumn.Header` 的不穩定 binding 情境
- 與 `Layout2` 的現行策略一致，維護成本低
- 語系切換時可以明確控制刷新時機，不依賴模板/視覺樹

## 關鍵檔案

- `src/Views/Settings/Tabs/SettingsTab3OtherPlcParametersView.xaml`
  - `DataGridTextColumn` 改為命名欄位（例如 `ColDepartment`、`ColComponentName` 等）
- `src/Views/Settings/Tabs/SettingsTab3OtherPlcParametersView.xaml.cs`
  - 於 `Loaded`、`DataContextChanged`、`LocalizedString.Value` 變更時，集中刷新欄位標題文字
- `src/Views/Layout2/Layout2View.xaml.cs`
  - 參考實作：以 code-behind 直接設定 DataGrid 欄位 Header（已長期穩定）

## 後續規範建議

1. 若是 WPF `DataGridTextColumn.Header` + 動態多語，**優先使用 code-behind 指派 Header**。  
2. `FallbackValue/TargetNullValue` 僅作短期偵錯，不作最終方案。  
3. 多語系問題請分開檢查兩件事：  
   - key 是否存在且翻譯值正確（RESX）  
   - UI 是否真的能穩定取得該值（Binding/Code-behind）

