# Phase 7 Settings 版面與資料對應（逐 Tab 控制項）

> 目的：把你附圖中的 Settings 控制項逐一列出，並對應到 `TODO_Phase7.md` 裡規劃的 DB Key/表。**Tab1～Tab3** 為目前詳列範圍；**Tab4、Tab5 本階段不實作內容**、子頁留白；**Tab6** 空白子頁；**Tab4～Tab6** 之 **分頁標題（tab title）仍須保留**，且 **RESX 多語系**（見 **Tab1～Tab6 附錄：分頁標題 RESX**）。
>
> 註記規則：
> - 若某個 UI 控制項在我目前能辨識的附圖中「看得到但無法確定對應 DB Key」，會標註 `未對應 DB（待確認）`
> - 若某個 UI 控制項「看不到/缺圖」，但依功能邏輯應該存在，我會標註 `可能存在（待你補截圖/文字）`

**與主規 `TODO_Phase7.md` §7.5.3 章節對照**（本檔 Tab3 小節 ↔ 主規）：

| 本檔（Tab3） | 主規 |
|--------------|------|
| **§3**（Feed 表） | §7.5.3 **2.1** Feed 部門表 |
| **§4**（`Print{n}`） | §7.5.3 **2.1** Print 部門表 |
| **§5**（`All` 合併） | §7.5.3 盲點檢查／部門映射 **All** |
| **§6**（`Other`） | §7.5.3 **`Other`** 部門與 `Component.*` |

## SettingsView 整體

- **頁面型態**：與 **Layout2**、**OrderMakingView** 相同，主內容為**一整頁**；**不**在內容區頂部另加「頁面 Title」列（例如不使用 `SettingsTab2_Title` 或各 Tab 的 `SettingsTabN_Title`）。識別依 **TabControl 分頁標籤**（僅需各 Tab 的 `Header` RESX，如 `SettingsTab2_TabHeader`）及 Shell 視窗標題（若專案有設定）。

1. `SettingsView` 內部：`TabControl`
   - Tab1：`SettingsTab1_KnifeSafetyView`（HMI 參考 IMG_5947；**刀模示意**見 `Box_Measurement_3-removebg-preview*.png`，切割/馬達安全閾值；**分頁標題 RESX** `SettingsTab1_TabHeader`，zh-TW：**5把刀安全設定**）
   - Tab2：`SettingsTab2_BoxParametersView`（附圖 IMG_5948；**分頁標題** `SettingsTab2_TabHeader`，zh-TW：**紙箱參數**）
   - Tab3：`SettingsTab3_OtherPlcParametersView`（通訊設定 + 部門別 DataGrid；**分頁標題** `SettingsTab3_TabHeader`，zh-TW：**其他參數**；**Feed** 列見 **IMG_5954**；**`Print{n}`／`Other`** 以本檔 **§4～§6** 定案為準，附圖僅供對照；其餘畫面參考 **IMG_5949**）
   - Tab4：`SettingsTab4_AlarmHistoryView` — **內容不實作**、留白；**分頁標題必留**，綁 **`SettingsTab4_TabHeader`**（RESX 五語系，**不**寫死）
   - Tab5：`SettingsTab5_ProductionManagementParametersView` — **內容不實作**、留白；**分頁標題必留**，綁 **`SettingsTab5_TabHeader`**（RESX 五語系，**不**寫死）
   - Tab6：`SettingsTab6_EmptyView` — **內容空白**；**分頁標題必留**，綁 **`SettingsTab6_TabHeader`**（RESX 五語系，**不**寫死；預設顯示字串見 **Tab1～Tab6 附錄：分頁標題 RESX**）
2. **底部按鈕區**（`SettingsView` 共用，**所有 Tab 共用**；**圖示 + RESX 標籤**）
   - 原 HMI 附圖（如 IMG_5947）若於**畫面右側**另有「類別設定」等按鈕，**本專案不採用該右側按鈕列**；日後若需同等功能，改由此**底部按鈕區**擴充（或另開 Dialog），避免 Tab 內容區被右欄壓縮。
   - **多語系**：按鈕**標籤文字**僅能綁 **RESX**（`Settings_Footer_*`）；**圖示**為靜態 **PNG**（`src/Resources/Icons/`），**不**隨語系切換。完整對照與行為見 **`TODO_Phase7.md` §7.4、§7.6**。
   - **定案（F 鍵／RESX／zh-TW／圖示檔）**：

   | F | RESX Key | 預設顯示字串（zh-TW） | 圖示檔（`src/Resources/Icons/`） |
   |---|----------|------------------------|----------------------------------|
   | F1 | `Settings_Footer_F1_Exit` | 離開 | `icons8-exit-96.png` |
   | F2 | `Settings_Footer_F2_Save` | 儲存 | `icons8-save-as-80.png` |
   | F3 | `Settings_Footer_F3_Cancel` | 放棄修改 | `icons8-cancel-96.png` |
   | F5 | `Settings_Footer_F5_MachineDimension` | 機器尺寸 | `icons8-assembly-machine-80.png` |
   | F6 | `Settings_Footer_F6_SpecialParams` | 特殊參數區 | `icons8-open-end-wrench-96.png` |

## Tab1：KnifeSafety（馬達運轉/切割閾值）
> **分頁標題（tab title）**：`SettingsTab1_TabHeader`，zh-TW 定案：**5把刀安全設定**（與 §7.5.1 馬達／刀位閾值內容對應；**不**在內容區重複加一層頁面 Title）。
> 對應：**HMI 畫面** IMG_5947（輸入框排版）；**刀模展開圖** `Box_Measurement_3-removebg-preview*.png`（尺寸語意與字母對照）。  
> **DB Key / 預設值**與 `TODO_Phase7.md` §7.5.1 **對齊（已確認）**。

### Tab1 刀模圖與參數語意（參考你提供的示意圖）
紙箱**展開圖（die-line）**上綠色字母標示主要尺寸，供 PLC 控制裁切／壓線路徑與安全閾值對照：

| 圖上字母 | 幾何意義（示意） | 對應 DB Key 前綴 |
|----------|------------------|------------------|
| **A** | 展開圖**整體水平寬度**（四主面板總寬） | `KnifeSlot_A_*` |
| **E** | **最左主面板**水平寬度 | `KnifeSlot_E_*` |
| （無字母） | **中間兩片主面板**各一條水平尺寸箭頭，圖上**未標字母** | 依專案約定命名為 **J、K**（由左而右）→ `KnifeSlot_J_*`、`KnifeSlot_K_*` |
| **F** | **最右主面板**水平寬度 | `KnifeSlot_F_*` |
| **B** | 展開圖**整體垂直高度**（含上下翼） | `KnifeSlot_B_*` |
| **H** | **上翼**垂直高度（頂部折翼） | `KnifeSlot_H_*` |
| **I** | **主身（中間直立區）**垂直高度 | `KnifeSlot_I_*` |
| （無字母） | 右下**下翼**垂直高度箭頭，圖上常與 **H 對稱**、未單獨標字母 | **本 Phase 未獨立 DB Key**；若日後要獨立監控可再新增 Key（目前不列入 16 個 TextBox） |

圖例補充（實作 UI 可當 Tooltip 或說明文字）：
- **實線**：裁切線；**紅色虛線**：摺線／壓線。
- Tab1 的 **MAX/MIN** 為 PLC **安全／允許範圍閾值**，與圖上「標註尺寸」語意對齊，實際數值仍以訂單與機台為準。

### Tab1 視覺區塊（版面）
1. **底圖**：使用去背 PNG **`Box_Measurement_3-removebg-preview.png`**。專案內建議路徑：`src/Resources/Icons/Box_Measurement_3-removebg-preview.png`（與你提供之刀模圖一致；若 Cursor assets 另有副本可只作備份）。WPF 以 **pack URI** 引用，例如 `pack://application:,,,/CursorTestApp;component/Resources/Icons/Box_Measurement_3-removebg-preview.png`；`Image` **Stretch** 建議 **Uniform** 或 **UniformToFill**，避免過度拉伸。
2. Tab1 其餘區域背景依整體 APP（白／淺灰等）與 **Settings** 風格一致。
3. **輸入區排版**（與刀模字母對照見上表）：
   - **上方橫列**（左→右）：`E、J、K、F` 四欄；每欄 **上 MAX / 下 MIN**（`KnifeSlot_E/J/K/F_*`）。**J、K** 對應刀模圖**頂部中間兩段水平尺寸**（圖上無字母處）。放在**底圖**上方
   - **底部**：`MAX A` / `MIN A`（`KnifeSlot_A_*`），對應刀模圖**最下方整體寬度 A**。放在**底圖**下方
   - **右側直列**（上→下）：`H、I、B` 三列；每列 **左 MAX / 右 MIN**（`KnifeSlot_H/I/B_*`），對應刀模圖右側 **H（上翼高）、I（主身高）、B（總高）**。放在**底圖**右方
4. 所有閾值均為可編輯 **TextBox**（數值）；樣式：**MAX** 藍底白字、**MIN** 紅底橘字（與既有 HMI 語意一致）。
5. **與原 HMI 附圖差異（右側按鈕取消）**  
   - IMG_5947 等舊畫面在**主內容右側**可能出現「類別設定」、捷徑圖示等按鈕列：**Tab1 不實作**於此位置。  
   - Tab1 **主內容**（刀模圖 + E/J/K/F + A + H/I/B 輸入區）可**水平延伸至頁面中間可用寬度**（無右欄按鈕預留）。  
   - 若日後需要「類別／個別設定」等行為：改放在 **SettingsView 底部按鈕區**（上列 §SettingsView 整體）或獨立視窗，**不**再放回 Tab1 右側。

### Tab1 控制項清單（逐一列出）與 DB Key
（以下 **16 個 TextBox** 對應 `PlcParameterValues` 的 Key，定義見 `PlcParameterDefinitions`）  
- **預設值**：與 `TODO_Phase7.md` §7.5.1 相同；§7.5.1 標為 `NULL` 者，UI 顯示**空白**，勿顯示字串 `"NULL"`。  
- **樣式**：MAX 用藍底白框白字；MIN 用紅底白框橘字（與附圖語意一致）。  
- **驗證**：同一刀位若 Min、Max 皆有輸入，儲存前檢查 **`Min <= Max`**。

1. TextBox（KnifeSlot_A）
   - `KnifeSlot_A_Max`（MAX A）
   - `KnifeSlot_A_Min`（MIN A）
2. TextBox（KnifeSlot_B）
   - `KnifeSlot_B_Max`（MAX B）
   - `KnifeSlot_B_Min`（MIN B）
3. TextBox（KnifeSlot_E）
   - `KnifeSlot_E_Max`（MAX E）
   - `KnifeSlot_E_Min`（MIN E）
4. TextBox（KnifeSlot_J）
   - `KnifeSlot_J_Max`（MAX J）
   - `KnifeSlot_J_Min`（MIN J）
5. TextBox（KnifeSlot_K）
   - `KnifeSlot_K_Max`（MAX K）
   - `KnifeSlot_K_Min`（MIN K）
6. TextBox（KnifeSlot_F）
   - `KnifeSlot_F_Max`（MAX F）
   - `KnifeSlot_F_Min`（MIN F）
7. TextBox（KnifeSlot_H）
   - `KnifeSlot_H_Max`（MAX H）
   - `KnifeSlot_H_Min`（MIN H）
8. TextBox（KnifeSlot_I）
   - `KnifeSlot_I_Max`（MAX I）
   - `KnifeSlot_I_Min`（MIN I）

### Tab1 Review 摘要（與 §7.5.1 一致與注意點）
| 項目 | 說明 |
|------|------|
| Key 覆蓋 | A、B、E、J、K、F、H、I 共 8 組 × Min/Max = **16 鍵**，與 §7.5.1 一一對應，無缺漏。 |
| J/K 與刀模圖 | 刀模圖僅標 **A、B、E、F、H、I**；**J/K** 為中間兩段水平寬度之**專案命名**，與 HMI 四欄 E/J/K/F 對齊。 |
| 圖上未單獨標字母之下翼高 | 與 **H** 對稱之尺寸若不需獨立監控，可不新增欄位；若需則另開 Key（本 Tab 不包含）。 |
| 預設 NULL | `E/J/K/F` 的 **Max** 為 `NULL` → 僅影響種子/UI 空白，不與數值 0 混淆。 |
| H/I Min | §7.5.1 為 **0**（非空白），與附圖「可為 0」一致。 |
| 右側按鈕（原附圖） | **取消**：不實作 HMI 右側按鈕列；Tab1 主內容區加寬。若需類似功能，改由 **Settings 底部共用按鈕區**擴充（見 §SettingsView 整體）。 |

## Tab2：DieCutter Parameters（箱型刀選擇 + 連續參數）
> 對應附圖：**IMG_5948**（參數設定／印刷模切相關）  
> **TabControl 分頁標籤**（僅此需要字串）：**紙箱參數**（RESX：`SettingsTab2_TabHeader`）。**不**使用 `SettingsTab2_Title` 或內容區頂部大標題；與 **Layout2、OrderMakingView** 相同，主內容為**一整頁**、**不**另加頁面 Title。  
> **DB**：`TODO_Phase7.md` §7.5.2；**多國語（定案）**：本 Tab **所有**使用者可見字串 — `TabControl` Header、各 **GroupBox.Header**、**Label**／**CheckBox**／**TextBlock**（含副標）、連續參數與車速 **Label**、**ComboBox** 項目（含車速兩選項）、**刀組合**顯示（`SettingsTab2_KnifeCombo_0`～`_8`）— **僅** 透過 **`SettingsTab2_*` RESX**，**不**在 XAML 寫死；數值 **TextBox** 內容為數字；**ToolTip** 若有亦需 Key。  
> 以下表格 **RESX Key** 請加入 `Resources*.resx`（至少 **zh-TW** 先填**預設顯示字串**（附圖參考），其餘語系可先用英文或占位）。

### Tab2 與 DB 對照原則
| 項目 | 說明 |
|------|------|
| **C 型** | 附圖僅 **開槽部咬紙退刀** + **刀組合 ComboBox**；`BoxTypes` 其餘欄位可種子保留，**本版 Layout 不顯示**。 |
| **E 型** | **開槽部咬紙退刀** + 全功能：刀組合、**使用資料庫選刀**、**原「E型箱設定」區塊**（公式與退刀）已**合併**至本 Group，**需**獨立「E型箱設定」GroupBox但放在此Group中。 |
| **D/A7 型** | **開槽部咬紙退刀** + **刀組合** + **自動判斷**；預設退刀 **0**（與 C/E 的 -5 不同，見 §7.5.2）。 |
| **取消的 UI** | 附圖「E型箱選擇刀」內之 **紙長**、其後多餘 **ComboBox／TextBox**（依你先前規格）**不實作**；DB 亦**不**強制新增「紙長」欄，除非日後業務補需求。 |
| **刀組合** | **共用主檔 9 筆** `KnifeCombinedOptions`（相異文案 9 種；附圖三區加總 11 **列**係 C/E 重複選項）。**依箱型**以 `KnifeCombinedOptionByBoxType` 篩選 ComboBox（C：Id **0,1,2**；E：**0,2,3,4,5**；DA7：**6,7,8**）。 |

### Tab2 版面（建議）
- **四欄等分**（左／中左／中右／右）：主內容 **Grid** 四個 `ColumnDefinition` 同寬；**右欄**先留白供日後擴充。
- **左欄**：上 **C型箱 選擇刀**、下 **D/A7型箱 選擇刀**，**上下排列**。
- **中左欄**：**E型箱 選擇刀**（含合併後的公式／退刀區）。**需**獨立「E型箱設定」GroupBox但放在此 **E型箱 選擇刀** Group 中；**VerticalAlignment=Top** 避免被右側長表單拉高。
- **中右欄**：**11 個連續參數**（`DieCutter.*`）+ **車速 ComboBox**（**不**再使用獨立「車速定義」GroupBox 外框，車速接在參數列表下方即可）。
- **右欄**：預留（透明 `Border` 等），不擺業務控制項。

---

### Tab2-A：箱型刀選擇區（`BoxTypes` + `KnifeCombinedOptions` + 關聯表）

#### A1. GroupBox — C 型箱 選擇刀（僅左欄上段）
| # | 控制項 | 綁定 / 資料來源 | RESX Key（建議） | 預設顯示字串（zh-TW／附圖參考） |
|---|--------|-----------------|------------------|------------------|
| — | `Header` | `BoxTypes` Code=`C` 之 `LabelKey` | `SettingsTab2_Group_C_SelectKnife` | C型箱 選擇刀 |
| 1 | `Label` + `TextBox` | `BoxTypes.KnifePullOut`（該列 C） | `SettingsTab2_Label_SlotBitePaperReturnKnife` | 開槽部咬紙退刀 |
| 2 | `ComboBox` | 選項來自 `KnifeCombinedOptions` ∩ `KnifeCombinedOptionByBoxType`（C）；選中值寫入 **`PlcParameterValues`**，鍵 **`BoxParameters.SelectedKnifeCombinedOption`**（`ValueInt` **0～8**，對應 **C 箱型**之 **Profile**） | 選項見 **§Tab2 附錄 B** | （選項文字） |

#### A2. GroupBox — E 型箱 選擇刀（中欄，含「E型箱設定」）
| # | 控制項 | 綁定 / 資料來源 | RESX Key（建議） | 預設顯示字串（zh-TW／附圖參考） |
|---|--------|-----------------|------------------|------------------|
| — | `Header` | `BoxTypes` Code=`E` | `SettingsTab2_Group_E_SelectKnife` | E型箱 選擇刀 |
| 1 | `Label` + `TextBox` | `BoxTypes.KnifePullOut`（E） | `SettingsTab2_Label_SlotBitePaperReturnKnife` | 開槽部咬紙退刀 |
| 2 | `ComboBox` | 刀組合（E：5 個可選）；選中寫入 **`PlcParameterValues`** **`BoxParameters.SelectedKnifeCombinedOption`**（**E** 箱型 **Profile**） | 選項 **§附錄 B** | — |
| 3 | `CheckBox` | `BoxTypes.UseDatabaseBlade`（E） | `SettingsTab2_Check_UseDatabaseBlade` | 使用資料庫選刀 |

#### A2.1 GroupBox — E型箱設定 （中欄，仍放在**GroupBox — E 型箱 選擇刀**中）
| 4 | `CheckBox` | `BoxTypes.UseEquation2` | `SettingsTab2_Check_UseFormula2` | 使用公式2計算 |
| 5 | `Label` + `TextBox` | `BoxTypes.FrontKnifePullOut` | `SettingsTab2_Label_E_FrontKnifeRetract` | E型箱前刀退刀 |
| 6 | `Label` + `TextBox` | `BoxTypes.BackKnifePullOut` | `SettingsTab2_Label_E_RearKnifeRetract` | E型箱後刀退刀 |
| 7 | `TextBlock`（副標，可用紅字或次級樣式） | 僅顯示 | `SettingsTab2_Subtitle_WithoutFormula2` | 不使用公式2計算 |
| 8 | `Label` + `TextBox` | `BoxTypes.BothKnifePullOut`（`UseEquation2=false` 時有意義） | `SettingsTab2_Label_SlotFrontBackRetractEach` | 開槽前後各退 |

#### A3. GroupBox — D/A7 型箱 選擇刀（左欄下段）
| # | 控制項 | 綁定 / 資料來源 | RESX Key（建議） | 預設顯示字串（zh-TW／附圖參考） |
|---|--------|-----------------|------------------|------------------|
| — | `Header` | `BoxTypes` Code=`DA7` | `SettingsTab2_Group_DA7_SelectKnife` | D/A7型箱 選擇刀 |
| 1 | `Label` + `TextBox` | `BoxTypes.KnifePullOut`（DA7，預設 0） | `SettingsTab2_Label_SlotBitePaperReturnKnife` | 開槽部咬紙退刀 |
| 2 | `ComboBox` | 刀組合（DA7，3 選項）；選中寫入 **`BoxParameters.SelectedKnifeCombinedOption`**（**DA7** **Profile**） | **§附錄 B** | — |
| 3 | `CheckBox` | `BoxTypes.AutoJudge` | `SettingsTab2_Check_AutoJudge` | 自動判斷 |

---

### Tab2-B：連續參數 + 車速（`PlcParameterValues` 之 `DieCutter.*`）
> **版面**：**中右欄（Grid 第 2 欄）**由上而下為第 **1～9** 項；**右欄（Grid 第 3 欄）**為第 **10～12** 項（含車速 Combo）。**車速**接在第 11 項之後，**無**「車速定義」獨立 GroupBox。

| # | 控制項 | DB Key（§7.5.2） | RESX Key（Label） | 預設顯示字串（zh-TW／附圖參考） |
|---|--------|------------------|-------------------|------------------|
| 1 | TextBox | `DieCutter.TrimValue` | `SettingsTab2_Label_TrimValue` | 修邊值 |
| 2 | TextBox | `DieCutter.BackBoardDensity` | `SettingsTab2_Label_BackBaffleTightness` | 後擋板緊密度 |
| 3 | TextBox | `DieCutter.BackBoardMaximum` | `SettingsTab2_Label_PaperFeedBackBaffleMax` | 送紙後擋板最大值 |
| 4 | TextBox | `DieCutter.DriveDensity` | `SettingsTab2_Label_DriveSideTightness` | 驅動側緊密度 |
| 5 | TextBox | `DieCutter.ExpandParam` | `SettingsTab2_Label_PaperFeedExtension` | 送紙延伸量參數 |
| 6 | TextBox | `DieCutter.SubmitWindParam` | `SettingsTab2_Label_PaperFeedAirVolume` | 送紙風量參數 |
| 7 | TextBox | `DieCutter.PrintWindParam` | `SettingsTab2_Label_PrintingAirVolume` | 印刷風量參數 |
| 8 | TextBox | `DieCutter.Phase2Offset` | `SettingsTab2_Label_DestroyPhase2Offset` | 破壞2相位偏差量 |
| 9 | TextBox | `DieCutter.ServerPrintOffset` | `SettingsTab2_Label_ServoPrintOffset` | 伺服印刷偏差值 |
| 10 | TextBox | `DieCutter.KnifeThresholdAdding` | `SettingsTab2_Label_SafetyBladeExtra` | 安全刀多加值 |
| 11 | TextBox | `DieCutter.ItemCountDefault` | `SettingsTab2_Label_DropCollectCountDefault` | 落下收料次數預設值 |
| 12 | ComboBox | `DieCutter.CarSpeed`（值 0／1） | `SettingsTab2_Label_CarSpeedFormat`（可選，作為列標題） | 車速（表達格式） |
| 12-選項 | `ComboBoxItem` | 值 `0` | `SettingsTab2_CarSpeed_Option0` | **0 張/分鐘**（完整用語，**不**用「張/分」等簡寫） |
| 12-選項 | `ComboBoxItem` | 值 `1` | `SettingsTab2_CarSpeed_Option1` | **1 張/小時**（完整用語） |

---

### Tab2 附錄 A：Tab 分頁標籤（僅 TabControl）
| RESX Key | 預設顯示字串（zh-TW；各語系以 RESX 為準） |
|----------|----------------------------------|
| `SettingsTab2_TabHeader` | **紙箱參數**（`TabControl` 該分頁 Header） |

- **不**使用 `SettingsTab2_Title`：內容區**不**顯示「參數設定」等獨立頁面標題列，與 **Layout2**、**OrderMakingView** 一致。
- 若 Shell 視窗標題需顯示「參數設定」，沿用既有全域／Shell 設定即可，**不**在本 Tab 內容再加一層 Title。

### Tab2 附錄 B：刀組合（**9 筆共用主檔** + `KnifeCombinedOptionByBoxType`）

#### 規劃要點（定案）
1. **`KnifeCombinedOptions` 僅種子 9 筆**（`Id` **0～8**），對應 **9 種相異顯示文案**（**預設顯示字串** zh-TW 見下表）。
2. **各箱型共用同一套選項**：C／E／DA7 的 ComboBox 皆從這 9 筆篩選；**不**再為「畫面上 11 列」另建 11 筆 PK。
3. 附圖 **C 3 列 + E 5 列 + D/A7 3 列 = 11 列**，其中 **Id 0、2** 同時出現在 **C 與 E**，故列數加總 > 9；**重複列顯示同一文案 → 共用同一 `DisplayTextKey`（同一 `KnifeCombinedOptionId`）**。
4. **`KnifeCombinedOptionByBoxType`**：規定各 `BoxTypeId` **可選的 `KnifeCombinedOptionId` 清單**（下表「箱型」欄）。UI：`ItemsSource = 主檔 ⋂ 關聯表篩選`。
5. **儲存**：各箱型目前選中項 → **`PlcParameterValues`**（**`BoxParameters.SelectedKnifeCombinedOption`**，`ValueInt` **0～8**），依該箱型 **`PlcParameterSetProfiles`** 寫入（見 `TODO_Phase7.md` §7.5.2）。

#### 主檔一覽（9 筆）
| `KnifeCombinedOptions.Id` | `Code`（建議） | `DisplayTextKey`（RESX，**全專案共用**） | 預設顯示字串（zh-TW／附圖參考） | 允許箱型（`KnifeCombinedOptionByBoxType`） |
|---------------------------|----------------|----------------------------------------|------------------------|---------------------------------------------|
| 0 | `0` | `SettingsTab2_KnifeCombo_0` | 0: 2,3刀 | **C、E** |
| 1 | `1` | `SettingsTab2_KnifeCombo_1` | 1: 2,4刀 | **C** |
| 2 | `2` | `SettingsTab2_KnifeCombo_2` | 2: 3,4刀 | **C、E** |
| 3 | `3` | `SettingsTab2_KnifeCombo_3` | 1: 2,3,4刀 | **E** |
| 4 | `4` | `SettingsTab2_KnifeCombo_4` | 3: 3,4,5刀 | **E** |
| 5 | `5` | `SettingsTab2_KnifeCombo_5` | 4: 1,5刀 | **E** |
| 6 | `6` | `SettingsTab2_KnifeCombo_6` | 0: 1,2,3刀 | **DA7** |
| 7 | `7` | `SettingsTab2_KnifeCombo_7` | 1: 1,2,4刀 | **DA7** |
| 8 | `8` | `SettingsTab2_KnifeCombo_8` | 2: 3,4,6刀 | **DA7** |

#### 列數 vs 9 筆（對照附圖）
- **C**：顯示 **Id 0,1,2** → 3 列。
- **E**：顯示 **Id 0,2,3,4,5** → 5 列（**0 與 2** 與 C 文案相同，仍為同一 `Id`／同一 RESX）。
- **DA7**：顯示 **Id 6,7,8** → 3 列。  
- **3+5+3=11 列** = 畫面列數；**資料主檔 = 9 筆**。

---

### Tab2 Review（Layout ↔ DB）
| 檢查項 | 結果 |
|--------|------|
| 刀組合主檔 | **9 筆** `KnifeCombinedOptions`（Id **0～8**），與附錄 B、§7.5.2 一致；**11 列**僅為畫面列數（C/E 重複 Id 0、2）。 |
| 選中刀組合儲存 | **`PlcParameterValues`** 鍵 **`BoxParameters.SelectedKnifeCombinedOption`**（**0～8**），**不**使用 `KnifeCombinationOfBox` 表。 |
| 連續參數 11 + 車速 1 | 與 §7.5.2 `DieCutter.*` 一致。 |
| C 少欄位 | DB `BoxTypes` 仍可有完整欄位；**Layout 僅顯示** 咬紙退刀 + Combo。 |
| E 合併區塊 | `UseEquation2`、`Front/Back/Both`、`UseDatabaseBlade` 與 §7.5.2 欄位對齊。 |
| DA7 `Auto` | `AutoJudge` 與 §7.5.2 對齊。 |
| 已取消 UI | **紙長**等不列出、不綁定。 |


## Tab3：OtherPlcParameters（通訊設定 + 元件/部門別參數）
> **分頁標題（tab title）**：`SettingsTab3_TabHeader`，zh-TW 定案：**其他參數**（通訊／部門 DataGrid 等仍屬本 Tab；**不**在內容區重複頁面 Title）。
> 對應附圖：**IMG_5954**～**IMG_5958**（各部門 DataGrid 列與數值；**Feed** + 右欄通訊／密碼）；**IMG_5949**（其餘 Tab3 畫面參考）

### Tab3 控制項清單（逐一列出）與 DB Key
- Tab3 **所有可見標籤／按鈕／欄位標題**皆走 **RESX**（`SettingsTab3_*`）。
- DataGrid **欄標題**（**「元件名稱」「最大值」「最小值」「精準度」**）：**僅** RESX（例：`SettingsTab3_Column_ComponentName`…），**不可編輯**。
- DataGrid **儲存格 — 元件名稱欄（每一列）**：
  - **可編輯**（含選 **All** 時列出之每一列）；僅改**顯示文字**，**列邏輯鍵** **`DepartmentCode` + `ComponentCode`** 不變。
  - **多語存 DB**：表 **`ComponentDisplayNameOverrides`**，**`DisplayNamesJson`**（方案 B），見 `TODO_Phase7.md` §7.5.3 2.1；無覆寫則顯示該部門該列之預設 RESX。

#### A. 左側：部門/元件選擇（Feed / Print1..8 / Slotter / Knife ...）
1. RadioBox：`DepartmentSelector`
   - 選項（**定案：與 All 合併順序對齊**；**Print1～Print8** 連續）：`Feed、Print1、Print2、Print3、Print4、Print5、Print6、Print7、Print8、Slotter、Knife、DieCut、Stack、Inline、Knife II、Other、All`（若實機 HMI 必須不同，以現場為準並註記於 `IMPL.md`）

2. （依選中的部門）**可編輯**的 Grid/表格：`ComponentParameterGrid`
   - 欄位：**元件名稱**（儲存格可編輯）／**最大值**／**最小值**／**精準度**（儲存格可編輯）（**欄標題**僅 RESX；數值對應 `PlcParameterValues`）
   - 顯示名覆寫：**`ComponentDisplayNameOverrides`**；數值：**`Feed.*`** 等（見下表與 §7.5.3）

3. **`Feed` 部門** — ComponentParameterGrid 列與 DB Key（與 `TODO_Phase7.md` §7.5.3 **2.1** 一致）

| # | `ComponentCode` | `PlcParameterDefinitions.Key`（`.Max` / `.Min` / `.Accurate`） | 附圖預設 (Max, Min, Accurate) | 預設顯示字串（RESX：`SettingsTab3_Feed_Name_{ComponentCode}`） |
|---|-----------------|------------------------------------------------------------------|-------------------------------|---------------------------|
| 1 | `DriveSideBaffle` | `Feed.DriveSideBaffle.*` | 1350, 350, 1 | 驅動側擋板 |
| 2 | `OperatorSideBaffle` | `Feed.OperatorSideBaffle.*` | 1375, 375, 1 | 操作側擋板 |
| 3 | `RearBaffle` | `Feed.RearBaffle.*` | 1600, 340, 1 | 後擋板 |
| 4 | `FeedRollerGap` | `Feed.FeedRollerGap.*` | 10, 0, 0.2 | 進紙輪間隙 |
| 5 | `FrontBaffleGap` | `Feed.FrontBaffleGap.*` | 12, 0, 0.2 | 前擋板間隙 |
| 6 | `FeedPhase` | `Feed.FeedPhase.*` | 1272, 0, 1 | 送紙相位 |
| 7 | `CurrentSheetCount` | `Feed.CurrentSheetCount.*` | 99999, 0, 0 | 目前張數 |
| 8 | `OptimizationButton` | `Feed.OptimizationButton.*` | 0, 0, 0 | 最佳化按鈕 |

> **附圖用字（Feed）**：**IMG_5954**／**IMG_5958** 若出現「檔／擋」差異，**同一 `ComponentCode`**；**RESX** 預設以 **驅動側擋板**、**操作側擋板** 為準。元件名稱欄**可編輯**時，現場可修正錯字，覆寫存 **`ComponentDisplayNameOverrides`**。

4. **`Print{n}` 部門（`n` ∈ {1,2,3,7,8}）— 定案鍵名（不再依附圖逐欄對齊）**

> **說明**：**`Print.{n}.*** 鍵名與 **`ComponentCode`** 已於本節與 `TODO_Phase7.md` §7.5.3 **定案**；**數值預設**、**種子**、**Print7／Print8** 詳細數值以 **`TODO_Phase7.md` §7.5.3** 為準。**附圖（IMG_5955～5957 等）**僅供歷史對照，**不**作為鍵名或列數的唯一依據。  
> **UI**：**Print1～3** 舊機可能**僅顯示 3 列**（`PressAperture`、`Phase`、`ColorLateralShift`）；**DB** 仍建 **6** 組鍵時，`Register`／`LateralGap2`／`LateralGap3` 可種 **0** 或由 §7.5.3 給預設，**隱藏列**與否由 ViewModel 依 `DepartmentCode` 設定。

| # | `ComponentCode` | `PlcParameterDefinitions.Key`（`.Max` / `.Min` / `.Accurate`） | 預設顯示字串（RESX：`SettingsTab3_Print{n}_Name_{ComponentCode}`） | 備註 |
|---|-----------------|------------------------------------------------------------------|----------------------------------------------------------------|------|
| 1 | `Register` | `Print.{n}.Register.*` | **Register**（可沿用英文；或依機台改 RESX） | **Print1～3** 舊畫面常**不顯示**；鍵可仍建 |
| 2 | `PressAperture` | `Print.{n}.PressAperture.*` | **印壓間隙**（**All** 若顯示「印刷間隙」→ **同一 `ComponentCode`**） | |
| 3 | `Phase` | `Print.{n}.Phase.*` | **相位** | |
| 4 | `LateralGap2` | `Print.{n}.LateralGap2.*` | **`Px{n}Gap2`**（**定案**：**不**另翻譯；**`{n}`=1～8** 與部門 **Print{n}** 一致） | 邏輯鍵仍 **`LateralGap2`**；**廢止**舊 DB 鍵名 **`Px7Gap2`** 見主規 |
| 5 | `ColorLateralShift` | `Print.{n}.ColorLateralShift.*` | **{n}色橫移** | **廢止** `7ColorsOffset` |
| 6 | `LateralGap3` | `Print.{n}.LateralGap3.*` | **`Px{n}Gap3`**（**定案**：**不**另翻譯；**`{n}`=1～8** 與部門 **Print{n}** 一致） | **勿**與 `LateralGap2` 混淆；**廢止**舊 DB 鍵名 **`Px7Gap3`** |

> **定案（`LateralGap2`／`LateralGap3` 預設顯示字串）**：**五語系**預設均以 **`Px{n}Gap2`**、**`Px{n}Gap3`** 為元件名稱字串（**`n` 代入 1～8**，與 **Print1～Print8** 對應；例：**Print7** → **Px7Gap2**、**Px7Gap3**）。**不**強制繁中／日文等翻譯；若某語系需加括號說明，僅寫在 RESX，**不**改 **`ComponentCode`**。

**列順序（單選 `Print{n}` 與 All 合併時）**：**Register** → **PressAperture** → **Phase** → **LateralGap2** → **ColorLateralShift** → **LateralGap3**。

5. **`All` 部門** — **合併**顯示（**不**寫入 `DepartmentCode='All'`；每列仍為實際部門鍵）

| 合併列順序（由上而下） | 說明 |
|------------------------|------|
| 1～8 | **Feed** 8 列（`Feed.*`） |
| 9～14 | **Print1**：`Register` → `PressAperture` → `Phase` → `LateralGap2` → `ColorLateralShift` → `LateralGap3`（`Print.1.*`） |
| 15～20 | **Print2** 同上（`Print.2.*`） |
| 21～26 | **Print3** 同上（`Print.3.*`） |
| 27～32 | **Print7** 同上（`Print.7.*`） |
| 33～38 | **Print8** 同上（`Print.8.*`）；**數值種子與 Print7 相同**（**已確認**） |
| 39～44 | **Other** 6 列（見 **§6**）；選 **All** 時**必含**本段（**定案**） |

6. **`Other` 部門** — `DepartmentCode` = **`Other`**（原「未分類」之 **`Component.*`** 鍵，統一掛此部門）

| # | `ComponentCode` | `PlcParameterDefinitions.Key`（`.Max` / `.Min` / `.Accurate`） | 預設顯示字串（RESX：`SettingsTab3_Other_Name_{ComponentCode}`） | 預設（見 `TODO_Phase7.md` §7.5.3） |
|---|-----------------|------------------------------------------------------------------|---------------------------------------------------------------|-------------------------------------|
| 1 | `NumberOfPaper.Bundle` | `Component.NumberOfPaper.Bundle.*` | **張捆** | Max 999／Min 1／Acc 0 |
| 2 | `BellAlarm` | `Component.BellAlarm.*` | **Bell 響鈴** | Max NULL／Min 0／Acc 0 |
| 3 | `ContinueSubmittingPaper` | `Component.ContinueSubmittingPaper.*` | **連續送紙** | 99999／0／0 |
| 4 | `CTKMaximumSpeed` | `Component.CTKMaximumSpeed.*` | **CTK 最高車速** | Max NULL／Min 0／Acc 0 |
| 5 | `EarKnife` | `Component.EarKnife.*` | **耳刀** | 45／0／0 |
| 6 | `AlreadyProduced` | `Component.AlreadyProduced.*` | **本班已生產張數**（與 §7.5.3 拼字 **Produced** 一致） | Max NULL／Min 0／Acc 0 |

- **說明**：邏輯鍵前綴仍為 **`Component.*`**（與既有 TODO 一致）；**部門別**選 **Other** 時，Grid 載入上表 **6** 列，**`DepartmentCode` 寫入 `Other`**（覆寫表亦同）。

**部門 → 資料列映射（定案）**
- **Feed**：**8** 列（`Feed.*`，見 **§3**）。
- **`Print1`～`Print3`、`Print7`、`Print8`**：各 **6** 列，鍵名見 **§4**（順序見 **§5**）。
- **`Print4`～`Print6`**：**無種子** → Grid **空白**（日後若與 **§4** 同結構再補）。
- **`Other`**：**6** 列（**§6**）。
- **`All`**：**合併**（**Feed** + **Print1～3** + **Print7** + **Print8** + **Other**，順序見 **§5**；**必含 Other**，**定案**）；**每列**仍標 **`DepartmentCode`**（**非** `All`）；覆寫鍵 **`(DepartmentCode, ComponentCode)`**。
- **其餘**（Slotter、Knife、DieCut、Stack、Inline、Knife II…）：**無種子** → Grid **空白**。

#### B. 右側（**由上而下**）：其他通訊埠設定 → 糊車 PLC → 使用者密碼管理
> **GroupBox 標題**：**不得**在 XAML 寫死任一語系文字；**一律**綁 **RESX Key**（`LocalizedString` / 資源字典），五語系與專案其他畫面相同。下表「**預設顯示字串（zh-TW）**」僅供種 RESX 時參考。
> DB 仍為 **`PlcCommChannelSettings` 三列**（`Printer` / `Panel` / `PLC`）；**畫面**上 **印／看** 與 **糊車** 分兩個 GroupBox，**不**出現「最佳化埠」文案。

1. **`SettingsTab3_Group_OtherCommPorts`**（RESX Key；**zh-TW 預設顯示字串**：**其他通訊埠設定**）
   - 子標籤「**印刷機 PLC**」「**看板**」等亦各綁 **RESX**（建議 `SettingsTab3_Label_PrinterPlcPort`、`SettingsTab3_Label_PanelPort` 等），**不**寫死。
   - **印刷機 PLC** `ComboBox` → `ChannelRole=Printer` 之 `PortName`
   - **看板** `ComboBox` → `ChannelRole=Panel` 之 `PortName`
   - 此二列之 Baud/Byte/Parity/Stop 存預設 **115200 / 8 / None / 1**（可不顯示於 UI）

2. **`SettingsTab3_Group_GluePlc`**（RESX Key；**zh-TW 預設顯示字串**：**糊車 PLC**）
   - 子標籤（**糊車 PLC 埠**、Baud、Byte、Parity、Stop）皆 **RESX**（建議 `SettingsTab3_Label_GluePlcPort`、`SettingsTab3_Label_BaudRate`…），**不**寫死。
   - **糊車 PLC 埠** `ComboBox` → `ChannelRole=PLC` 之 `PortName`（舊 HMI 若曾稱「最佳化」埠，本專案**一律**在此區標示為 **糊車 PLC 埠**）
   - **BaudRate** / **ByteSize** / **Parity** / **StopBits** `ComboBox` → 同列 `PLC`（附圖 **19200 / 8 / None / 1**）

3. **`SettingsTab3_Group_UserPasswordManagement`**（RESX Key；**zh-TW 預設顯示字串**：**使用者密碼管理**）
   - **定案（§7.5.3 1.1）**：**僅** 維護 **`Users` 表**；**Admin** 可修改 **所有使用者**（含 **Admin** 自身）之密碼等欄位。
   - **儲存**：與 **`F2`** 一併寫入 DB，**立即生效**；**Login** 驗證以 **DB** 為準（見 `TODO_Phase7.md` §7.6）。

**GroupBox 與綁定摘要**（標題皆為 **RESX**，下為 **zh-TW** **預設顯示字串**）
| 順序 | RESX Key | 預設顯示字串（zh-TW） | 控制項 | 綁定 |
|------|----------|------------|--------|------|
| 上 | `SettingsTab3_Group_OtherCommPorts` | 其他通訊埠設定 | `ComboBox` ×2 | `Printer`、`Panel` 之 `PortName` |
| 中 | `SettingsTab3_Group_GluePlc` | 糊車 PLC | `ComboBox` ×5 | `PLC`：`PortName` + `BaudRate` + `ByteSize` + `Parity` + `StopBits` |
| 下 | `SettingsTab3_Group_UserPasswordManagement` | 使用者密碼管理 | 依 `Users` 表設計 | **Admin** 編輯；**F2** 存檔 |

### Tab3 自附圖移除（改由他處負責）
- **系統語系**：不在 Tab3；由 **Login／全域語系**切換（`ILocalizationService`）。
- **排單優先順序**（含觀看排單過程、排序、PcGap 等）：**改 Tab5**；見 `TODO_Phase7.md` §7.5.4.1 補註。

### Tab3 Review（Layout ↔ `TODO_Phase7.md` §7.5.3）
| 檢查項 | 結果 |
|--------|------|
| Feed 8 列 | `Feed.{ComponentCode}.*`；**目前張數** **99999/0/0**；**All** 與單選 Feed **同一 `ComponentCode`** |
| **`Print{n}`** | **定案** **6** 鍵（§4）；**不**再依附圖逐欄；數值／種子以 **§7.5.3** 為準；**廢止**舊鍵名見主規 |
| **Other 6 列** | **`DepartmentCode=Other`** + **`Component.*`** 鍵（§6）；RESX **`SettingsTab3_Other_Name_*`** |
| **All 合併順序** | **Feed** → **Print1～3** → **Print7** → **Print8** → **Other**（§5；**必含 Other**） |
| 右欄版面 | **上→下** 三個 GroupBox，標題皆 **RESX**（**其他通訊埠設定** → **糊車 PLC** → **使用者密碼管理**）；印／看埠、糊車 PLC、`Users`。 |
| DB | `PlcCommChannelSettings` **三列**（`Printer`／`Panel`／`PLC`），與 §7.5.3 一致。 |
| 密碼 | **僅** `Users`；**Admin** 可改含自身；**F2** 後 **Login** 以 DB 驗證。 |
| 拼字 | `Component.AlreadyProduced.*`（**Produced**）。 |
| 舊 HMI | **語系**、**排單**已移出 Tab3 → **Tab5**（見 §7.5.4.1）。 |

### Tab3 附錄 C：元件名稱預設 **RESX** 命名（定案）

**附圖**：**IMG_5954**（**Feed**）、**IMG_5958**（**All**）等供視覺對照；**`Print{n}`** 與 **`Other`** 之鍵名、列數以 **§3～§6** 定案為準，**不**再要求與附圖逐欄一致。

| 部門（`DepartmentCode`） | 預設顯示字串（RESX 命名模式） | 說明 |
|--------------------------|----------------------|------|
| **Feed** | `SettingsTab3_Feed_Name_{ComponentCode}` | **8** 個 `ComponentCode`（§3）；**All** 中 Feed 段 **同一套 Key** |
| **Print1～Print3**、**Print7**、**Print8** | `SettingsTab3_Print{n}_Name_{ComponentCode}` | **6** 個 `ComponentCode`（§4）；**`ColorLateralShift`** 為 **「{n}色橫移」**；**`LateralGap2`**／**`LateralGap3`** **預設顯示字串** **不翻譯**，用 **`Px{n}Gap2`**／**`Px{n}Gap3`**（**n=1～8** 與 **Print{n}** 一致，§4）；**`Register`** 可英文 |
| **Other** | `SettingsTab3_Other_Name_{ComponentCode}` | **6** 個 `ComponentCode`（§6）：`NumberOfPaper.Bundle`、`BellAlarm`、`ContinueSubmittingPaper`、`CTKMaximumSpeed`、`EarKnife`、`AlreadyProduced` |
| **Print4～6** | 同 `SettingsTab3_Print{n}_Name_*` | **無種子** 前可不建 RESX；日後若與 **§4** 同 **6** 鍵再補 |
| **All** | （無獨立前綴） | 每列依 **`DepartmentCode`** 取 **Feed**／**Print{n}**／**Other** 對應之 **`SettingsTab3_*_Name_*`** |

**規則**：**`Print{n}`** 已定義者以本檔 **§4** 為準；**印壓／印刷間隙** 同 **`PressAperture`**。**預設顯示字串**：**`LateralGap2`／`LateralGap3`** 用 **`Px{n}Gap2`**／**`Px{n}Gap3`**（**n=1～8**）。**DB 邏輯鍵**仍 **`Print.{n}.LateralGap2`** 等；**廢止**舊 **DB** 鍵字串 **`Px7Gap2`**／**`7ColorsOffset`**／**`Px7Gap3`**（見 `TODO_Phase7.md` §7.5.3），與本節**畫面預設顯示字串** **`Px{n}Gap*`** 不同層次，勿混淆。



## Tab4：AlarmHistory（警報歷史）
> **本階段不實作內容**（子頁留白，供日後擴充）。
- **子頁**：仍使用 `SettingsTab4_AlarmHistoryView`，內容區**空白**即可（或僅一層 `Grid` 占位）。
- **分頁標題（tab title）— 必做、多語系**：`TabControl` 該項 **Header** **必須**綁定 **`SettingsTab4_TabHeader`**，五語系寫入 `Resources.*.resx`，**不**在 XAML 寫死。**zh-TW 預設顯示字串**（與舊 HMI 一致可調）：**歷史警報**。
- **詳細**控制項／DB／附圖 **IMG_5950**：見 **`TODO_Phase7.md` §7.5.4** — 日後要做 UI 時再從該節與本檔歷史版本對照補齊。

## Tab5：ProductionManagement（生產管理條件 + LCD 參數）
> **本階段不實作內容**（子頁留白，供日後擴充）。
- **子頁**：仍使用 `SettingsTab5_ProductionManagementParametersView`，內容區**空白**。
- **分頁標題（tab title）— 必做、多語系**：**Header** 綁定 **`SettingsTab5_TabHeader`**，五語系 RESX，**不**寫死。**zh-TW 預設顯示字串**：**生產管理參數**。
- **詳細**參數 Key、**自 Tab3 移入之排單區**、附圖 **IMG_5951**：見 **`TODO_Phase7.md` §7.5.4.1** — 日後實作時再對照。

### Tab1～Tab6 附錄：分頁標題 RESX（順序＝左欄分頁由上而下）
| RESX Key | 預設顯示字串（zh-TW；各語系以 RESX 為準） |
|----------|----------------------------------|
| `SettingsTab1_TabHeader` | **5把刀安全設定** |
| `SettingsTab2_TabHeader` | 紙箱參數 |
| `SettingsTab3_TabHeader` | **其他參數** |
| `SettingsTab4_TabHeader` | 歷史警報 |
| `SettingsTab5_TabHeader` | 生產管理參數 |
| `SettingsTab6_TabHeader` | **備份區** |

> **英文預設（`Resources.resx`）對照**：`Five-knife safety settings`／`Box parameters`／`Other parameters`／`Alarm history`／`Production management parameters`／`Backup area`。

## Tab6：EmptyView
- **子頁**：`SettingsTab6_EmptyView`，內容區**空白**（或僅一層 `Grid` 占位）。
- **分頁標題（tab title）— 必做、多語系**：**Header** 綁定 **`SettingsTab6_TabHeader`**，五語系 RESX，**不**寫死。**zh-TW 預設顯示字串**：**備份區**（見上表）。
- 不做其他 UI 控制項綁定。

