using System.Text;
using Microsoft.SqlServer.Management.Smo;

namespace DbScriptExporter;

public sealed class ExportSummary
{
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public int TotalCount => SuccessCount + FailureCount;
}

/// <summary>
/// 依物件類型走訪資料庫，將每個物件的指令碼輸出為獨立 .sql 檔。
/// </summary>
public sealed class DatabaseScriptExporter
{
    private readonly string _outputFolder;
    private readonly Scripter _scripter;

    public DatabaseScriptExporter(Server server, string outputFolder)
    {
        _outputFolder = outputFolder;
        _scripter = new Scripter(server)
        {
            Options = new ScriptingOptions
            {
                IncludeIfNotExists = true,
                Indexes = true,
                Triggers = true,
                DriPrimaryKey = true,
                DriForeignKeys = true,
                DriUniqueKeys = true,
                DriChecks = true,
                DriDefaults = true,
                ExtendedProperties = true,
                SchemaQualify = true,
                ToFileOnly = true,
                Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            },
        };
    }

    public ExportSummary Export(Database database)
    {
        var summary = new ExportSummary();

        ExportSchemas(database, summary);
        ExportTables(database, summary);
        ExportViews(database, summary);
        ExportStoredProcedures(database, summary);
        ExportUserDefinedFunctions(database, summary);
        ExportTriggers(database, summary);
        ExportUserDefinedTableTypes(database, summary);
        ExportUserDefinedDataTypes(database, summary);
        ExportSequences(database, summary);
        ExportSynonyms(database, summary);

        return summary;
    }

    private void ExportSchemas(Database database, ExportSummary summary)
    {
        foreach (Schema schema in database.Schemas)
        {
            if (schema.IsSystemObject)
            {
                continue;
            }

            var fileName = ScriptFileNameHelper.BuildFileName(schema.Name, schema.Name);
            ScriptObject("Schemas", "Schema", schema.Name, fileName, schema, summary);
        }
    }

    private void ExportTables(Database database, ExportSummary summary)
    {
        // 關閉 Triggers 選項，避免資料表指令碼內嵌觸發器定義，與 Triggers/ 資料夾的獨立檔案重複。
        _scripter.Options.Triggers = false;
        try
        {
            foreach (Table table in database.Tables)
            {
                if (table.IsSystemObject)
                {
                    continue;
                }

                var fileName = ScriptFileNameHelper.BuildFileName(table.Schema, table.Name);
                ScriptObject("Tables", "Table", $"{table.Schema}.{table.Name}", fileName, table, summary);
            }
        }
        finally
        {
            _scripter.Options.Triggers = true;
        }
    }

    private void ExportViews(Database database, ExportSummary summary)
    {
        foreach (View view in database.Views)
        {
            if (view.IsSystemObject)
            {
                continue;
            }

            var fileName = ScriptFileNameHelper.BuildFileName(view.Schema, view.Name);
            ScriptObject("Views", "View", $"{view.Schema}.{view.Name}", fileName, view, summary);
        }
    }

    private void ExportStoredProcedures(Database database, ExportSummary summary)
    {
        foreach (StoredProcedure procedure in database.StoredProcedures)
        {
            if (procedure.IsSystemObject)
            {
                continue;
            }

            var fileName = ScriptFileNameHelper.BuildFileName(procedure.Schema, procedure.Name);
            ScriptObject("StoredProcedures", "StoredProcedure", $"{procedure.Schema}.{procedure.Name}", fileName, procedure, summary);
        }
    }

    private void ExportUserDefinedFunctions(Database database, ExportSummary summary)
    {
        foreach (UserDefinedFunction function in database.UserDefinedFunctions)
        {
            if (function.IsSystemObject)
            {
                continue;
            }

            var fileName = ScriptFileNameHelper.BuildFileName(function.Schema, function.Name);
            ScriptObject("Functions", "Function", $"{function.Schema}.{function.Name}", fileName, function, summary);
        }
    }

    private void ExportTriggers(Database database, ExportSummary summary)
    {
        foreach (Table table in database.Tables)
        {
            if (table.IsSystemObject)
            {
                continue;
            }

            foreach (Trigger trigger in table.Triggers)
            {
                var fileName = ScriptFileNameHelper.BuildTriggerFileName(table.Schema, table.Name, trigger.Name);
                ScriptObject("Triggers", "Trigger", $"{table.Schema}.{table.Name}.{trigger.Name}", fileName, trigger, summary);
            }
        }
    }

    private void ExportUserDefinedTableTypes(Database database, ExportSummary summary)
    {
        foreach (UserDefinedTableType tableType in database.UserDefinedTableTypes)
        {
            var fileName = ScriptFileNameHelper.BuildFileName(tableType.Schema, tableType.Name);
            ScriptObject("TableTypes", "TableType", $"{tableType.Schema}.{tableType.Name}", fileName, tableType, summary);
        }
    }

    private void ExportUserDefinedDataTypes(Database database, ExportSummary summary)
    {
        foreach (UserDefinedDataType dataType in database.UserDefinedDataTypes)
        {
            var fileName = ScriptFileNameHelper.BuildFileName(dataType.Schema, dataType.Name);
            ScriptObject("DataTypes", "DataType", $"{dataType.Schema}.{dataType.Name}", fileName, dataType, summary);
        }
    }

    private void ExportSequences(Database database, ExportSummary summary)
    {
        foreach (Sequence sequence in database.Sequences)
        {
            var fileName = ScriptFileNameHelper.BuildFileName(sequence.Schema, sequence.Name);
            ScriptObject("Sequences", "Sequence", $"{sequence.Schema}.{sequence.Name}", fileName, sequence, summary);
        }
    }

    private void ExportSynonyms(Database database, ExportSummary summary)
    {
        foreach (Synonym synonym in database.Synonyms)
        {
            var fileName = ScriptFileNameHelper.BuildFileName(synonym.Schema, synonym.Name);
            ScriptObject("Synonyms", "Synonym", $"{synonym.Schema}.{synonym.Name}", fileName, synonym, summary);
        }
    }

    private void ScriptObject(string subFolder, string objectTypeLabel, string displayName, string fileName, SqlSmoObject smoObject, ExportSummary summary)
    {
        var folderPath = Path.Combine(_outputFolder, subFolder);
        Directory.CreateDirectory(folderPath);
        var filePath = Path.Combine(folderPath, fileName);

        try
        {
            _scripter.Options.FileName = filePath;
            _scripter.Script(new[] { smoObject.Urn });

            summary.SuccessCount++;
            Console.WriteLine($"[{subFolder}] {displayName} -> {subFolder}/{fileName}");
        }
        catch (Exception ex)
        {
            summary.FailureCount++;
            Console.WriteLine($"[WARN] 略過 {objectTypeLabel} {displayName}：{ex.Message}");
        }
    }
}
