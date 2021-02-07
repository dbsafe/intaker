using DataProcessor.Models;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace FileValidator.Blazor
{
    public class TabulatorManager
    {
        private readonly IJSInProcessRuntime _js;
        private readonly string _id;
        private readonly IEnumerable<string> _columnHeaders;

        public static TabulatorManager Init(IJSRuntime js, string id, IEnumerable<Row> rows, IEnumerable<string> columnHeaders)
        {
            var table = new TabulatorManager(js, id, columnHeaders);
            table.Init(rows);
            return table;
        }

        public static TabulatorManager CreateDemoTable(IJSRuntime js, string id)
        {
            var table = new TabulatorManager(js, id, null);
            table._js.InvokeVoid("tabulator.initDemo", id);
            return table;
        }

        private TabulatorManager(IJSRuntime js, string id, IEnumerable<string> columnHeaders)
        {
            _js = js as IJSInProcessRuntime;
            _id = id;
            _columnHeaders = columnHeaders;
        }

        private List<object> BuildColumnInfo()
        {
            var columnInfo = new List<object>
            {
                new { title = "Validation", field = nameof(Row.ValidationResult), headerSort = false }
            };

            var counter = 0;
            foreach (var _columnHeader in _columnHeaders)
            {
                columnInfo.Add(new { title = _columnHeader, field = $"field_{counter++}", headerSort = false });
            }

            columnInfo.Add(new { title = "Raw", field = nameof(Row.Raw), headerSort = false });

            return columnInfo;
        }

        private static List<object> BuildSubtableColumnInfo()
        {
            return new List<object>()
            {
                new { title = "Message Type", field = "messageType" },
                new { title = "Message", field = "message" }
            };
        }

        private static List<object> BuildTableData(IEnumerable<Row> rows)
        {
            var tableData = new List<ExpandoObject>();
            foreach (var row in rows)
            {
                dynamic item = new ExpandoObject();

                item.ValidationResult = row.ValidationResult.ToString();
                item.Raw = row.Raw;

                if (row.Warnings.Count > 0 || row.Errors.Count > 0)
                {
                    var subtableData = new List<object>();
                    item.subtabledata = subtableData;

                    foreach (var error in row.Errors)
                    {
                        subtableData.Add(new { Message = error, MessageType = "Error" });
                    }

                    foreach (var warning in row.Warnings)
                    {
                        subtableData.Add(new { Message = warning, MessageType = "Warning" });
                    }
                }

                var itemAsDictionary = item as IDictionary<string, object>;
                foreach (var rowField in row.Fields)
                {
                    itemAsDictionary.Add($"field_{rowField.Index}", rowField.Value);
                }

                tableData.Add(item);
            }

            return tableData.Select(a => a as object).ToList();
        }

        private void Init(IEnumerable<Row> rows)
        {
            var columnInfo = BuildColumnInfo();
            var subtableColumnInfo = BuildSubtableColumnInfo();
            var tableData = BuildTableData(rows);

            _js.InvokeVoid("tabulator.init", _id, tableData, columnInfo, subtableColumnInfo);
        }
    }
}
