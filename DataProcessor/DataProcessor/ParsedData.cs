using DataProcessor.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace DataProcessor
{
    public class ParsedData
    {
        public Row Header { get; set; }
        public Row Trailer { get; set; }
        public IList<Row> AllRows { get; set; }
        public IList<Row> DataRows { get; set; }
        public IList<Row> InvalidRows { get; set; }
        public IList<string> Errors { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValidationResultType ValidationResult { get; set; }
    }
}
