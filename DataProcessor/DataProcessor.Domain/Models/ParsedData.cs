using System.Collections.Generic;

namespace DataProcessor.Domain.Models
{
    public class ParsedData
    {
        public Row Header { get; set; }
        public Row Trailer { get; set; }
        public IList<Row> Lines { get; set; }
        public IList<Row> InvalidLines { get; set; }
        public IList<string> Errors { get; set; }
    }
}
