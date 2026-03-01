# Lesson Learned：右側數據區滑鼠點選某筆時捲軸自動跑掉

## 問題描述

在 Phase 6 訂單製作畫面（OrderMakingView）右側數據顯示區（4.4 DataGrid）：

- 使用者用滑鼠點擊某一筆資料時，捲軸（scroll bar）會自動捲動到後面，造成畫面跳動、操作困擾。

## 原因分析

- 當時為了實作「從版號輸入框或版號查詢找到訂單時，將該筆定位並捲動到列表中央」，在 View 訂閱了 ViewModel 的 **`PropertyChanged`**，只要 **`SelectedOrder`** 變更就執行「捲動至選取列中央」。
- 因此不論選取是**程式設定**（版號搜尋／版號查詢）還是**使用者手動點選列**，都會觸發捲動。
- 手動點選時，選取列本來就在可見範圍內，再強制捲動到中央反而會把捲軸拉到不預期的位置（例如捲到後面），形成 bug。

## 解決方案（已實作）

1. **區分「程式觸發」與「使用者點選」**  
   在 ViewModel 新增事件 **`RequestScrollToSelected`**，僅在「版號搜尋」(F6) 或「版號查詢」成功找到訂單並設定 `SelectedOrder` 後才呼叫此事件。

2. **View 只依事件捲動**  
   View 改為訂閱 **`RequestScrollToSelected`**，收到時才執行「捲動至選取列中央」；不再依賴 `SelectedOrder` 的 PropertyChanged 來觸發捲動。

3. **結果**  
   - 版號輸入框按 F6 搜尋、或版號查詢欄按 Enter 找到訂單時：仍會選取該筆並捲動到列表中央。  
   - 使用者用滑鼠點選右側某一筆時：只會選取該列，捲軸不再自動移動。

## 相關檔案

- `src/ViewModels/OrderMakingDialogViewModel.cs`：新增 `RequestScrollToSelected` 事件；在 `SearchByVersionNo`、`QueryVersionNoInGrid` 設定選取後呼叫 `RequestScrollToSelected?.Invoke()`。
- `src/Views/Layout2/OrderMakingView.xaml.cs`：移除對 `PropertyChanged`（SelectedOrder）的捲動邏輯；改為訂閱 `vm.RequestScrollToSelected` 並在回呼中執行 `ScrollSelectedToCenter()`。
