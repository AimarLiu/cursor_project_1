# Q&A：ProductionInfoPanel 為什麼是可選？

## 問題描述

> ProductionInfoPanel.xaml(.cs) 為什麼是可選？

（出自 `TODO.md` Phase 4 可能新增的檔案與目錄結構）

---

## 解釋

**左側生產資訊看板**的內容是**必做**的（生產訂單、生產版號、車速、目前產量、受訂量、預估完成時間等），但「要不要拆成一個獨立的 UserControl」是可選的。

| 做法 | 說明 |
|------|------|
| **有 ProductionInfoPanel** | 把左側看板的 XAML 與邏輯抽成 `ProductionInfoPanel.xaml` + `.cs`，在 `Layout2View` 裡用一個 `<ProductionInfoPanel />` 引用。優點：Layout2View 較精簡、左側看板可重用、職責較清楚。 |
| **沒有 ProductionInfoPanel** | 不建這個檔案，把左側看板的所有 XAML 直接寫在 `Layout2View.xaml` 裡。優點：少一個檔案、結構簡單，功能一樣能完成。 |

**結論**：功能（左側生產資訊看板）必做；是否用獨立 UserControl 實作是可選的實作方式。註解寫「(可選) 左側生產資訊看板 UserControl」——可選的是「用不用這個 UserControl」，不是「要不要左側看板」。
