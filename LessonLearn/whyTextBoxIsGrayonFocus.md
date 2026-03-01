# Lesson Learned：頂部輸入框點擊後背景變深灰、字看不清楚

## 問題描述

在 Phase 6 訂單製作畫面（OrderMakingView）頂部有兩個輸入框：

- **版號搜尋**（VersionNoSearch）
- **版號查詢**（VersionNoQuery）

使用者點擊進入可輸入狀態時，背景變成深灰色、字是黑色，對比不足，完全看不清楚。

## 原因分析

1. **只設了內聯背景，未控制焦點狀態**  
   這兩個 TextBox 只設了內聯 `Background="#FFFFFF"`、`Foreground="Black"`。取得焦點時，WPF 預設或系統主題會套用「焦點樣式」，常會把背景改成深色，因而覆蓋掉設定的白底，變成深灰底＋黑字難以辨識。

2. **游標顏色不當**  
   當時設了 `CaretBrush="White"`，在白底上幾乎看不到游標；若焦點時被主題改成深底，游標與背景的搭配也不一定合理。

## 解決方案（已實作）

1. **改為使用 Phase6TextBoxStyle**  
   兩個頂部 TextBox 都改為 `Style="{StaticResource Phase6TextBoxStyle}"`，與下方 Phase 6 輸入框一致。該樣式在焦點時會把背景設成 `#FFF9C4`（黃底）、維持黑字，焦點時就不會再被主題改成深灰。

2. **游標改為黑色**  
   將 `CaretBrush="White"` 改為 `CaretBrush="Black"`，在白底／黃底時游標都清楚可見。

3. **移除多餘內聯**  
   背景、前景、邊框已由 `Phase6TextBoxStyle` 設定，因此移除重複的 `Background`、`Foreground`、`BorderBrush`、`BorderThickness`。

修改後：未焦點時為白底黑字，點擊取得焦點時為黃底黑字，游標為黑色，可正常辨識。

## 相關檔案

- `src/Views/Layout2/OrderMakingView.xaml`：頂部兩個 TextBox 改為使用 `Phase6TextBoxStyle` 並設 `CaretBrush="Black"`。
