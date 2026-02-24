# Lesson Learned：F2 生產完成按下時程式 Exception

## 問題描述

在 Layout2 畫面按下 **F2 生產完成** 時，程式拋出例外（exception），無法正常將所選排程項移入生產完成訂單。

## 原因分析

**並非**插入完成訂單的資料錯誤或資料庫寫入問題。

根本原因為 **WPF DataGrid 的選取綁定時序**：

- F2 會呼叫 `MoveSelectedToCompleted(SelectedScheduleItem)`，在 `ProductionScheduleService` 內對 `ScheduleOrders` 做 `RemoveAt(idx)`，將選取項從排程移除並加入 `CompletedOrders`。
- 此時 DataGrid 的 `SelectedItem` 仍透過 TwoWay 綁定指向「正在被移除」的那一筆。
- 從 `ObservableCollection` 中移除「目前選取項」時，WPF 在同步選取狀態與集合變更的過程中會拋出例外。

也就是：**先從集合移除選取項、再更新選取** 會觸發例外；應改為 **先清除選取，再從集合移除**。

## 解決方案

在 `Layout2ViewModel.cs` 中調整 F2、F6 的執行順序：

1. **F2 生產完成**
   - 先將 `SelectedScheduleItem` 存成區域變數 `item`，並記下 `orderNo`。
   - **先設定 `SelectedScheduleItem = null`**（清除選取）。
   - 再呼叫 `_scheduleService.MoveSelectedToCompleted(item)` 並寫入 Log。

2. **F6 撤單**
   - 同樣先保存 `item` 與 `orderNo`，**先設定 `SelectedScheduleItem = null`**，再呼叫 `_scheduleService.RemoveSelectedScheduleItem(item)` 並寫入 Log。

完成訂單的建立與加入 `CompletedOrders` 的邏輯無需變更；目前專案也未對完成訂單做 DB 寫入（Repository 僅有 Load，無 Insert）。

## 修改檔案

| 檔案 | 修改內容 |
|------|----------|
| `src/ViewModels/Layout2ViewModel.cs` | `F2ProductionComplete()`、`F6RemoveOrder()` 改為先清選取再移除 |

## 總結

從綁定為 `SelectedItem` 的集合中移除「目前選取項」時，應**先將選取清空（設為 null）**，再從集合移除該項，可避免 WPF 綁定與集合變更同步時拋出例外。
