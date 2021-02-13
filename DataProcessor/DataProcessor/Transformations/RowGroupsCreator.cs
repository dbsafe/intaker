using System.Collections.Generic;

namespace DataProcessor.Transformations
{
    public class RowGroupsCreatorConfig
    {
        public string MasterDataType { get; set; }
    }

    public class RowGroupsCreator
    {
        private readonly Dictionary<string, Row20Group> _dictionary = new Dictionary<string, Row20Group>();
        private readonly RowGroupsCreatorConfig _config;

        public RowGroupsCreator(RowGroupsCreatorConfig config)
        {
            _config = config;
        }

        public IEnumerable<Row20Group> BuildRowGroups(IEnumerable<Row20> rows)
        {
            _dictionary.Clear();

            foreach(var row in rows)
            {
                var key = row.KeyField?.Value?.ToString();
                var rowGroup = FindOrCreateRow20Group(key);
                AddRowToGroup(rowGroup, row);
            }

            return _dictionary.Values;
        }

        private void AddRowToGroup(Row20Group group, Row20 row)
        {
            var dataType = row.DataTypeField?.Value?.ToString();
            if (dataType == _config.MasterDataType)
            {
                group.MasterRow = row;
            }
            else
            {
                group.Rows.Add(row);
            }
        }

        private Row20Group FindOrCreateRow20Group(string key)
        {
            if (_dictionary.ContainsKey(key))
            {
                return _dictionary[key];
            }

            var row20Group = new Row20Group();
            _dictionary[key] = row20Group;
            return row20Group;
        }
    }
}
