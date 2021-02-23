using DataProcessor.InputDefinitionFile.Models;
using DataProcessor.Models;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Dynamic;

namespace FileValidator.Blazor
{
    public class TabulatorManager10 : TabulatorManager
    {
        private readonly IJSInProcessRuntime _js;
        private readonly string _id;
        private readonly RowDefinition _rowDefinition;

        public static TabulatorManager10 Init(IJSRuntime js, string id, IEnumerable<Row> rows, RowDefinition rowDefinition)
        {
            var table = new TabulatorManager10(js, id, rowDefinition);
            table.Init(rows);
            return table;
        }

        private TabulatorManager10(IJSRuntime js, string id, RowDefinition rowDefinition)
        {
            _js = js as IJSInProcessRuntime;
            _id = id;
            _rowDefinition = rowDefinition;
        }

        private void Init(IEnumerable<Row> rows)
        {
            var tableModel = BuildTableDataModel(rows, _rowDefinition);
            _js.InvokeVoid("tabulator.init10", _id, tableModel, ErrorsAndWarningsColumnInfo);
        }

        private static object BuildTableDataModel(IEnumerable<Row> rows, RowDefinition rowDefinition)
        {
            var tableData = new List<ExpandoObject>();
            foreach (var row in rows)
            {
                dynamic item = GetExpandoObjectFromRow(row, rowDefinition);
                tableData.Add(item);
            }

            var columnInfo = BuildColumnInfo(rowDefinition);

            return new { tableData, columnInfo };
        }
    }
}
