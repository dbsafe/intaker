using System.Collections.Generic;

namespace DataProcessor.Domain.Models
{
    public class Row
    {
        public IList<Field> Fields { get; set; } = new List<Field>();
        public string[] RawFields { get; set; }
        public int Index { get; set; }
        public string Raw { get; set; }
        public ValidationResultType? ValidationResult { get; set; }
        public IList<string> Errors { get; set; } = new List<string>();
    }
}
