using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace DataProcessor.Models
{
    public class Field
    {
        public string Raw { get; set; }
        public int Index { get; set; }
        [JsonIgnore]
        public Row Row { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValidationResultType ValidationResult { get; set; }
        public object Value { get; set; }

        public int AsInt()
        {
            return Convert.ToInt32(Value);
        }

        public bool AsBool()
        {
            return Convert.ToBoolean(Value);
        }

        public decimal AsDecimal()
        {
            return Convert.ToDecimal(Value);
        }

        public DateTime AsDateTime()
        {
            return Convert.ToDateTime(Value);
        }

        public string AsText()
        {
            return Convert.ToString(Value);
        }
    }
}
