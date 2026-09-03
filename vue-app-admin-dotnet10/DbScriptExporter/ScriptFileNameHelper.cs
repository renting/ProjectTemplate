using System.Text;

namespace DbScriptExporter;

/// <summary>
/// 產生符合檔名規則的 .sql 檔名，並過濾檔名中的不合法字元。
/// </summary>
public static class ScriptFileNameHelper
{
    private static readonly char[] InvalidChars = Path.GetInvalidFileNameChars();

    /// <summary>
    /// 一般物件檔名規則：{Schema}.{ObjectName}.sql
    /// </summary>
    public static string BuildFileName(string schema, string objectName)
        => $"{Sanitize(schema)}.{Sanitize(objectName)}.sql";

    /// <summary>
    /// Trigger 檔名規則：{Schema}.{TableName}.{TriggerName}.sql
    /// </summary>
    public static string BuildTriggerFileName(string schema, string tableName, string triggerName)
        => $"{Sanitize(schema)}.{Sanitize(tableName)}.{Sanitize(triggerName)}.sql";

    private static string Sanitize(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var c in value)
        {
            builder.Append(Array.IndexOf(InvalidChars, c) >= 0 ? '_' : c);
        }

        return builder.ToString();
    }
}
