using System;

namespace DataProcessor.Domain.Models
{
    public class Field
    {
        public string Raw { get; set; }
        public int Index { get; set; }
        public Row Row { get; set; }
        public ValidationResultType? ValidationResult { get; set; }
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
