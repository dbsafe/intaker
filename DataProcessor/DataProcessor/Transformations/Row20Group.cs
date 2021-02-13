using System.Collections.Generic;

namespace DataProcessor.Transformations
{
    public class Row20Group
    {
        public Row20 MasterRow { get; set; }
        public List<Row20> Rows { get; } = new List<Row20>();
    }
}
