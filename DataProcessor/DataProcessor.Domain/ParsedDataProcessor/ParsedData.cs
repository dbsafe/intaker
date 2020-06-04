using DataProcessor.Domain.Models;
using System.Collections.Generic;

namespace DataProcessor.Domain
{
    public class ParsedData
    {
        public Row Header { get; set; }
        public Row Trailer { get; set; }
        public IList<Row> Rows { get; set; }
        public IList<Row> InvalidRows { get; set; }
        public IList<string> Errors { get; set; }
    }
}
