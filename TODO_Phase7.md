# Project TODO - Phase 7：Settings（PLC 數據取得/切割控制）

## Phase 7 實作進度紀錄

### 昨日（2026-03-22）— 頁面與骨架：**已完成**

以下為已落地於程式碼之項目（導航、帳號、Settings 主殼、底部列與 F1；**Tab1～Tab3** 已接 DB 與完整 UI）。

| 狀態 | 項目 | 說明／檔案 |
|------|------|------------|
| ✅ 已完成 | `NavigateToSettings()` | `INavigationService`／`NavigationService`；`ShellWindow` 以 factory 建立 `SettingsView` + `SettingsViewModel` |
| ✅ 已完成 | Admin 登入分流 | `LoginViewModel`：`Username == "Admin"` → `NavigateToSettings()`；其餘 → `NavigateToLayout2()` |
| ✅ 已完成 | Admin 測試帳號種子 | `DatabaseService.EnsureAdminUserIfMissing`：`Id=0002`，`Admin`／`Admin`（無則插入） |
| ✅ 已完成 | Settings 主頁殼 | `SettingsView.xaml`：頂部 **6 分頁**（`ListBox` + `SelectedSettingsTabIndex`）、下方 **F1～F6** 圖+文底部列（RESX）；**無**內容區整頁 Title 列 |
| ✅ 已完成 | Tab1～Tab6 子頁檔案 | 各 Tab 對應 `Views/Settings/Tabs/*View.xaml`；Tab4／Tab5 **留白**、Tab6 **空白**，符合 `TODO_Phase7_Layout.md` |
| ✅ 已完成 | 分頁與底部文案 RESX | `SettingsTab1_TabHeader`～`SettingsTab6_TabHeader`、`Settings_Footer_F*`、`SettingsTabN_SkeletonBody` 等（五語系 `Resources.*.resx` 已補） |
| ✅ 已完成 | F1 離開 | `SettingsViewModel.FooterF1Exit` → `NavigateToLogin()`（回登入，不留在 Shell 主功能內） |
| ✅ 已完成（2026-03-23） | **Tab1** 完整 UI + DB | 見下方 Tab1 勾選 |
| ✅ 已完成（2026-03-23） | **Tab2** 完整 UI + DB | 見下方 Tab2 勾選；**F2** 依序儲存 Tab1+Tab2、**F3** 重載 Tab1+Tab2 |
| ⏳ 占位 | **F5／F6** | 仍僅 Log（見 §7.6） |
| ✅ 已完成（2026-03-23） | **Tab3** 完整 UI + DB | 見下方 Tab3 勾選；**F2** 依序 Tab1→Tab2→Tab3、**F3** 重載三者 |

---

### Tab1～Tab3 待實作細項（勾選：`- [x]` 已完成，`- [ ]` 未完成）

> 對照主規 §7.5.1～§7.5.3、`TODO_Phase7_Layout.md` Tab1～Tab3。

#### Tab1：5把刀安全設定（§7.5.1／Layout Tab1）

- [x] **資料庫**：`PlcParameterDefinitions` 登錄 `KnifeSlot_*` 共 16 鍵（`TabGroup=KnifeSafety`）；`PlcParameterSetProfiles`（ProfileId=**1** `KnifeSafetyGlobal`）／`PlcParameterValues` 種子與讀寫（實作於 `DatabaseService` + `KnifeSafetyParameterRepository`）
- [x] **UI 版面**：刀模底圖、`ScrollViewer` **VerticalContentAlignment=Center** + `MaxWidth` 邊框 **水平置中**；右欄 H/I/B **RowSpan** 垂直置中；**不**實作舊 HMI 右側按鈕列
- [x] **16 個 TextBox**：綁定 `SettingsTab1KnifeViewModel`；`NULL` 顯示空白
- [x] **樣式**：MAX 藍底白字、MIN 紅底橘字（`KnifeMaxTextBox`／`KnifeMinTextBox`）
- [x] **驗證**：同一刀位 Min、Max 皆有值時 **`Min <= Max`**；非空必須為整數
- [x] **ViewModel**：`SettingsTab1KnifeViewModel`；**F2** → `TrySaveToDatabase`；**F3** → `ReloadFromDatabase`
- [x] **檔案**：`SettingsTab1KnifeSafetyView`；`SettingsView` 以 **`DataContext={Binding Tab1Knife}`** 綁子 VM

#### Tab2：紙箱參數（§7.5.2／Layout Tab2）

- [x] **參照表與種子**：`KnifeCombinedOptions`（9 筆）、`KnifeCombinedOptionByBoxType`、`BoxTypes`（C/E/DA7）、`CorrugatedTypes`（至少 `Id=0`）、`PlcParameterSetProfiles`（每箱型 Profile）— 見 `DatabaseService`／`BoxDieCutterSettingsRepository`
- [x] **刀組合**：三區 Combo 依箱型篩選；選中寫入 **`BoxParameters.SelectedKnifeCombinedOption`**（`ValueInt` 0～8）至對應箱型 Profile
- [x] **箱型欄位 UI**：`KnifePullOut`、`UseDatabaseBlade`、`UseEquation2`、`Front/Back/BothKnifePullOut`、`AutoJudge` 等（C/E/DA7 依 Layout 顯示／隱藏）
- [x] **連續參數**：`DieCutter.*` 11 項 + `DieCutter.CarSpeed`（Combo：**0 張/分鐘**／**1 張/小時** RESX）
- [x] **RESX**：所有 `SettingsTab2_*`（GroupBox、Label、CheckBox、刀組合 `SettingsTab2_KnifeCombo_0`～`_8` 等），XAML **不**寫死使用者可見字串（ja／pt／th 刀組合等暫用英文占位）
- [x] **已取消項**：附圖「紙長」等 **不**實作（與 Layout 一致）
- [x] **實作檔案**：`SettingsTab2BoxParametersView`、`SettingsTab2BoxViewModel`、`IBoxDieCutterSettingsRepository`／`BoxDieCutterSettingsRepository`；`SettingsView` Tab2 外層 **`Border` + `DataContext={Binding Tab2Box}`**

#### Tab3：其他參數（§7.5.3／Layout Tab3）

- [x] **通訊**：`PlcCommChannelSettings` 三列（`Printer`／`Panel`／`PLC`）；印／看 **Port**；糊車 **Port + Baud + Byte + Parity + Stop**；預設值與 §7.5.3 一致（`DatabaseService` + `PlcCommChannelRepository`）
- [x] **左側部門清單 + DataGrid**：`ListBox` 部門選取（含 Feed、Print1～8、Slotter…、Other、All）；欄：**元件名稱**（可編輯顯示名）／最大值／最小值／精準度；欄標題僅 RESX；**All** 時顯示部門欄
- [x] **列映射**：Feed 8 列、`Print1～3`／`Print7`／`Print8` 各 6 列、`Other` 6 列、`All` 合併順序；`Print4～6` 等無種子可空白
- [x] **數值鍵**：`Feed.*`、`Print.{n}.*`、`Component.*` → `PlcParameterDefinitions`／`PlcParameterValues`（ProfileId=**6** `OtherPlcGlobal`，`TabGroup=OtherPlcParameters`）
- [x] **`ComponentDisplayNameOverrides`**：`DisplayNamesJson` 多語覆寫（目前語系寫入 JSON）；**F2** 以 **`ReplaceAll`** 覆寫整表
- [x] **使用者密碼**：`Users` 表維護；**`UserDirectoryRepository`**；Login 已每次查 DB（既有 `AuthService`）
- [x] **Repository**：**`IPlcCommChannelRepository`**、**`IOtherPlcParametersRepository`**、**`IComponentDisplayNameRepository`**、**`IUserDirectoryRepository`**（命名與 §7.7 草稿可日後對齊）
- [x] **實作檔案**：`SettingsTab3OtherPlcParametersView`、`SettingsTab3ViewModel`、`Models/Tab3/Tab3PlcCatalog.cs`、`Tab3DatabaseBootstrap.cs`

#### 跨 Tab（與 Tab1～3 共用）

- [x] **F2 儲存（完整 §7.6；單一 transaction）**：F2 建立 **同一個** `SqliteConnection + BeginTransaction()`，以外部 transaction 分別寫入 **Tab1 → Tab2 → Tab3**；任一頁失敗則整體 rollback
- [x] **F3 放棄（Tab1～3）**：F3 同時 **`Tab1Knife`／`Tab2Box`／`Tab3Other`** **`ReloadFromDatabase`**
- [ ] **建表／種子腳本**：獨立檔 `Scripts/CreatePlcSettingsTables.sql` 等（§7.5.6）；**Tab1 表**已於 **`DatabaseService`** 內建表／種子（程式內遷移）

---

## Phase 7：PLC 數據取得 Settings 頁（Tab 介面、Admin 進入）

### 7.1 頁面與 NavigateToSettings 導航串接
- 建立 `SettingsView`（主內容區頁面），由 `NavigateToSettings` 切換顯示於 `ShellWindow` 的 `ContentHost`。
- 在 `INavigationService` 新增 `NavigateToSettings()`，並補上對應的 View 建立 factory。
- 在 `ShellWindow` 註冊/建立 `SettingsView`（依現有架構注入 `ILocalizationService`、`ILogService`、DB service/repository）。
- 登入成功後分流：
  - `Username == "Admin"`：導向 `NavigateToSettings()`（**僅 Admin 可進入 Phase 7 Settings**）
  - 其他使用者：維持既有流程（導向 `NavigateToLayout2()`）
- **離開 Settings**（底部 **F1**）：**固定** `NavigateToLogin()`（或等效「回到登入頁」），**不**留在 Shell 主功能內，與 §7.6 一致。

### 7.2 登入帳號新增（Admin/Admin）
- 在 SQLite `Users` 表 seed/初始化新增一筆（若不存在則插入）：
  - `Id`：`0002`（或沿用專案既有規則）
  - `Username`：`Admin`
  - `Password`：`Admin`（明文，測試用途）
- 更新 `DatabaseService.Initialize()` / `SeedUsersIfEmpty`，確保首次啟動時可得到 `Admin/Admin`。

### 7.3 Tab 介面規格（6 個頁：前 5 依附圖、Tab6 空白）
- **不**在 Settings 主內容區頂部另加「頁面 Title」列（與 **Layout2**、**OrderMakingView** 相同）；**不**使用 `SettingsTabN_Title` 類鍵。僅 **分頁標籤**需 RESX（`SettingsTab1_TabHeader`～`SettingsTab6_TabHeader`）。
- **分頁標題（zh-TW 定案順序）**：`SettingsTab1_TabHeader`～`SettingsTab6_TabHeader` 對應 **5把刀安全設定／紙箱參數／其他參數／歷史警報／生產管理參數／備份區**（各語系以 `Resources.*.resx` 為準；詳 **`TODO_Phase7_Layout.md` §分頁標題附錄**）。
- `SettingsView` 放置 `TabControl`：
  - Tab1~Tab3：依附圖建立對應子頁（UI 骨架／接 DB）
  - **Tab4、Tab5**：**本階段不實作內容**，子頁**留白**占位（仍保留分頁與 View 檔名）；**但** `TabControl` **分頁標題**仍須顯示且走 **RESX**（`SettingsTab4_TabHeader`、`SettingsTab5_TabHeader`，五語系）。詳見 **`TODO_Phase7_Layout.md` §Tab4／§Tab5**；DB／參數規格仍可在 §7.5.4／§7.5.4.1 供日後擴充。
  - **Tab6**：保留空白子頁（**僅 TabHeader**）；**RESX** **`SettingsTab6_TabHeader`**（zh-TW 預設顯示字串：**備份區**，見 **`TODO_Phase7_Layout.md` Tab6** 與 **Tab1～Tab6 附錄：分頁標題 RESX**）。
- Tab 子頁建議拆檔（後續各自 ViewModel/RESX）：
  - `SettingsTab1_KnifeSafetyView`
  - `SettingsTab2_BoxParametersView`
  - `SettingsTab3_OtherPlcParametersView`
  - `SettingsTab4_AlarmHistoryView`
  - `SettingsTab5_ProductionManagementParametersView`
  - `SettingsTab6_EmptyView`
- `TabControl` 的 Tab Header、各 Tab 內 **GroupBox／Label** 等區塊文字走 RESX（使用 `LocalizedString` 或 RESX 綁定）；**無**整頁級 Title 字串。

### 7.4 RESX 多國語系（Settings 頁 + Tab 子頁）
- 在 `Resources.*.resx` 補齊 Settings 與各 Tab 的字串 Key（五語系：`en / ja / zh-TW / pt / th`）。
- 原則：
  - `Settings_` 為總前綴
  - Tab 用 `SettingsTab{n}_...` 命名
  - **底部按鈕列**（見下表 `Settings_Footer_*`）：**所有**按鈕顯示文字（含圖旁標籤）**僅**能來自 RESX，**不**在 XAML 寫死
- **Tab2、Tab3、底部按鈕列 — 多語系定案**：
  - **Tab2**：`TabControl` 分頁 Header、各 `GroupBox.Header`、`Label`、`CheckBox`、`TextBlock`／副標、連續參數與車速相關 **Label**、**ComboBox** 選項（含車速 **0 張/分鐘**／**1 張/小時**）、**刀組合**顯示字（經 `DisplayTextKey` → `SettingsTab2_KnifeCombo_*`）— **皆** `SettingsTab2_*` RESX；數值 **TextBox** 內容為數字不需翻譯，**ToolTip** 若有則亦需 Key。
  - **Tab3**：見 `TODO_Phase7_Layout.md` — 含 **GroupBox**（如 **其他通訊埠設定**、**糊車 PLC**、**使用者密碼管理**）、部門 Radio、DataGrid **欄標題**、子標籤（印表機 PLC／看板／糊車 PLC 埠等）— **皆** `SettingsTab3_*`；**元件名稱**格內使用者輸入為 DB 多語，非硬編碼。
  - **Tab4、Tab5、Tab6**：Tab4／5 **無內容區 UI**（本階段留白），Tab6 **空白子頁**；**仍須** `SettingsTab4_TabHeader`、`SettingsTab5_TabHeader`、**`SettingsTab6_TabHeader`**（見 `TODO_Phase7_Layout.md` **Tab1～Tab6 附錄：分頁標題 RESX**）。
  - **底部按鈕列**（`SettingsView` 共用）建議 Key（**預設顯示字串**以 zh-TW 種字串為參考，可調）；**圖示檔**見 §7.6 **「文案與圖示（定案）」**。
    | F 鍵 | RESX Key | 預設顯示字串（zh-TW；可調） |
    |------|----------|---------------------------|
    | **F1** | `Settings_Footer_F1_Exit` | **F1 離開**（行為：回 Login，見 §7.6） |
    | **F2** | `Settings_Footer_F2_Save` | **F2 儲存** |
    | **F3** | `Settings_Footer_F3_Cancel` | **F3 放棄修改** |
    | **F5** | `Settings_Footer_F5_MachineDimension` | **F5 機器尺寸** |
    | **F6** | `Settings_Footer_F6_SpecialParams` | **F6 特殊參數區** |
  - **版面（定案）**：整列三等分 — **左欄**由左至右 **F2（最左）**、**F5**；**中欄** **F6**（中左對齊）；**右欄**由左至右 **F3**、**F1（最右）**。
  - 若按鈕另需 **AccessKey**／**ToolTip**，各增 `Settings_Footer_*_Tooltip` 等 Key。

---

### 7.5 基於附圖的 DB 與資料編排（先規劃、先建表、先放 default）
> 重點：先把「UI 上每個 textbox/radio/combobox」對應到穩定的 DB Key，再用參數集（profile）存實際值，之後才串 PLC。

#### 7.5.1 Tab1：馬達運轉/切割用 PLC 設定（**已確認**）
> 此 Tab 的目標：**提供 PLC 切割馬達安全/限制的閾值（MAX/MIN）**，讓切割動作可依紙箱位置/代號控制與保護。

- 建議使用「參數 Key」的方式管理（不要用大量動態欄位，提升易讀性與可擴充）
- 參數 Key 與預設值（**已確認**；`NULL` 表示種子不寫入數值，UI 顯示空白）：
  - `KnifeSlot_A_Min`  default：`750`
  - `KnifeSlot_A_Max`  default：`2700`
  - `KnifeSlot_B_Min`  default：`340`
  - `KnifeSlot_B_Max`  default：`1150`
  - `KnifeSlot_E_Min`  default：`220`
  - `KnifeSlot_E_Max`  default：`NULL`
  - `KnifeSlot_J_Min`  default：`100`
  - `KnifeSlot_J_Max`  default：`NULL`
  - `KnifeSlot_K_Min`  default：`220`
  - `KnifeSlot_K_Max`  default：`NULL`
  - `KnifeSlot_F_Min`  default：`100`
  - `KnifeSlot_F_Max`  default：`NULL`
  - `KnifeSlot_H_Min`  default：`0`
  - `KnifeSlot_H_Max`  default：`1272`
  - `KnifeSlot_I_Min`  default：`0`
  - `KnifeSlot_I_Max`  default：`1272`

- 實作備註（Tab1）：
  - 每個 Key 需在 `PlcParameterDefinitions` 登錄（`TabGroup=KnifeSafety`、`ValueKind` 依數值為 INT 或 REAL）。
  - 儲存前建議驗證：同一刀位若 `Min`、`Max` 皆有值，則 **`Min <= Max`**；若任一端為 `NULL` 則略過該組比對。
  - 下發 PLC 前再依業務決定是否將 `NULL` 視為「不變更/不傳送」或「傳 0」。

> 重要：這些 Key 最終都會落在 `PlcParameterValues`（見 7.5.5），因此 Tab1 UI 只要綁定這些 Key 即可。

#### 7.5.2 Tab2：印刷模切機相關參數與箱型刀選擇（DieCutter Parameters）
> 原理: 紙箱印刷模切機（常稱「五把刀」或五聯機）是自動化紙箱生產的關鍵設備，它整合了印刷、開槽、壓痕與模切功能。其原理是瓦楞紙板通過設備時，先進行水性印刷，隨後經刀模（軋型）切割出複雜形狀（如飛機盒），實現一次成型、無毛刺。 其組成部件數量通常涉及 5 个關鍵的切割或印壓轉輪，故俗稱「五把刀」或「五聯機」：
>   印刷部 (1-2把/組)：負責上色印製。
>   開槽/壓痕部 (3-4把/組)：負責紙箱箱體的開槽與折線壓痕。
>   模切部 (5把/組)：使用刀模切出特殊邊緣或異形孔洞。
> 印刷模切機製作紙箱五個步驟：
> Phase 1: 送紙：自動進紙單元將瓦楞紙(A, AB, C, E, F..跟厚度有關)板依序送入。
> Phase 2: 印刷：紙板通過印刷滾筒，完成商標圖案印製。
> Phase 3: 開槽/壓痕：機器對印刷後的紙板進行開槽和預壓痕，這是傳統紙箱的必要步驟。
> Phase 4: 模切（軋型）：這一步驟是將紙板裁切成最終形狀，如果是不規則紙箱（如飛機盒）或需精密孔洞，則需此工序。
> Phase 5: 排廢與成品輸出：機器自動切除廢邊，輸出成型後的紙箱。 
> 箱型: (可根據Box_Measurement_3-removebg-preview.png圖示版面)
>   E型箱 (又稱為A型箱): 一般常見的箱型，上下兩側需要開槽部位總共6個，一般會使用印刷模切機的2~3把刀(印刷部、開槽/壓痕部與模切部的組合)
>   C型箱: 上下兩側需要開槽部位總共4個，一般會使用印刷模切機的2把刀(印刷部與開槽/壓痕部的組合)
>   D/A7箱 (又稱DA5型箱): 上下兩側需要開槽部位總共8個，一般會使用印刷模切機的3把刀(印刷部、開槽/壓痕部與模切部的組合)
> 車速: 實際產生紙箱的速度，指機器每分鐘產出的紙箱個數，「張/小時 (pcs/hr)」。

> 附圖 Tab2 主要是「切割/送紙/背板/風參數/車速」等箱體參數，含少量「離散選項」（刀組合/刀選擇模式）。

1. E，C，DA7 類型紙箱刀把選擇和其額外選項（建議用獨立參照表，易讀性更好）
  （**定案**：刀組合為**各箱型共用**之**9 筆**主檔 `KnifeCombinedOptions`；C／E／DA7 畫面上原共 **11 列**選項，係因 **「0:2,3刀」「2:3,4刀」** 在 C 與 E **重複出現**，故以 **9 種相異文案** 對應 **9 個 PK**；重複顯示**共用同一 `DisplayTextKey`**。依**箱型**篩選可選項見 `KnifeCombinedOptionByBoxType` 與 `TODO_Phase7_Layout.md` Tab2 附錄 B。）
  - `KnifeCombinedOptions`（**固定 9 筆**）
    - `Id`（PK，自增；種子 **`KnifeCombineOpt_0`～`KnifeCombineOpt_8`**）
    - `Code`（字串，建議 **`"0"`～`"8"`**，與 `Id` 序一致）
    - `DisplayTextKey`（RESX：`SettingsTab2_KnifeCombo_0`～`_8`；**一筆選項一個 Key**，重複出現在不同箱型時仍指同一 `KnifeCombinedOptionId`／同一 Key）
    - `SortOrder`（int，全域排序 0～8）
  - `KnifeCombinedOptionByBoxType`（**哪些組合出現在哪种箱型**；C／E／DA7 可選子集不同）
    - `BoxTypeId`（FK → `BoxTypes.Id`）
    - `KnifeCombinedOptionId`（FK → `KnifeCombinedOptions.Id`）
    - `UNIQUE(BoxTypeId, KnifeCombinedOptionId)`
  - `KnifeSelectionModes`
    - `Id`（PK）
    - `Code`（`Manual` / `FromDatabase` / `Auto`；**UI 上多為 CheckBox「使用資料庫選刀」「自動判斷」與預設手動選組合並存時的語意**）
    - `LabelKey`（RESX key）
  - `BoxTypes`（**一種箱型一列**；下列為該箱型之預設／目前值，**非**三種共用同一列）
    - `Id`（PK，整數；**勿**與 **`Code`** 混淆——下列 **`Box.C`** 等僅為 **`Code` 欄位之示意**，**不是** `Id` 的字串值）
    - `Code`（建議：`C` / `E` / `DA7`，程式與 FK／種子用）
    - `LabelKey`（RESX：GroupBox 標題，如「C型箱 選擇刀」）
    - `KnifePullOut`（開槽部咬紙退刀；預設：C/E **-5**，DA7 **0** — 依附圖）
    - `UseEquation2`（僅 **E** 有意義；預設 `false`）
    - `FrontKnifePullOut` / `BackKnifePullOut`（僅 **E**；預設 `20`）
    - `BothKnifePullOut`（僅 **E**，且 **UseEquation2=false** 時作用；預設 `5`）
    - `UseDatabaseBlade`（bool；僅 **E** 顯示；對應勾選「使用資料庫選刀」）
    - `AutoJudge`（bool；僅 **DA7** 顯示；對應勾選「自動判斷」）
    - *註：***C 型**可設定欄位較少時 **UI 可不顯示** `UseEquation2`、`FrontKnifePullOut` 等，但 **DB 仍可種子預設**，供日後擴充；與 Layout 一致。*
2. **目前選中的刀組合（定案）**
  - **不**建立獨立表 `KnifeCombinationOfBox`（此名稱廢止）。
  - **儲存位置**：**`PlcParameterValues`**（§7.5.5），透過 **`PlcParameterSetProfiles`** 已帶 **`BoxTypeId`**（及 `CorrugatedTypeId`）之 **Profile**，一種箱型對應一筆作用中 Profile。
  - **`PlcParameterDefinitions.Key`**：**`BoxParameters.SelectedKnifeCombinedOption`**（`ValueKind=INT`），**`ValueInt` = `KnifeCombinedOptions.Id`**（**0～8**）。Tab2 各箱型 ComboBox 綁定至**該箱型 Profile** 下此鍵之值。
  - **讀取鏈（供 F7／F1 下發）**：訂單（F7）帶 **箱型** → 解析對應 **Profile** → 讀 **`BoxParameters.SelectedKnifeCombinedOption`** → 必要時併同 **`DieCutter.*`**、**`PlcCommChannelSettings`** 等 → **F1 排程**選定訂單後，將應下發之 PLC 資料**打包**，經 **COM**（§7.5.3）送出。**下發格式／位址映射**屬後續 Phase，本節只定「資料來源與鍵」。

3. 連續參數（textbox）用「參數 Key」管理，落在 `PlcParameterValues`
  - `DieCutter.TrimValue` default：`3`
  - `DieCutter.BackBoardDensity` default：`1`
  - `DieCutter.BackBoardMaximum` default：`1600`
  - `DieCutter.DriveDensity` default：`0`
  - `DieCutter.ExpandParam` default：`30`
  - `DieCutter.SubmitWindParam` default：`0`
  - `DieCutter.PrintWindParam` default：`0`
  - `DieCutter.Phase2Offset` default：`100`
  - `DieCutter.ServerPrintOffset` default：`0`
  - `DieCutter.KnifeThresholdAdding` default：`20`
  - `DieCutter.ItemCountDefault` default：`3`
  - 車速表達格式（ComboBox；顯示文案須為**完整用語**，不簡寫）
    - `DieCutter.CarSpeed` default：`0` (ComboBox)
      - 選項 RESX：**「0 張/分鐘」**、**「1 張/小時」**（對應 `SettingsTab2_CarSpeed_Option0` / `Option1`）

#### 7.5.3 Tab3：通訊設定 + 元件/板卡參數（OtherPlcParameters）
> **Layout 對照**：`TODO_Phase7_Layout.md` Tab3 開頭 **「與主規 §7.5.3 章節對照」**（本檔 **§3～§6** ↔ 下述 **2.1** 等）。  
> 附圖 Tab3 **右側**版面（由上而下）：**其他通訊埠設定**（印刷機 PLC + 看板；GroupBox 標題 **RESX** `SettingsTab3_Group_OtherCommPorts`）→ **糊車 PLC**（**糊車 PLC 埠** + 串列參數）→ **使用者密碼管理**（`Users`；**RESX** `SettingsTab3_Group_UserPasswordManagement`）。**不**使用「**最佳化埠**」用語（舊 HMI 若曾分開，本專案併入 **糊車 PLC**）。  
> **左側**：依 **部門別（Feed、Print1…、All）** 篩選的 **DataGrid**（欄：**元件名稱**／最大值／最小值／精準度）。**欄標題**僅 **RESX**，**不可**當儲存格編輯。  
> **與 Tab1 區分**：Tab1（§7.5.1）為**刀位安全閾值**（刀模圖 16 鍵）；**部門參數 DataGrid** 屬 **Tab3**，數值鍵名前綴依部門（例 **`Feed.*`**）。

1. 通訊設定：建議合併成一張「可讀性高」的表
  - `PlcCommChannelSettings`（**固定三列**，`ChannelRole`：**三種** `Printer` / `Panel` / `PLC`）
      - **`Printer`**：**印刷機 PLC** 通訊埠（預設 **COM2**）
      - **`Panel`**：**看板**通訊埠（預設 **NONE**）
      - **`PLC`**：**糊車 PLC 埠** + **糊車**串列參數（**`PortName`** + **`BaudRate` / `ByteSize` / `Parity` / `StopBits`**；預設 **COM3**、**19200 / 8 / None / 1**，依機台可調）。UI 與文件稱呼**糊車 PLC 埠**，**不**另建 `Optimization` 角色、**不**在畫面上標「最佳化埠」。
    - `PortName`（`COM1`…`COMn` 或 `NONE`）
    - `BaudRate` (9600, 19200, 38400, 56700, 115200, 921600)
    - `ByteSize` (7 / 8)
    - `Parity`（None/Even/Odd）
    - `StopBits` (1 / 2)
    - `UpdatedAt`
  - **Printer / Panel**：UI 僅需 **Port** 下拉；**Baud/Byte/Parity/Stop** 可存預設 **`115200, 8, None, 1`**（與 Layout「非糊車用預設」一致），畫面可不顯示細項。
  - **PLC（糊車）**：UI 需 **Port + Baud + Byte + Parity + Stop**（與實體串列一致）。
  - default（與附圖 IMG_5954 對齊）：
    - Printer：`PortName=COM2`、`BaudRate=115200`、`ByteSize=8`、`Parity=None`、`StopBits=1`
    - Panel：`PortName=NONE`（與上列 **`PortName`** 約定一致：**無埠**時字串為 **`NONE`**，**勿**與 `None` 混用）、`BaudRate=115200`、`ByteSize=8`、`Parity=None`、`StopBits=1`
    - PLC（糊車）：`PortName=COM3`（**糊車 PLC 埠**，依機台）、`BaudRate=19200`、`ByteSize=8`、`Parity=None`、`StopBits=1`

1.1 Tab3 右側：**`Users` 密碼維護（定案：僅方案 A）**
  - **僅**維護 **`Users` 表**（與 Login 同一資料來源），**不**使用 `PlcParameterValues`／**不**在本 Tab 實作 `Security.*` 機台 PIN。
  - **僅 Admin** 可進入 Settings（§7.1），故**僅 Admin** 可操作本區：可修改 **所有使用者** 之密碼，包含 **Admin 自身** 與 **其他使用者**（欄位以現有 `Users` 為準，至少 `Username`／`Password`）。
  - **儲存後立即生效**：**F2**（或本區「儲存」）寫入 DB 後，**下次於 Login 輸入**即依 **DB** 驗證（**不重啟 App** 亦應讀到最新資料；實作時 Login 驗證必須查 DB，**不得**快取舊密碼）。
  - 實作備註：量產應避免密碼寫入 Log；種子僅供開發測試。

1.2 **已自 Layout 移除、改到 Tab5**：「**排單優先順序**」Group（含「是否觀看排單過程」、排序下拉、PcGap2/4 等）; **§7.5.4.1 Tab5** 有備註及參數 Key，但目前部落地，僅供日後參考

2. 元件/部門參數：同樣走「參數 Key」機制落在 `PlcParameterValues`

2.1 **Tab3 部門 DataGrid（元件列）**

**欄標題（不可編輯）**
- **「元件名稱」「最大值」「最小值」「精準度」** 等：**僅** **RESX**（例：`SettingsTab3_Column_ComponentName`、`SettingsTab3_Column_Max`…），**不**存入 DB、**不**允許使用者改標題文字。

**儲存格 — 元件顯示名稱（可編輯；定案：方案 B JSON）**
- **可編輯範圍**：**欄「元件名稱」下方每一列**（含選 **All** 時列出之每一列）。使用者僅改**顯示文字**；**程式識別用 `DepartmentCode` + `ComponentCode` 不變**（對應 `PlcParameterDefinitions`／數值 Key 之邏輯鍵亦不改）。
- **多語儲存**：**`ComponentDisplayNameOverrides`**（表名定案）
  - **複合主鍵**：`DepartmentCode`（`TEXT`，例：`Feed`、`Print1`…）+ `ComponentCode`（`TEXT`，例：`DriveSideBaffle`…）
  - **`DisplayNamesJson`**（`TEXT`，**Unicode／UTF-8**）：`{"zh-TW":"…","en":"…",…}`。**NULL**／**`{}`**：無覆寫 → 顯示該部門該列預設 **RESX**（Feed 已列 `SettingsTab3_Feed_Name_{ComponentCode}`；其他部門之預設 Key 規則於實作時與附圖對齊）。
- **讀寫**：C# **`System.Text.Json`** 序列化；**預設於程式內**解析 JSON（必要時再用 SQLite **JSON1** 查詢）。
- **與數值欄分離**：**Max／Min／Accurate** 仍走 **`PlcParameterValues`**（`Feed.*` 等）；本表**只**管顯示名稱覆寫。

**`PlcParameterDefinitions.NameKey` 與 Tab3「元件名稱」列（避免混淆）**
- **DataGrid 左欄列名**（可覆寫）：以 **`SettingsTab3_{DepartmentCode}_Name_{ComponentCode}`** 為準（Feed 例：`SettingsTab3_Feed_Name_DriveSideBaffle`；Print 例：`SettingsTab3_Print7_Name_LateralGap2` → 預設 **`Px7Gap2`**），詳見 **`TODO_Phase7_Layout.md`** Tab3。
- **`PlcParameterDefinitions.NameKey`**（§7.5.5）：綁在**單一邏輯鍵**（如 **`Print.7.LateralGap2.Max`**）上，多用於 **Max／Min／Accurate 分欄**、除錯或後台（種子例 **`SettingsTab3_Def_Print7_LateralGap2_Max`**，見 §7.5.6.1），**不等同**於左欄整列之**元件顯示名**。

**Feed 部門數值列（點選 Feed 時；`ComponentCode` 邏輯鍵不變）**

| 列序 | `ComponentCode`（程式/DB 邏輯鍵，英文） | `PlcParameterDefinitions.Key`（後綴皆為 `.Max` / `.Min` / `.Accurate`） | 附圖預設（Max, Min, Accurate） | 預設顯示字串（RESX：`SettingsTab3_Feed_Name_{ComponentCode}` 等） |
|------|----------------------------------------|------------------------------------------------------------------------|--------------------------------|-------------------------------------|
| 1 | `DriveSideBaffle` | `Feed.DriveSideBaffle.*` | 1350, 350, 1 | 驅動側擋板 |
| 2 | `OperatorSideBaffle` | `Feed.OperatorSideBaffle.*` | 1375, 375, 1 | 操作側擋板 |
| 3 | `RearBaffle` | `Feed.RearBaffle.*` | 1600, 340, 1 | 後擋板 |
| 4 | `FeedRollerGap` | `Feed.FeedRollerGap.*` | 10, 0, 0.2 | 進紙輪間隙 |
| 5 | `FrontBaffleGap` | `Feed.FrontBaffleGap.*` | 12, 0, 0.2 | 前擋板間隙 |
| 6 | `FeedPhase` | `Feed.FeedPhase.*` | 1272, 0, 1 | 送紙相位 |
| 7 | `CurrentSheetCount` | `Feed.CurrentSheetCount.*` | **99999**, **0**, **0** | 目前張數 |
| 8 | `OptimizationButton` | `Feed.OptimizationButton.*` | 0, 0, 0 | 最佳化按鈕 |

**對照範例：Feed 第 1 列（`ComponentCode` = `DriveSideBaffle`）**

> 說明：**一列** = 同一組 **`DepartmentCode` + `ComponentCode`**；**元件名稱**與 **Max／Min／Accurate 數值**分屬不同表／鍵，靠 **`ComponentCode`** 與 **Key 前綴 `Feed.DriveSideBaffle`** 對齊。

| 對象 | 角色 | 對應內容（範例值見上表「列 1」） |
|------|------|--------------------------------|
| **UI DataGrid** | 同一列 | 欄：**元件名稱**、**最大值**、**最小值**、**精準度** |
| **邏輯鍵** | 列識別 | `DepartmentCode` = **`Feed`**，`ComponentCode` = **`DriveSideBaffle`**（選 **All** 時此列仍標 **`Feed`**，**不**寫 `All`） |
| **元件名稱（左欄儲存格）** | **不**在 `PlcParameterValues` | **無覆寫**：RESX Key **`SettingsTab3_Feed_Name_DriveSideBaffle`** → **預設顯示字串（zh-TW）**「驅動側擋板」。**有覆寫**：表 **`ComponentDisplayNameOverrides`** 一筆（複合鍵 **`Feed` + `DriveSideBaffle`**），`DisplayNamesJson` 存各語系顯示字串。 |
| **最大值** | 數值 → `PlcParameterValues` | **`PlcParameterDefinitions.Key`** = **`Feed.DriveSideBaffle.Max`** → 以 `ParameterDefinitionId` 關聯 **`PlcParameterValues`**（`ProfileId` = 作用中 Profile），存 **1350**（型別依定義為 REAL／INT）。 |
| **最小值** | 同上 | Key = **`Feed.DriveSideBaffle.Min`** → 同 Profile 另一筆 **`PlcParameterValues`**，存 **350**。 |
| **精準度** | 同上 | Key = **`Feed.DriveSideBaffle.Accurate`** → 同 Profile 再一筆，存 **1**。 |

- **小結**：**3 筆** `PlcParameterValues`（同一 `ProfileId`、三個不同 `ParameterDefinitionId`）對應這一列的 **Max／Min／Accurate**；**元件名稱**僅對 **`DepartmentCode` + `ComponentCode`**，**不**佔用上述三筆數值列。

**`Print1`～`Print3`、`Print7`／`Print8` 部門數值列**

| 列序 | `ComponentCode` | `PlcParameterDefinitions.Key` （後綴 `.Max` / `.Min` / `.Accurate`）| 預設顯示字串（RESX：`SettingsTab3_Print{n}_Name_{ComponentCode}`；`Print{n}` 替換部門編號） |
|------|-----------------|--------------------------------|--------------------------------|
| 1 | `Register` | `Print.{n}.Register.*` | （依機台／附圖；舊例：**寄存器**類） |
| 2 | `PressAperture` | `Print.{n}.PressAperture.*` | **印壓間隙** |
| 3 | `Phase` | `Print.{n}.Phase.*` | **相位** |
| 4 | `LateralGap2` | `Print.{n}.LateralGap2.*` | **`Px{n}Gap2`**（**n=1～8** 與 **Print{n}** 一致；**不**另翻譯顯示名，見 `TODO_Phase7_Layout.md` Tab3 **§4**） |
| 5 | `ColorLateralShift` | `Print.{n}.ColorLateralShift.*` | **{n}色橫移**（Print7→**7色橫移**） |
| 6 | `LateralGap3` | `Print.{n}.LateralGap3.*` | **`Px{n}Gap3`**（**n=1～8**；**不**另翻譯顯示名，見 Layout Tab3 **§4**） |

> **預設顯示字串（RESX）**：**`LateralGap2`／`LateralGap3`** 列之預設字串為 **`Px{n}Gap2`**、**`Px{n}Gap3`**（**`n`** 與部門 **Print1～Print8** 一致），**不**強制翻譯；與 **`PlcParameterDefinitions.Key`**（**`Print.{n}.LateralGap2.*`** 等）為不同層次。

- **種子預設（依附圖，`n`=1～3 可分開）**：`PressAperture` 常為 Max **9**、Min **0.5～1.5**、Accurate **0.2**（**Print2** 附圖 Min **1.5**，**Print1／3** 有 **0.5**）；`Phase` 常為 **1272／0／1**；`ColorLateralShift` 常為 **10／-10／1**。若附圖**未**另列 **`Register`／`LateralGap2`／`LateralGap3`** 之數值，可採 **Max／Min／Accurate = 0/0/0** 作占位；**Print7／8** 已下詳列者**以下段為準**（避免與 **0/0/0** 並讀為矛盾）。

- **種子預設（沿用先前 Print7 數值；鍵名已統一）**（**`n=7`** 例；）：
  - `Print.7.Register.*`：`1277`／`0`／`0`
  - `Print.7.PressAperture.*`：`9`／`1.5`／`0.2`
  - `Print.7.Phase.*`：`1272`／`0`／`1`
  - `Print.7.LateralGap2.*`：`10`／`-10`／`0.2`
  - `Print.7.ColorLateralShift.*`：`10`／`-10`／`1`
  - `Print.7.LateralGap3.*`：`10`／`-10`／`0.2`
  - `Print.8.Register.*`：`1277`／`0`／`0`
  - `Print.8.PressAperture.*`：`9`／`1.5`／`0.2`
  - `Print.8.Phase.*`：`1272`／`0`／`1`
  - `Print.8.LateralGap2.*`：`10`／`-10`／`0.2`
  - `Print.8.ColorLateralShift.*`：`10`／`-10`／`1`
  - `Print.8.LateralGap3.*`：`10`／`-10`／`0.2`

> **Print8**：上列數值**與 Print7 相同**（**已確認**）；僅 Key 前綴為 **`Print.8.*`**。

> **PLC 閘道** 對應到上表；**DB 內** `PlcParameterDefinitions.Key` **一律**新鍵。

**`Other` 部門數值列**（點選 **Other** 時）：**`DepartmentCode` = `Other`**；**`ComponentCode`** 與 **`Component.*`** 鍵對照表見 **`TODO_Phase7_Layout.md` §6**（張捆、Bell 響鈴、連續送紙等 **6** 列）。

**盲點檢查（本節定案後）**

| 項目 | 狀態／說明 |
|------|------------|
| **All** 合併列 | **必含**：**Feed** → **Print1～3** → **Print7** → **Print8** → **Other**（順序見 `TODO_Phase7_Layout.md` Tab3 **§5**；**Other 必含**為定案）。 |
| **PLC 實體位址** (這暫時還沒有跟實際PLC結合驗證，目前就提供邏輯表可以) | 本文件只定 **邏輯鍵**；**位址表**另檔維護，避免與 HMI 鍵名混寫。 |


- **驗證（數值列）**：若 Max、Min 皆有值，建議 **`Min <= Max`**；`Accurate` 為**精準度／步進**（REAL）。顯示名覆寫**不**寫入 RESX Key 字串，只寫 **`DisplayNamesJson`**。

  - 先列出附圖/你原 TODO 可對應的主要 key（其餘若 UI 顯示但 key 不在這裡，後續補，應該都有了）：
    - `Component.NumberOfPaper.Bundle.Max` default：`999`
    - `Component.NumberOfPaper.Bundle.Min` default：`1`
    - `Component.NumberOfPaper.Bundle.Accurate` default：`0`
    - `Component.BellAlarm.Max` default：`NULL`
    - `Component.BellAlarm.Min` default：`0`
    - `Component.BellAlarm.Accurate` default：`0`
    - `Component.ContinueSubmittingPaper.Max` default：`99999`
    - `Component.ContinueSubmittingPaper.Min` default：`0`
    - `Component.ContinueSubmittingPaper.Accurate` default：`0`
    - `Component.CTKMaximumSpeed.Max` default：`NULL`
    - `Component.CTKMaximumSpeed.Min` default：`0`
    - `Component.CTKMaximumSpeed.Accurate` default：`0`
    - `Component.EarKnife.Max` default：`45`
    - `Component.EarKnife.Min` default：`0`
    - `Component.EarKnife.Accurate` default：`0`
    - `Component.AlreadyProduced.Max` default：`NULL`
    - `Component.AlreadyProduced.Min` default：`0`
    - `Component.AlreadyProduced.Accurate` default：`0`


> Tab3 左側「部門別」映射（與 `TODO_Phase7_Layout.md` 一致）：
> - **`Feed`**：DataGrid 綁定 **§7.5.3 2.1** 之 **`Feed.*`** 共 8 列（見上表）。
> - **`Print4`～`Print8`**：**Print4～6** 無種子可空白；**`Print1`～`Print3`**、**Print7／8** 各 **6** 列（**`Register`**、**`PressAperture`**、**`Phase`**、**`LateralGap2`**、**`ColorLateralShift`**、**`LateralGap3`**），見上表。
> - **`All`**：**合併**顯示（**Feed** → **Print1～3** → **Print7** → **Print8** → **Other**，**必含 Other**，見 **Layout Tab3 §5**）。**每一列**仍帶 **`DepartmentCode` + `ComponentCode`**（**不**使用 `DepartmentCode='All'` 存 DB）。顯示名覆寫**仍**寫入 **`ComponentDisplayNameOverrides`** 之對應 **`DepartmentCode`**。
> - **其餘部門**（Slotter、Knife、DieCut…）：**目前無對應種子資料** → DataGrid **空白**即可。
> - 服務層：`DepartmentCode -> Parameter Key 範圍` 固定映射表（`Print1..8` → `Index=1..8`）。

#### 7.5.4 Tab4：警報歷史（Alarm History）
> **Layout 本階段 Tab4 不實作內容**（見 `TODO_Phase7_Layout.md`）；下表 **`PlcAlarmHistory`** 仍可先建表，供日後 UI／查詢使用。  
> 此 Tab UI 主要是「查詢/顯示警報清單」與顯示 Alarm Count。

- `PlcAlarmHistory`
  - `Id`（PK，自增）
  - `AlarmTime`（DateTime）
  - `AlarmSource`（例如 PLC / RS485 / Panel）
  - `AlarmCode`（字串）
  - `AlarmText`（顯示用，可和 code 一致）
  - `Count`（可選：若 UI 有 Alarm Count 統計值）
  - `Severity`（Info/Warning/Error）
  - `CreatedAt`
- default：
  - Seed 插入若干筆（依你附圖顯示的時間區間先做假資料）

#### 7.5.4.1 Tab5：生產管理條件 + LCD 參數（ProductionManagement）
> 對應附圖：IMG_5951

> **自 Tab3 移入（本版 HMI 已移除 Tab3 區塊）**：「**排單優先順序**」Group（是否觀看排單過程、排序規則下拉、**開槽部刀蓋上升值**、**PcGap2 Up** / **PcGap4 Up** 等）— 參數 Key 待 Tab5 截圖定稿後補列（暫可預留 `Scheduling.*` 前綴）。

> 建議 Tab5 的所有 textbox/checkbox 值都用「參數 Key」落在 `PlcParameterValues`（避免又做大量固定欄位表）。

- `Running_Time_Condition.OverPiece` default：`5`
- `Running_Time_Condition.OverSpeed` default：`30`
- `Running_Time_Condition.KeepTime` default：`5`

- `Stop_Car_Condition.Overtime` default：`1`
- `Stop_Car_Condition.OverStopTime` default：`NULL`（附圖若空值）
- `Stop_Car_Condition.NoOutputTime` default：`0`

- `Change_Ordering.NotAllowChangeItems` default：`3`
- `Change_Ordering.ChangePermission` default：`false`

- `No_Paper_Submitting.OverTime` default：`0`
- `No_Paper_Submitting.AlarmTime` default：`30`

- `LCD_Parameter.EstimateSamplingTime` default：`1`
- `LCD_Parameter.InformationStayTime` default：`10`

- `Other.StandardSpeed` default：`80`
- `Other.StandardPrepareTime` default：`10`
- `Other.RestTimeFrom` default：`NULL`
- `Other.RestTimeOver` default：`NULL`

> 附圖右側「啟運PLC…/LCD顯示/啟動」相關控制項若需要落 DB：
> - 未對應 DB（待確認）：請你補更清晰截圖，我再把每個欄位對應到具體 parameter keys 或建立新 keys。

#### 7.5.5 PLC 參數值存儲（優化版：易讀性 + 強型別）
> 目標：把 7.5.1~7.5.3／7.5.4.1 中落在 **`PlcParameterValues`** 的「textbox/checkbox/radio／連續參數」集中管理，讓查詢/除錯更容易。  
> **不含**：**關聯表**（§7.5.2 之 `BoxTypes`、`KnifeCombinedOptions`、`KnifeCombinedOptionByBoxType` 等）、**警報列** `PlcAlarmHistory`（§7.5.4）、**使用者** `Users`、**元件顯示名覆寫** `ComponentDisplayNameOverrides`（§7.5.3：JSON 多語，非 Profile 數值）— 各表獨立，勿與本節三表混淆。  
> **刀組合選中**已併入 **`PlcParameterValues`**（鍵 **`BoxParameters.SelectedKnifeCombinedOption`**，§7.5.2 第 2 點），**不**另表。

1. `PlcParameterSetProfiles`（一組「紙箱設定檔」）
  - `ProfileId`（PK）
  - `BoxTypeId`（FK → `BoxTypes`）
  - `CorrugatedTypeId`（FK → **`CorrugatedTypes`**，見下段 **7.5.5.1**）
  - `ProfileName`（例：`Default_E_STD`，便於人工辨識）
  - `IsActive`（可選）
  - `CreatedAt`
  - `UpdatedAt`
  - unique：`UNIQUE(BoxTypeId, CorrugatedTypeId)`（**CorrugatedTypeId** 含 **預設材質** `Id=0`，見 **7.5.5.1**）

#### 7.5.5.1 瓦楞／板材厚度與 `CorrugatedTypeId`（定案）

> 現場**無**獨立「厚度設定畫面」，但**切割力道／速度**與**紙板材質、楞型、厚度**有關；**Profile** 仍須能區分「同一箱型、不同材質」時的參數組（含刀組合、日後可擴充之壓力係數等）。

**建議作法（單一路徑，避免無法對應 Profile）**

1. **`CorrugatedTypes`**（參照表，先簡後繁）
   - `Id`（PK，整數）
   - `Code`（`TEXT`，唯一，例：`Default`、`AFlute`、`BFlute`…）
   - `LabelKey`（RESX：顯示名，例：「標準／未指定」「A楞」）
   - 可選：`ThicknessMm`、`SortOrder`（**日後**與機台配方、PLC 對表）
2. **種子至少一筆 `Id = 0`**（**`Code = 'Default'`**）：表示**未指定材質／沿用單一預設剖面**；**所有** `PlcParameterSetProfiles` 種子與目前無厚度資訊之訂單，**一律**使用 **`CorrugatedTypeId = 0`** 對應 Profile。
3. **`PlcParameterSetProfiles`**：**不**使用 `NULL` 當 FK；**無**材質資訊時寫 **`CorrugatedTypeId = 0`**。
4. **訂單（F7）／排程（F1）**：若訂單**尚無**材質欄位 (有的,有個欄位叫**類型** (楞型)) → **`ResolveProfile(BoxTypeId)`** 內部固定 **`CorrugatedTypeId = 0`**；**未來**訂單若增加「楞型(**類型**)」→ 對應 **`CorrugatedTypes.Id`**，再查 **`GetProfile(BoxTypeId, CorrugatedTypeId)`**。
5. **力道／速度**：**短期**由 **`PlcParameterValues`** 內既有鍵（如 **`DieCutter.*`**、Tab1 **`KnifeSlot_*`**）在 **Profile(箱型, 類型(楞型))** 下給不同預設值；**長期**可新增 **`BoxParameters.CutForceScale`** 等鍵，**不**在 §7.5.5 另開平行表，避免與 Profile 脫鉤。

**Repository**：`GetProfile(BoxTypeId, int corrugatedTypeId = 0)` **預設** `0`。

2. `PlcParameterDefinitions`（參數定義：Key + 型別 + 預設值 + 顯示文字 key）
  - `ParameterDefinitionId`（PK）
  - `Key`（例如 `KnifeSlot_A_Min`、`Box.TrimValue`，唯一）
  - `TabGroup`（KnifeSafety / BoxParameters / OtherPlcParameters / ProductionManagement；**對應** §7.5.1～7.5.3 與 §7.5.4.1 之 **Key**，**不含** `PlcAlarmHistory` 列）
  - `ValueKind`（INT/REAL/BOOL/TEXT）
  - `NameKey`（RESX Key）：用於**該筆定義**（多為 **`.Max`/`.Min`/`.Accurate`** 單鍵）之標籤／除錯；**Tab3 DataGrid 左欄「元件名稱」** 仍以 **`SettingsTab3_{DepartmentCode}_Name_{ComponentCode}`**（§7.5.3 **2.1**）為準，**勿**將三筆 `NameKey` 當成同一列的「列標題」。
  - `Unit`（可選）
  - `SortOrder`（顯示排序）
  - 預設值（強型別欄位提升可讀性）：
    - `DefaultInt`
    - `DefaultReal`
    - `DefaultBool`
    - `DefaultText`

3. `PlcParameterValues`（某 profile 的實際值）
  - `ProfileId`（FK）
  - `ParameterDefinitionId`（FK）
  - typed 值欄位（只會有一種型別有效；用程式/檢核保證）：
    - `ValueInt`
    - `ValueReal`
    - `ValueBool`
    - `ValueText`
  - `UpdatedAt`
  - unique：`UNIQUE(ProfileId, ParameterDefinitionId)`

> 比起原本的 `ValueType + ValueText`，這套設計的好處：
> - 用 SQLite 查表時「值直接看得到型別」
> - 不用再解析字串去除錯
> - 以 RESX Key 做顯示更一致

#### 7.5.6 建表與種子資料（優化版：可驗證、可擴充、好維護）
- 建表腳本（建議拆開並明確版本號）：
  - `Scripts/CreatePlcSettingsTables.sql`
    - 參數定義/值（`PlcParameterDefinitions`、`PlcParameterValues`、`PlcParameterSetProfiles`）
    - 通訊（`PlcCommChannelSettings`）
    - 警報（`PlcAlarmHistory`；**Tab4 UI 本階段不實作**時，表仍可先建，**種子假資料**可選）
    - §7.5.2 離散/箱型關聯：`KnifeCombinedOptions`、`KnifeCombinedOptionByBoxType`、`KnifeSelectionModes`、`BoxTypes`、**`CorrugatedTypes`**（至少 `Id=0` **Default**，§7.5.5.1）。**刀組合選中**僅經 **`BoxParameters.SelectedKnifeCombinedOption`** + **`PlcParameterValues`**（§7.5.2）。
    - Tab3 元件顯示名覆寫：**`ComponentDisplayNameOverrides`**（`DepartmentCode`、`ComponentCode`、`DisplayNamesJson`，§7.5.3 2.1）
    - **`Users`**：若 Phase 2 已建表則**不重複建**，僅確保欄位滿足 Tab3 密碼維護（§7.5.3 **1.1**）
- Seed 腳本：
  - `Scripts/SeedPlcSettingsDefault.sql`
    - Seed `PlcParameterDefinitions` 的 Key + 預設值 + NameKey + ValueKind（§7.5.1、§7.5.2 **`BoxParameters.SelectedKnifeCombinedOption`**、`DieCutter.*`、§7.5.3 元件參數 Key、§7.5.4.1 等）
    - Seed `KnifeCombinedOptions`（**9 筆**）、`KnifeCombinedOptionByBoxType`、`BoxTypes`（C/E/DA7）等（§7.5.2）
    - Seed **`CorrugatedTypes`**：`Id=0`、`Code='Default'`（§7.5.5.1）
    - Seed **`PlcParameterSetProfiles`**：每 **箱型** 至少一筆 **`CorrugatedTypeId=0`**（與 §7.5.5.1 一致）
    - Seed profiles 的 `PlcParameterValues`：可採用「定義預設值」複製進值表；**每箱型 Profile** 含 **`BoxParameters.SelectedKnifeCombinedOption`**（INT **0～8**，對應預設刀組合）
    - **`ComponentDisplayNameOverrides`**：可種 **Feed** 部門 **8** 列（`DepartmentCode='Feed'` + 各 `ComponentCode`），`DisplayNamesJson` **NULL**／**`{}`**
    - Tab3 **`Feed.*`**／**`Print.{n}.*`** 之 **`PlcParameterDefinitions` 與 `PlcParameterValues` 種子 SQL 範本**（**新鍵版**）：見 **§7.5.6.1**
    - `PlcAlarmHistory`：可選（Tab4 延後時可略）
- default 數值規則：
  - 你附圖看得到的就填（含需要人工確認的註記）
  - 無法辨識的欄位用 `NULL`（不要亂填 0，避免之後下發 PLC 時誤判）

#### 7.5.6.1 種子 SQL 範本（Tab3 元件參數 · **新鍵版**）

> 目的：給 **`Scripts/SeedPlcSettingsDefault.sql`**（或等價遷移）一份 **可直接照抄結構** 的範本；**Key** 與 §7.5.3 **完全一致**（`Print.*` **已**用 **`LateralGap2`／`ColorLateralShift`／`LateralGap3`**，**禁止** `Px7Gap2`／`7ColorsOffset`／`Px7Gap3`）。  
> **假設**：`PlcParameterDefinitions` 已存在且欄位含 **`Key`（唯一）**、`TabGroup`、`ValueKind`、`NameKey`、`SortOrder`、**`DefaultInt`／`DefaultReal`**（依型別擇一）；`PlcParameterValues` 含 **`ProfileId`**、`ParameterDefinitionId`、`ValueInt`、`ValueReal`、…（僅示範 **REAL**；整數型請改 `ValueKind`/`ValueInt`）。實際欄位名以 **`Scripts/CreatePlcSettingsTables.sql`** 定稿為準。  
> **約定**：每個 **元件** 的 **最大值／最小值／精準度** 各為 **一筆** `PlcParameterDefinitions`（Key 後綴 **`.Max`**、**`.Min`**、**`.Accurate`**），與 §7.5.3 表格一致。  
> **`NameKey` 與 Tab3 列名**：下表 **`SettingsTab3_Def_Print7_LateralGap2_Max`** 等為**定義層**占位；**畫面左欄元件名** 仍依 §7.5.3 **`SettingsTab3_Print{n}_Name_{ComponentCode}`**（如 **`Px7Gap2`**），見 §7.5.3 **2.1** **「`PlcParameterDefinitions.NameKey` 與 Tab3「元件名稱」列」**。

**1）`PlcParameterDefinitions` 範例（節錄；其餘列依 §7.5.3 類推）**

```sql
-- TabGroup=OtherPlcParameters；ValueKind=REAL（Tab3 數值多為實數；若專案定為 INT 請改）
-- NameKey 可對應 RESX 或暫用占位，實作時再補齊

INSERT OR IGNORE INTO PlcParameterDefinitions
  ("Key", TabGroup, ValueKind, NameKey, SortOrder, DefaultReal)
VALUES
  -- Feed：DriveSideBaffle（附圖 1350 / 350 / 1）
  ('Feed.DriveSideBaffle.Max', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Feed_DriveSideBaffle_Max', 1001, 1350),
  ('Feed.DriveSideBaffle.Min', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Feed_DriveSideBaffle_Min', 1002, 350),
  ('Feed.DriveSideBaffle.Accurate', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Feed_DriveSideBaffle_Acc', 1003, 1),
  -- Print1：PressAperture / Phase / ColorLateralShift（附圖約 9 / 0.5 / 0.2；1272/0/1；10/-10/1）
  ('Print.1.PressAperture.Max', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print1_PressAperture_Max', 2001, 9),
  ('Print.1.PressAperture.Min', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print1_PressAperture_Min', 2002, 0.5),
  ('Print.1.PressAperture.Accurate', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print1_PressAperture_Acc', 2003, 0.2),
  ('Print.1.Phase.Max', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print1_Phase_Max', 2011, 1272),
  ('Print.1.Phase.Min', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print1_Phase_Min', 2012, 0),
  ('Print.1.Phase.Accurate', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print1_Phase_Acc', 2013, 1),
  ('Print.1.ColorLateralShift.Max', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print1_ColorShift_Max', 2021, 10),
  ('Print.1.ColorLateralShift.Min', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print1_ColorShift_Min', 2022, -10),
  ('Print.1.ColorLateralShift.Accurate', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print1_ColorShift_Acc', 2023, 1),
  -- Print1：Register / LateralGap2 / LateralGap3（§7.5.3；附圖未列時可 0/0/0 占位，與 Print2／3 類推補齊）
  ('Print.1.Register.Max', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print1_Register_Max', 2024, 0),
  ('Print.1.Register.Min', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print1_Register_Min', 2025, 0),
  ('Print.1.Register.Accurate', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print1_Register_Acc', 2026, 0),
  ('Print.1.LateralGap2.Max', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print1_LateralGap2_Max', 2027, 0),
  ('Print.1.LateralGap2.Min', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print1_LateralGap2_Min', 2028, 0),
  ('Print.1.LateralGap2.Accurate', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print1_LateralGap2_Acc', 2029, 0),
  ('Print.1.LateralGap3.Max', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print1_LateralGap3_Max', 2030, 0),
  ('Print.1.LateralGap3.Min', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print1_LateralGap3_Min', 2031, 0),
  ('Print.1.LateralGap3.Accurate', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print1_LateralGap3_Acc', 2032, 0),
  -- Print7：Register 起六列（§7.5.3 種子；新鍵名）
  ('Print.7.Register.Max', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print7_Register_Max', 7001, 1277),
  ('Print.7.Register.Min', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print7_Register_Min', 7002, 0),
  ('Print.7.Register.Accurate', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print7_Register_Acc', 7003, 0),
  ('Print.7.PressAperture.Max', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print7_PressAperture_Max', 7011, 9),
  ('Print.7.PressAperture.Min', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print7_PressAperture_Min', 7012, 1.5),
  ('Print.7.PressAperture.Accurate', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print7_PressAperture_Acc', 7013, 0.2),
  ('Print.7.Phase.Max', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print7_Phase_Max', 7021, 1272),
  ('Print.7.Phase.Min', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print7_Phase_Min', 7022, 0),
  ('Print.7.Phase.Accurate', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print7_Phase_Acc', 7023, 1),
  ('Print.7.LateralGap2.Max', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print7_LateralGap2_Max', 7031, 10),
  ('Print.7.LateralGap2.Min', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print7_LateralGap2_Min', 7032, -10),
  ('Print.7.LateralGap2.Accurate', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print7_LateralGap2_Acc', 7033, 0.2),
  ('Print.7.ColorLateralShift.Max', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print7_ColorShift_Max', 7041, 10),
  ('Print.7.ColorLateralShift.Min', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print7_ColorShift_Min', 7042, -10),
  ('Print.7.ColorLateralShift.Accurate', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print7_ColorShift_Acc', 7043, 1),
  ('Print.7.LateralGap3.Max', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print7_LateralGap3_Max', 7051, 10),
  ('Print.7.LateralGap3.Min', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print7_LateralGap3_Min', 7052, -10),
  ('Print.7.LateralGap3.Accurate', 'OtherPlcParameters', 'REAL', 'SettingsTab3_Def_Print7_LateralGap3_Acc', 7053, 0.2);
```
- **Print8**：再插入 **`Print.8.*`** 共 **18** 列（Key 與 **§7.5.3** 已列之 `Print.8.*` 一致），**`DefaultReal` 與 `Print.7.*` 對應鍵相同**（**已確認**）。
- **`Other`**：再插入 **`Component.NumberOfPaper.Bundle.*`**、**`Component.BellAlarm.*`**、**`Component.ContinueSubmittingPaper.*`**、**`Component.CTKMaximumSpeed.*`**、**`Component.EarKnife.*`**、**`Component.AlreadyProduced.*`**（各 **Max／Min／Accurate**；預設見 §7.5.3 與 **`TODO_Phase7_Layout.md` §6**）。

- **補齊**：`Feed` 其餘 **7** 列 `ComponentCode`、**Print2／Print3**（數值見 §7.5.3；**Print2** 之 `PressAperture.Min` 為 **1.5**）、**Print8**（鍵 **`Print.8.*`**，**數值種子與 Print7 相同**，**已確認**）、**`Other`** 之 **`Component.*`**（見上兩項）— 同樣 **每鍵三列** `.Max/.Min/.Accurate`。
- **`SortOrder`**：上表僅示例；實作可改為 **依部門**分段編號，便於除錯。

**2）`PlcParameterValues` 範例（寫入某一 `ProfileId`；以子查詢綁定 Definition）**

```sql
-- :profileId 為已存在之 PlcParameterSetProfiles.ProfileId（例：某箱型 + CorrugatedTypeId=0）
-- 若種子策略為「先 INSERT Definitions，再依 Default 複製到 Values」，可改為從 Definitions 讀 DefaultReal 批次插入

INSERT OR IGNORE INTO PlcParameterValues (ProfileId, ParameterDefinitionId, ValueReal, UpdatedAt)
SELECT :profileId, d.ParameterDefinitionId, d.DefaultReal, datetime('now')
FROM PlcParameterDefinitions d
WHERE d."Key" IN (
  'Feed.DriveSideBaffle.Max', 'Feed.DriveSideBaffle.Min', 'Feed.DriveSideBaffle.Accurate',
  'Print.1.PressAperture.Max', 'Print.1.PressAperture.Min', 'Print.1.PressAperture.Accurate', 'Print.1.Phase.Max', 'Print.1.Phase.Min', 'Print.1.Phase.Accurate',
  'Print.1.ColorLateralShift.Max', 'Print.1.ColorLateralShift.Min', 'Print.1.ColorLateralShift.Accurate',
  'Print.1.Register.Max', 'Print.1.Register.Min', 'Print.1.Register.Accurate',
  'Print.1.LateralGap2.Max', 'Print.1.LateralGap2.Min', 'Print.1.LateralGap2.Accurate',
  'Print.1.LateralGap3.Max', 'Print.1.LateralGap3.Min', 'Print.1.LateralGap3.Accurate'
  -- …其餘 Key 依 §7.5.3 全量列出，或改為 WHERE d.TabGroup = 'OtherPlcParameters' AND (d."Key" LIKE 'Feed.%' OR d."Key" LIKE 'Print.%')
);
```

**3）注意**

- **`INSERT OR IGNORE`**：依賴 **唯一索引**（`Key`）；若表無唯一約束，請改為 **`INSERT OR REPLACE`** 或先 `DELETE` 再種。
- **型別**：若 `Register` 等機台要求 **整數**，請將該鍵之 **`ValueKind`** 改為 **`INT`**、`DefaultInt`／`ValueInt` 種子，並在 `PlcParameterValues` 寫入對應欄位。
- **與 `ComponentDisplayNameOverrides`**：可另段 **INSERT** `DepartmentCode='Feed'` + `ComponentCode` 等，`DisplayNamesJson` NULL；**不**在本節重複。

---

### 7.6 Settings 頁底部按鈕（圖+字組合；優化：明確動作與 DB 行為）

#### 文案與圖示（定案）

- **文字**：五語系 **RESX**（`Settings_Footer_*`，詳見 §7.4 表），**不**在 XAML 寫死。
- **圖示**：靜態 **PNG**，**不**隨語系切換；檔案置於專案 **`src/Resources/Icons/`**（路徑等同 **`Resources\Icons`** 於方案中的位置，依建置／`Resource` 設定引用）。
- **版面**：每鍵為 **圖示 + 標籤文字**（左圖右文或專案慣例，與 Layout2 底部列一致即可）。

| F 鍵 | RESX Key（標籤文字） | 預設顯示字串（zh-TW） | 圖示檔（`src/Resources/Icons/`） |
|------|----------------------|------------------------|--------------------------------|
| **F1** | `Settings_Footer_F1_Exit` | F1 離開 | `icons8-exit-96.png` |
| **F2** | `Settings_Footer_F2_Save` | F2 儲存 | `icons8-save-as-80.png` |
| **F3** | `Settings_Footer_F3_Cancel` | F3 放棄修改 | `icons8-cancel-96.png` |
| **F5** | `Settings_Footer_F5_MachineDimension` | F5 機器尺寸 | `icons8-assembly-machine-80.png` |
| **F6** | `Settings_Footer_F6_SpecialParams` | F6 特殊參數區 | `icons8-open-end-wrench-96.png` |

> **版面**：左區 **F2→F5**、中左 **F6**、右區 **F3→F1**（**F1** 最右）。見 `SettingsView.xaml` 底部 `Grid` 三欄。

#### 行為（與 DB）

- `F2` **儲存**
  - 行為：驗證輸入 → 將 **Settings** 全域 **ViewModel** 中標記為 **Dirty** 的區塊**一次**寫入 DB（**跨 Tab**，**不**限當前可見分頁）
  - **建議**：單一 **transaction** 內提交：`PlcParameterValues`（作用中 **Profile**）+ **`PlcCommChannelSettings`**（§7.5.3）+ **`Users`**（若 Tab3 有變更）+ **`ComponentDisplayNameOverrides`**（若 Tab3 元件顯示名 JSON 有變更）；任一失敗則 **rollback**。
  - **不含**（本階段 **Tab4／Tab5 先不落地**／無對應 UI）：Tab4 `PlcAlarmHistory`、Tab5 專屬參數—**待該 Tab 實作並落地後**再納入 **F2** 與 **`SaveSettingsShell`** transaction（見 §7.7 **`IPlcSettingsService`**）；**落地前**請保留本註解以免漏改。
  - 寫入 Log（成功/失敗；**勿**記錄密碼明文）
- `F3` **放棄修改**
  - 行為：重新從 DB 讀取並覆蓋 **Profile 參數**、**通訊設定**、**Users**、`ComponentDisplayNameOverrides`（若上述已載入 ViewModel）
  - 清除 `IsDirty`
- `F5` **機器尺寸**（目前無功能）
  - TODO：若是「機械校正」類指令，先在 TODO 註記後續要串 PLC
- `F6` **特殊參數區**（目前無功能）
  - TODO：若會開啟第二層參數頁，需新增對應 View/TabGroup
- `F1` **離開**
  - 行為：**固定** 返回 **Login**（`NavigateToLogin()`），與 §7.1 一致

### 7.7 （後續接續用）讀取/修改/送 PLC 的接口規劃（優化：分層職責）
- Repository（資料存取層）
  - `IUserRepository`
    - `GetAllUsers()` / `UpdateUser(...)`（Tab3 **Admin** 維護密碼；Login **驗證**讀 **DB** 最新值；介面若已存在則擴充）
  - `IComponentDisplayNameRepository`
    - `LoadAll()` / `SaveAll(IReadOnlyList<ComponentDisplayNameOverride>)`（**`ComponentDisplayNameOverrides`**，§7.5.3）
  - `IPlcParameterProfileRepository`
    - `GetProfile(BoxTypeId, corrugatedTypeId = 0)`（**預設 0** = 未指定材質，§7.5.5.1）
    - `LoadValues(profileId, keys?)`
    - `SaveValues(profileId, values)`
  - `IPlcCommSettingsRepository`
    - `LoadAll()`：一次讀 **Printer / Panel / PLC** 三列
    - `SaveAll(...)`：一次寫三列（與 §7.6 **F2** **transaction** 一致）
  - `IPlcAlarmHistoryRepository`（**可延後**至 Tab4 實作）
    - `QueryLatest/QueryRange(...)`（先簡化：依固定筆數）
  - **箱型／刀組合主檔**（`BoxTypes`、`KnifeCombinedOptions`、`KnifeCombinedOptionByBoxType`）：**`IBoxAndKnifeCatalogRepository`**（唯讀或管理種子），**與** **`IPlcParameterProfileRepository`**（讀寫 **`BoxParameters.SelectedKnifeCombinedOption`** 於 **Profile**）職責分離
- Service（業務層）
  - `IPlcSettingsService`
    - `LoadSettingsShell()`：進入 Settings 時載入 **Profile 值 + Comm + Users + ComponentDisplayNameOverrides**
    - `SaveSettingsShell(...)`：對應 §7.6 **F2**（**transaction** 內含上列，**不含** Tab4／Tab5 尚未落地之區塊—與 §7.6 **F2**「先不落地」註解一致）
    - `Validate()`：檢查 `Min <= Max`、型別轉換失敗、`Feed.*`／`DieCutter.*`、**`BoxParameters.SelectedKnifeCombinedOption` ∈ [0,8]** 等
- PLC 下發（下一個 Phase 才做）
  - **輸入資料來源**：依 **F1** 選定之訂單 → 取得 **箱型** → **`CorrugatedTypeId`**（訂單有則用之(訂單有叫**類型**(目前有A、C、AB、E、B、BC)，先用預設0就好，因為目前不確定那些設定是給楞型使用，但你在7.5.5.1建議很好，可以先用像`DieCutter.*`為一個關係到楞型的數據。)，**無則 0**）→ **`PlcParameterSetProfiles`** → 讀 **`PlcParameterValues`**（含 **`BoxParameters.SelectedKnifeCombinedOption`**、**`DieCutter.*`** 等）+ **`PlcCommChannelSettings`**（COM 位址）。
  - **實作**：映射成 PLC **address + payload** 封包；**二進位格式**另訂規格。
- **與訂單模型**：見 **§7.5.5.1**；**無**材質欄位時 **`CorrugatedTypeId = 0`** 即與現有種子 Profile 對齊。

