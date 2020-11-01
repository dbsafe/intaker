using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace DataProcessor.Domain.Models
{
    public class Row
    {
        public int Index { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValidationResultType ValidationResult { get; set; }
        public string Raw { get; set; }
        public string[] RawFields { get; set; }
        public IList<Field> Fields { get; set; } = new List<Field>();
        public IList<string> Errors { get; set; } = new List<string>();
        public IList<string> Warnings { get; set; } = new List<string>();
        [JsonIgnore]
        public string Json { get; set; }
    }
}
