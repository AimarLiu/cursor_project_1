# 登入密碼比對需取消隱藏才會正確

## 問題描述

使用 Wpf.Ui `PasswordBox` 時，密碼在隱藏（遮罩）狀態下登入比對失敗，只有取消隱藏（點擊眼睛圖示顯示明文）後才能登入成功。

## 原因分析

Wpf.Ui `PasswordBox` 有兩個與輸入相關的屬性，行為不同：

| 屬性 | 隱藏時 | 顯示時 |
|------|--------|--------|
| **Text** | 顯示遮罩字元（如 `*******`） | 顯示實際密碼 |
| **Password** | 儲存實際密碼 | 儲存實際密碼 |

原先綁定在 `Text`：

```xml
<ui:PasswordBox Text="{Binding Password, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
```

- **隱藏時**：`Text` 為遮罩字元，ViewModel 的 `Password` 收到 `*******` 而非實際密碼，比對失敗
- **顯示時**：`Text` 等同實際密碼，比對才會成功

## 修正方法

改為綁定 `Password` 屬性：

```xml
<ui:PasswordBox Password="{Binding Password, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
```

`Password` 無論隱藏或顯示都儲存實際密碼，ViewModel 可正確取得使用者輸入，登入比對即可正常運作。
