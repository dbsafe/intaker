using System.Collections.Generic;

namespace DataProcessor.Domain.Models
{
    public class Row
    {
        public IList<Field> Fields { get; set; }
        public int LineIndex { get; set; }
        public string Line { get; set; }
        public bool IsValid { get; set; }
        public IList<string> Errors { get; set; } = new List<string>();
    }
}
