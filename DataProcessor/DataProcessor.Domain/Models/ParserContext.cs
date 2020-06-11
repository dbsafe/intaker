using System.Collections.Generic;

namespace DataProcessor.Domain.Models
{
    public class ParserContext
    {
        public string CurrentRowRaw { get; set; }
        public string[] CurrentRowRawFields { get; set; }
        public int CurrentRowIndex { get; set; }
        public bool IsCurrentRowTheLast { get; set; }
        public bool IsAborted { get; set; }
        public IList<string> Errors { get; set; } = new List<string>();
        public IList<Row> AllRows { get; set; } = new List<Row>();
        public IList<Row> DataRows { get; set; } = new List<Row>();
        public IList<Row> InvalidRows { get; set; } = new List<Row>();
        public Row Header { get; set; }
        public Row Trailer { get; set; }
        public ValidationResultType ValidationResult { get; set; }
    }
}
