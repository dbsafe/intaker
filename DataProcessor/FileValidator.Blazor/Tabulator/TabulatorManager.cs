using DataProcessor.InputDefinitionFile.Models;
using DataProcessor.Models;
using FileValidator.Blazor.Formatters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace FileValidator.Blazor
{
    public abstract class TabulatorManager
    {
        protected static Dictionary<string, IFormatter> Formatters { get; } = new Dictionary<string, IFormatter>()
        {
            { nameof(DateTimeFormatter), new DateTimeFormatter() }
        };

        protected static List<object> ErrorsAndWarningsColumnInfo { get; } = new List<object>()
        {
            new { title = "Message Type", field = "messageType" },
            new { title = "Message", field = "message" }
        };

        protected static List<object> BuildColumnInfo(RowDefinition rowDefinition)
        {
            var columnInfo = new List<object>
            {
                new { title = "Line Number", field = "lineNumber", headerSort = false },
                new { title = "Validation", field = "validationResult", headerSort = false }
            };

            var counter = 0;
            var headers = GetHeaderNames(rowDefinition.Fields);
            foreach (var header in headers)
            {
                columnInfo.Add(new { title = header, field = $"field_{counter++}", headerSort = false });
            }

            columnInfo.Add(new { title = "Raw", field = "raw", headerSort = false });

            return columnInfo;
        }

        protected static ExpandoObject GetExpandoObjectFromRow(Row row, RowDefinition rowDefinition)
        {
            dynamic item = new ExpandoObject();

            item.lineNumber = row.Index + 1;
            item.validationResult = row.ValidationResult.ToString();
            item.raw = row.Raw;

            if (row.Warnings.Count > 0 || row.Errors.Count > 0)
            {
                var errorsAndWarnings = new List<ExpandoObject>();
                item.errorsAndWarnings = errorsAndWarnings;

                AddErrorsAndWarnings(row.Errors, "Error", errorsAndWarnings);
                AddErrorsAndWarnings(row.Warnings, "Warning", errorsAndWarnings);
            }

            AddFieldsValuesFromRow(item, row, rowDefinition);
            return item;
        }

        private static void AddFieldsValuesFromRow(ExpandoObject item, Row row, RowDefinition rowDefinition)
        {
            var itemAsDictionary = item as IDictionary<string, object>;
            foreach (var rowField in row.Fields)
            {
                itemAsDictionary.Add($"field_{rowField.Index}", Format(rowField, rowDefinition));
            }
        }

        private static void AddErrorsAndWarnings(IList<string> messages, string messageType, List<ExpandoObject> errorsAndWarnings)
        {
            foreach (var message in messages)
            {
                dynamic errorItem = new ExpandoObject();
                errorItem.message = message;
                errorItem.messageType = messageType;
                errorsAndWarnings.Add(errorItem);
            }
        }

        private static string Format(Field field, RowDefinition rowDefinition)
        {
            if (field.Value == null)
            {
                return string.Empty;
            }

            var fieldDefinition = rowDefinition.Fields[field.Index];
            if (string.IsNullOrWhiteSpace(fieldDefinition.UIFormatter))
            {
                return field.Value.ToString();
            }

            var formatter = GetFormatter(fieldDefinition.UIFormatter);

            return formatter.Format(field.Value, fieldDefinition.UIFormat);
        }

        private static IEnumerable<string> GetHeaderNames(IEnumerable<FieldDefinition> fieldDefinitions)
        {
            return fieldDefinitions.Select(a => string.IsNullOrWhiteSpace(a.UIName) ? a.Name : a.UIName);
        }

        private static IFormatter GetFormatter(string name)
        {
            if (!Formatters.ContainsKey(name))
            {
                throw new InvalidOperationException($"Formatter '{name}' not found");
            }

            return Formatters[name];
        }
    }
}
