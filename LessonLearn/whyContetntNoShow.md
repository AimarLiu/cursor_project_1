# Lesson Learned：Settings Tab1 內容區空白（圖、Label、TextBox 都不顯示）

## 問題描述

Phase 7 **Settings** 切到 **Tab1（5把刀安全設定）** 時，主內容區看起來是**一片空白**：刀模圖（`Box_Measurement_3-removebg-preview.png`）、各欄 **Label**、**TextBox** 都沒出現。

## 原因分析

1. **分頁可見性用 `Style` + `DataTrigger` 綁在子頁 `UserControl` 上**  
   多個 Tab 子頁疊在同一個 `Grid` 裡，用 **`Visibility`** 切換顯示：預設 **`Collapsed`**，當 **`SelectedSettingsTabIndex`** 等於對應索引時改為 **`Visible`**。

2. **Tab1 單獨把 `DataContext` 改成了子 ViewModel**  
   為了讓 Tab1 只綁 **`SettingsTab1KnifeViewModel`** 的欄位，在 **`SettingsTab1KnifeSafetyView`** 上設了 **`DataContext="{Binding Tab1Knife}"`**。

3. **綁定路徑錯位導致觸發永遠不成立**  
   WPF 的 **`Binding`** 預設以**該元素目前的 `DataContext`** 為起點。  
   當 **`DataTrigger`** 寫在 **`SettingsTab1KnifeSafetyView`** 上、且綁 **`{Binding SelectedSettingsTabIndex}`** 時，解析的是 **`Tab1Knife`（`SettingsTab1KnifeViewModel`）**，上面**沒有** **`SelectedSettingsTabIndex`** 屬性。  
   綁定失敗或永遠對不到 **`Value="0"`**，**`Visibility` 一直保持預設的 `Collapsed`**，因此整塊 Tab1 UI（含圖與輸入框）都被隱藏。

4. **為何其他 Tab 沒事**  
   Tab2～Tab6 當時使用 **`DataContext="{Binding}"`**（整個 **`SettingsViewModel`**），同一個 **`DataTrigger`** 能正確讀到 **`SelectedSettingsTabIndex`**。

## 解決方案（已實作）

1. **不要把「依 `SelectedSettingsTabIndex` 切換可見性」的 `Style` 放在已改寫 `DataContext` 的 `UserControl` 上。**

2. **在外層加一個容器（例如 `Border`），讓容器繼承外層的 `DataContext`（`SettingsViewModel`）**  
   - 容器的 **`Style`／`DataTrigger`** 綁 **`{Binding SelectedSettingsTabIndex}`**，索引對應時設 **`Visibility="Visible"`**。  
   - **內層**再放 **`SettingsTab1KnifeSafetyView`**，並維持 **`DataContext="{Binding Tab1Knife}"`**，專心綁刀位欄位。

如此：**可見性**由 **`SettingsViewModel`** 決定；**Tab1 表單內容**仍由 **`SettingsTab1KnifeViewModel`** 決定，兩者互不衝突。

## 可選寫法（概念）

若堅持不包一層容器，也可在 **`DataTrigger`** 使用 **`RelativeSource`／`ElementName`** 去找 **`SettingsView`** 的 **`DataContext.SelectedSettingsTabIndex`**，但可讀性與維護性通常不如外層 **`Border`** 清楚。

## 相關檔案

- `src/Views/Settings/SettingsView.xaml`：Tab1 改為外層 **`Border`** 控制 **`Visibility`**，內層 **`SettingsTab1KnifeSafetyView`** 綁定 **`Tab1Knife`**。

## 補充（2026-03）：Tab3 同樣症狀

若 **`SettingsTab3OtherPlcParametersView`** 同時設 **`DataContext="{Binding Tab3Other}"`** 又在該 **`UserControl`** 上用 **`DataTrigger`** 綁 **`SelectedSettingsTabIndex`**，會與 Tab1 相同：**觸發綁在子 VM 上** → 永遠 **`Collapsed`**。修正方式同 Tab1：**外層 `Border` 控可見性**，內層 Tab3 **`DataContext="{Binding Tab3Other}"`**。

## 補充（2026-03）：頂部 Tab **`ListBox` 有控制項但字看不到**

若使用 **WPF UI／Fluent** 等主題，**`ListBox`／`ListBoxItem`** 可能被設成與長條底色相同或過低的對比。作法：在 **`SettingsView`** 的 Tab 列對 **`ListBox`** 設 **`Foreground`**（與長條對比之色），並在 **`ItemContainerStyle`** 為 **`ListBoxItem`** 設 **`Foreground` + `Opacity="1"`**，必要時子項 **`TextBlock`** 仍維持明確 **`Foreground`**（見 `SettingsView.xaml` **`SettingsTopTabListBoxItem`**）。
