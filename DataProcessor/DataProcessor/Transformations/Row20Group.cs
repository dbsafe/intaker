using System.Collections.Generic;

namespace DataProcessor.Transformations
{
    public class DataRow20Group
    {
        public DataRow20 MasterRow { get; set; }
        public List<DataRow20> Rows { get; } = new List<DataRow20>();
    }
}
