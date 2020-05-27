using System.Collections.Generic;

namespace DataProcessor.Domain.Models
{
    public class ParserContext
    {
        public string CurrentLine { get; set; }
        public int CurrentLineIndex { get; set; }
        public bool IsCurrentLineTheLastLine { get; set; }
        public IList<string> Errors { get; set; } = new List<string>();
        public IList<Row> Lines { get; set; } = new List<Row>();
        public IList<Row> InvalidLines { get; set; } = new List<Row>();
    }
}
