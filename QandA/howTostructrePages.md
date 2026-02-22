# 登入頁面與主頁面目錄結構分離

## 討論問題

是否將登入頁面和主頁面的目錄結構分離，方便管理與追蹤？

## 答案

**建議分離**。分離後有利於職責劃分、維護與搜尋，未來功能擴充時也不易混雜。

---

## 關鍵問題點

### 1. 現況結構

- Views、ViewModels 各一層，所有頁面（LoginView、MainView、LogPanel）混在同一資料夾
- 隨頁面數量增加，檔案難以快速定位

### 2. 管理與追蹤需求

- 登入流程與主畫面邏輯不同，分開可減少互相干擾
- 結構清晰有助於多人協作與後續維護

### 3. 結構選擇

- 僅調整 Views 子資料夾 vs. 採用 Feature 型結構
- 需在改動幅度與長期擴充性之間取捨

---

## 解法／作法

### 作法一：Views 底下依頁面分子資料夾（建議起步採用）

維持 Views、ViewModels 分層，僅將 Views 再細分：

```
src/
├── Views/
│   ├── Login/
│   │   ├── LoginView.xaml
│   │   └── LoginView.xaml.cs
│   ├── Main/
│   │   ├── MainView.xaml
│   │   └── MainView.xaml.cs
│   └── Shared/           ← 若 LogPanel 被多處使用
│       └── LogPanel.xaml
├── ViewModels/
│   ├── LoginViewModel.cs
│   └── MainViewModel.cs
```

**優點**：改動小、符合既有 MVVM、僅需調整 namespace 與檔案路徑  
**適合**：專案規模不大、想維持既有架構

### 作法二：Feature 型結構（功能導向）

以功能模組為單位，每個模組包含自己的 View、ViewModel：

```
src/
├── Features/
│   ├── Login/
│   │   ├── LoginView.xaml
│   │   ├── LoginView.xaml.cs
│   │   └── LoginViewModel.cs
│   └── Main/
│       ├── MainView.xaml
│       ├── MainView.xaml.cs
│       ├── MainViewModel.cs
│       └── LogPanel.xaml   ← 若 LogPanel 僅屬於主頁
├── Services/
├── Models/
└── Helpers/
```

**優點**：功能內聚、易擴充、多人協作時職責清楚  
**缺點**：結構變動較大，需調整 namespace、DI、導航等  
**適合**：功能持續增加、追求較高模組化

---

## 比較與建議

| 考量 | 作法一（Views 子資料夾） | 作法二（Feature 型） |
|------|--------------------------|----------------------|
| 改動幅度 | 小 | 大 |
| 與現有架構一致 | 高 | 需較多重構 |
| 功能擴充性 | 中 | 高 |
| 登入／主頁的追蹤 | 清楚 | 更清楚 |

**建議**：先採用作法一，改動集中在 Views，待專案成長後再視需求演進至作法二。

---

## 實作時需檢查的項目

- NavigationService 中登入／主頁的路徑或型別
- `CursorTestApp.csproj` 對 Views 檔案的引用
- 專案內所有 `LoginView`、`MainView` 的 namespace 引用
- `LogPanel` 的共用性，決定放在 `Shared/` 或 `Main/` 底下
