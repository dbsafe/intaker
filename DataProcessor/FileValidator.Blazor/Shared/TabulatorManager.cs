﻿using DataProcessor.InputDefinitionFile.Models;
using DataProcessor.Models;
using FileValidator.Blazor.Formatters;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace FileValidator.Blazor
{
    public class TabulatorManager
    {
        private readonly IJSInProcessRuntime _js;
        private readonly string _id;
        private readonly RowDefinition _rowDefinition;

        private readonly Dictionary<string, IFormatter> _formatters = new Dictionary<string, IFormatter>()
        {
            { nameof(DateTimeFormatter), new DateTimeFormatter() }
        };

        public static TabulatorManager Init(IJSRuntime js, string id, IEnumerable<Row> rows, RowDefinition rowDefinition)
        {
            var table = new TabulatorManager(js, id, rowDefinition);
            table.Init(rows);
            return table;
        }

        public static TabulatorManager CreateDemoTable(IJSRuntime js, string id)
        {
            var table = new TabulatorManager(js, id, null);
            table._js.InvokeVoid("tabulator.initDemo", id);
            return table;
        }

        private TabulatorManager(IJSRuntime js, string id, RowDefinition rowDefinition)
        {
            _js = js as IJSInProcessRuntime;
            _id = id;
            _rowDefinition = rowDefinition;
        }

        private List<object> BuildColumnInfo()
        {
            var columnInfo = new List<object>
            {
                new { title = "Line Number", field = "LineNumber", headerSort = false },
                new { title = "Validation", field = nameof(Row.ValidationResult), headerSort = false }
            };

            var counter = 0;
            var headers = GetHeaderNames(_rowDefinition.Fields);
            foreach (var header in headers)
            {
                columnInfo.Add(new { title = header, field = $"field_{counter++}", headerSort = false });
            }

            columnInfo.Add(new { title = "Raw", field = nameof(Row.Raw), headerSort = false });

            return columnInfo;
        }

        private static IEnumerable<string> GetHeaderNames(IEnumerable<FieldDefinition> fieldDefinitions)
        {
            return fieldDefinitions.Select(a => string.IsNullOrWhiteSpace(a.UIName) ? a.Name : a.UIName);
        }

        private static List<object> BuildSubtableColumnInfo()
        {
            return new List<object>()
            {
                new { title = "Message Type", field = "messageType" },
                new { title = "Message", field = "message" }
            };
        }

        private List<object> BuildTableData(IEnumerable<Row> rows)
        {
            var tableData = new List<ExpandoObject>();
            foreach (var row in rows)
            {
                dynamic item = new ExpandoObject();

                item.LineNumber = row.Index + 1;
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
                    itemAsDictionary.Add($"field_{rowField.Index}", Format(rowField));
                }

                tableData.Add(item);
            }

            return tableData.Select(a => a as object).ToList();
        }

        private string Format(Field field)
        {
            if (field.Value == null)
            {
                return string.Empty;
            }

            var fieldDefinition = _rowDefinition.Fields[field.Index];
            if (string.IsNullOrWhiteSpace(fieldDefinition.UIFormatter))
            {
                return field.Value.ToString();
            }

            var formatter = GetFormatter(fieldDefinition.UIFormatter);

            return formatter.Format(field.Value, fieldDefinition.UIFormat);
        }

        private IFormatter GetFormatter(string name)
        {
            if (!_formatters.ContainsKey(name))
            {
                throw new InvalidOperationException($"Formatter '{name}' not found");
            }

            return _formatters[name];
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