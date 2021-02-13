using System;
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
                var key = row.KeyFieldIndex.HasValue ? row.Row.Fields[row.KeyFieldIndex.Value].Value.ToString() : null;
                var rowGroup = FindOrCreateRow20Group(key);
                AddRowToGroup(rowGroup, row);
            }

            return _dictionary.Values;
        }

        private void AddRowToGroup(DataRow20Group group, DataRow20 row)
        {
            if (IsMasterRow(row))
            {
                group.MasterRow = row;
            }
            else
            {
                group.Rows.Add(row);
            }
        }

        private bool IsMasterRow(DataRow20 row)
        {
            if (!row.DataTypeFieldIndex.HasValue)
            {
                return false;
            }

            if (row.DataTypeFieldIndex.Value >= row.Row.Fields.Count)
            {
                throw new InvalidOperationException($"RowIndex: {row.Row.Index}. DataTypeFieldIndex {row.DataTypeFieldIndex.Value} is greater or equal than the number of fields {row.Row.Fields.Count}");
            }

            return row.Row.Fields[row.DataTypeFieldIndex.Value].Value.ToString() == _config.MasterDataType;
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
