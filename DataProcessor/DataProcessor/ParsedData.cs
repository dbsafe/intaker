using DataProcessor.Domain.Models;
using System.Collections.Generic;

namespace DataProcessor
{
    public class ParsedData
    {
        public Row Header { get; set; }
        public Row Trailer { get; set; }
        public IList<Row> AllRows { get; set; }
        public IList<Row> InvalidRows { get; set; }
        public IList<string> Errors { get; set; }
    }
}
