using System.Collections.Generic;

namespace DataProcessor.Transformations
{
    public class DataRow20Group
    {
        public DataRow20 MasterRow { get; set; }

        private readonly Dictionary<string, List<DataRow20>> _rowsGroupedByDataType = new Dictionary<string, List<DataRow20>>();

        public IEnumerable<KeyValuePair<string, List<DataRow20>>> RowsGroupedByType => _rowsGroupedByDataType;

        public void Add(DataRow20 row)
        {
            if (!_rowsGroupedByDataType.TryGetValue(row.DataType, out List<DataRow20> list))
            {
                list = new List<DataRow20>();
                _rowsGroupedByDataType[row.DataType] = list;
            }

            list.Add(row);
        }
    }
}
