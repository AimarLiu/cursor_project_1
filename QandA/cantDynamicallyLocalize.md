# 能否動態切換控制項的文字語系？

## 討論問題

專案已有 `LocalizationService`、`CultureChanged` 事件和語系選擇 ComboBox，想確認：**能否在執行時動態切換 UI 控制項顯示的文字語系？**

## 答案

**可以**，但需配合 Data Binding 與 `CultureChanged` 通知機制，而不是依賴 WPF 預設的 `x:Static` / `StaticResource`。

---

## 關鍵問題點

### 1. WPF 靜態資源只載入一次

- `x:Static`、`StaticResource` 在畫面載入時只解析一次
- 之後即使修改 `CultureInfo.CurrentUICulture`，已綁定的文字不會自動更新

### 2. 寫死在 XAML 的文字

- 若直接在 XAML 寫死文字（如 `Text="登入"`、`Content="Enter"`），與資源檔完全脫勾
- 語系切換無法影響這些控制項

### 3. 缺乏通知機制

- UI 需要「被告知」語系已變更，才能重新取得對應語系的字串並刷新顯示
- 若沒有透過 `INotifyPropertyChanged` 或類似機制通知，綁定不會更新

---

## 解法／作法

### 作法一：ViewModel 屬性 + `CultureChanged` 訂閱

1. 在 ViewModel 訂閱 `LocalizationService.CultureChanged`
2. 提供對應資源 key 的屬性，使用 `ILocalizationService.GetString(key)` 取值
3. 在 `CultureChanged` 處理函式中對這些屬性呼叫 `OnPropertyChanged`
4. XAML 以 Data Binding 綁定這些屬性

**ViewModel 範例：**

```csharp
_localizationService.CultureChanged += OnCultureChanged;

public string LoginTitle => _localizationService.GetString("LoginTitle") ?? "登入";
public string PasswordPlaceholder => _localizationService.GetString("PasswordPlaceholder") ?? "請輸入密碼";

private void OnCultureChanged(object? sender, CultureInfo culture)
{
    OnPropertyChanged(nameof(LoginTitle));
    OnPropertyChanged(nameof(PasswordPlaceholder));
    // 對所有需更新的屬性呼叫 OnPropertyChanged
}
```

**XAML 綁定範例：**

```xml
<TextBlock Text="{Binding LoginTitle}" ... />
<ui:PasswordBox PlaceholderText="{Binding PasswordPlaceholder}" ... />
```

### 作法二：包裝類 `LocalizedString`

建立可綁定的包裝類，綁定資源 key，並在 `CultureChanged` 時自動發送 `PropertyChanged`，減少各 ViewModel 重複程式碼。

```csharp
public class LocalizedString : INotifyPropertyChanged
{
    private readonly ILocalizationService _loc;
    private readonly string _key;
    public string Value => _loc.GetString(_key) ?? _key;
    // CultureChanged 時呼叫 PropertyChanged("Value")
}
```

---

## 必要條件摘要

| 條件 | 說明 |
|------|------|
| 字串出自 Resources | 所有語系文字應放在 `.resx`，以 key 取得 |
| 使用 Data Binding | UI 控制項以 Binding 取得文字，而非 `x:Static` 或寫死 |
| 語系變更時通知 | `CultureChanged` 時更新屬性並觸發 `PropertyChanged` |

滿足以上三點，即可實現 ComboBox 切換語系後，控制項文字即時更新。
