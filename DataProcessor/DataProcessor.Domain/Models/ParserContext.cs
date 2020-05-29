using System.Collections.Generic;

namespace DataProcessor.Domain.Models
{
    public class ParserContext
    {
        public string CurrentRowRaw { get; set; }
        public int CurrentRowIndex { get; set; }
        public bool IsCurrentRowTheLast { get; set; }
        public bool Abort { get; set; }
        public IList<string> Errors { get; set; } = new List<string>();
        public IList<Row> Rows { get; set; } = new List<Row>();
        public IList<Row> InvalidRows { get; set; } = new List<Row>();
    }
}
