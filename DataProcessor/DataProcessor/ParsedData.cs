using DataProcessor.Models;
using DataProcessor.Transformations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace DataProcessor
{
    public class ParsedData<TDataRow>
    {
        public Row Header { get; set; }
        public Row Trailer { get; set; }
        public IList<TDataRow> DataRows { get; set; }
        public IList<TDataRow> InvalidDataRows { get; set; }
        public IList<string> Errors { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValidationResultType ValidationResult { get; set; }
    }

    public class ParsedData10 : ParsedData<Row>
    {
    }

    public class ParsedData20 : ParsedData<DataRow20>
    {
        public IList<DataRow20> DataRowsWithInvalidTypes { get; set; }
    }
}
