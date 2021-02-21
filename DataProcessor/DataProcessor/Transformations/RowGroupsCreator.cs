using System.Collections.Generic;

namespace DataProcessor.Transformations
{
    public class RowGroupsCreatorConfig
    {
        public string MasterDataType { get; set; }
    }

    public class RowGroupsCreator
    {
        private readonly Dictionary<string, DataRow20Group> _dictionary = new Dictionary<string, DataRow20Group>();
        private readonly RowGroupsCreatorConfig _config;

        public RowGroupsCreator(RowGroupsCreatorConfig config)
        {
            _config = config;
        }

        public IEnumerable<DataRow20Group> BuildRowGroups(IEnumerable<DataRow20> rows)
        {
            _dictionary.Clear();

            foreach(var row in rows)
            {
                var rowGroup = FindOrCreateRow20Group(row.DataKey);
                AddRowToGroup(rowGroup, row);
            }

            return _dictionary.Values;
        }

        private void AddRowToGroup(DataRow20Group group, DataRow20 row)
        {
            var isMasterRow = row.DataType == _config.MasterDataType;
            if (isMasterRow)
            {
                group.MasterRow = row;
            }
            else
            {
                group.Rows.Add(row);
            }
        }

        private DataRow20Group FindOrCreateRow20Group(string key)
        {
            if (_dictionary.ContainsKey(key))
            {
                return _dictionary[key];
            }

            var row20Group = new DataRow20Group();
            _dictionary[key] = row20Group;
            return row20Group;
        }
    }
}
