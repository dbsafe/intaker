using DataProcessor.InputDefinitionFile.Models;
using DataProcessor.Models;
using FileValidator.Blazor.Formatters;
using System.Collections.Generic;
using System.Linq;

namespace FileValidator.Blazor
{
    public abstract class TabulatorManager
    {
        protected readonly Dictionary<string, IFormatter> _formatters = new Dictionary<string, IFormatter>()
        {
            { nameof(DateTimeFormatter), new DateTimeFormatter() }
        };

        protected static List<object> BuildColumnInfo(RowDefinition rowDefinition)
        {
            var columnInfo = new List<object>
            {
                new { title = "Line Number", field = "LineNumber", headerSort = false },
                new { title = "Validation", field = nameof(Row.ValidationResult), headerSort = false }
            };

            var counter = 0;
            var headers = GetHeaderNames(rowDefinition.Fields);
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
    }
}
