namespace CursorTestApp.Helpers;

/// <summary>
/// 將 Exception 格式化為可顯示字串，包含內部例外（InnerException）與堆疊。
/// </summary>
public static class ExceptionFormatHelper
{
    /// <summary>
    /// 產生完整錯誤訊息：外層 Message、所有 InnerException 的 Message，以及最內層的 StackTrace。
    /// </summary>
    public static string ToDisplayString(Exception ex)
    {
        if (ex == null) return string.Empty;

        var sb = new System.Text.StringBuilder();
        var current = ex;
        var depth = 0;

        while (current != null)
        {
            if (depth > 0)
                sb.AppendLine($"--- 內部例外 (InnerException #{depth}) ---");
            sb.AppendLine(current.Message);
            if (!string.IsNullOrEmpty(current.StackTrace))
            {
                sb.AppendLine("StackTrace:");
                sb.AppendLine(current.StackTrace);
                sb.AppendLine();
            }
            current = current.InnerException;
            depth++;
        }

        return sb.ToString();
    }
}
