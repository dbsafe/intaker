using DataProcessor.InputDefinitionFile;
using DataProcessor.InputDefinitionFile.Models;
using DataProcessor.Transformations;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace FileValidator.Blazor
{
    public class TabulatorManager20 : TabulatorManager
    {
        private readonly IJSInProcessRuntime _js;
        private readonly string _id;
        private readonly Datas _dataRowsDefinition;

        public static TabulatorManager20 Init(IJSRuntime js, string id, IEnumerable<DataRow20> rows, Datas dataRowsDefinition)
        {
            var table = new TabulatorManager20(js, id, dataRowsDefinition);
            table.Init(rows);
            return table;
        }

        private TabulatorManager20(IJSRuntime js, string id, Datas dataRowsDefinition)
        {
            _js = js as IJSInProcessRuntime;
            _id = id;
            _dataRowsDefinition = dataRowsDefinition;
        }

        private void Init(IEnumerable<DataRow20> dataRows)
        {
            var groups = GroupHelper.BuildRowGroups(dataRows, _dataRowsDefinition);
            var tableModel = BuildTableDataModel(groups, _dataRowsDefinition);

            _js.InvokeVoid("tabulator.init20", _id, tableModel, ErrorsAndWarningsColumnInfo);
        }

        private static int FindRowsDefinitionIndex(Dictionary<string, IndexedRowDefinition> rowDefinitionDictionary, RowDefinition rowDefinition)
        {
            if (!rowDefinitionDictionary.ContainsKey(rowDefinition.DataType))
            {
                throw new InvalidOperationException($"DataType '{rowDefinition.DataType}' not found in dictionary");
            }

            return rowDefinitionDictionary[rowDefinition.DataType].Index;
        }

        private static ExpandoObject BuildDisplayData(DataRow20 row, RowDefinition rowDefinition, Dictionary<string, IndexedRowDefinition> rowDefinitionDictionary)
        {
            dynamic displayData = new ExpandoObject();
            displayData.data = GetExpandoObjectFromRow(row.Row, rowDefinition);
            displayData.columnInfoIndex = FindRowsDefinitionIndex(rowDefinitionDictionary, rowDefinition);

            return displayData;
        }

        private static List<ExpandoObject> BuildChildrenRowGroups(IEnumerable<DataRow20> rows, Dictionary<string, IndexedRowDefinition> rowDefinitionDictionary)
        {
            var result = new List<ExpandoObject>();

            foreach (var row in rows)
            {
                if (!row.DataTypeFieldIndex.HasValue)
                {
                    throw new InvalidOperationException("Row does not have a DataTypeFieldIndex");
                }

                var dataType = row.Row.Fields[row.DataTypeFieldIndex.Value].Value.ToString();
                if (!rowDefinitionDictionary.ContainsKey(dataType))
                {
                    throw new InvalidOperationException("DataType '{dataType}' not found");
                }

                var rowDefinition = rowDefinitionDictionary[dataType];
                var displayRow = BuildDisplayData(row, rowDefinition.RowDefinition, rowDefinitionDictionary);

                result.Add(displayRow);
            }

            return result;
        }

        private static object BuildTableDataModel(IEnumerable<DataRow20Group> rowGroups, Datas dataRowsDefinition)
        {
            var (rowDefinitionDictionary, rowDefinitions) = BuildIndexedColumnInfos(dataRowsDefinition);
            var columnInfos = rowDefinitions.Select(BuildColumnInfo).ToArray();

            var masterRowDefinition = FindMasterRowDefinition(dataRowsDefinition);

            var tableData = new List<ExpandoObject>();

            foreach (var rowGroup in rowGroups)
            {
                var masterRow = BuildDisplayData(rowGroup.MasterRow, masterRowDefinition, rowDefinitionDictionary);
                var childrenRowGroups = BuildChildrenRowGroups(rowGroup.Rows, rowDefinitionDictionary);

                dynamic tableDataItem = new ExpandoObject();
                tableDataItem.masterRow = masterRow;
                tableDataItem.childrenRowGroups = childrenRowGroups;

                tableData.Add(tableDataItem);
            }

            return new { tableData, columnInfos };
        }

        public static IEnumerable<ChildrenRowGroup> BuildChildrenRowGroups(IEnumerable<DataRow20> childrenRows, Datas dataRowsDefinition)
        {
            var childrenRowGroupDictionary = new Dictionary<string, ChildrenRowGroup>();

            foreach (var childRow in childrenRows)
            {
                if (!childRow.DataTypeFieldIndex.HasValue)
                {
                    throw new InvalidOperationException("Row does not have a DataTypeFieldIndex");
                }

                var dataType = childRow.Row.Fields[childRow.DataTypeFieldIndex.Value].Value.ToString();
                var childrenRowGroup = GetOrCreateChildrenRowGroup(dataType, childrenRowGroupDictionary, dataRowsDefinition);
                childrenRowGroup.Rows.Add(childRow);
            }

            return childrenRowGroupDictionary.Values;
        }

        private static ChildrenRowGroup GetOrCreateChildrenRowGroup(string dataType, Dictionary<string, ChildrenRowGroup> childrenRowGroupDictionary, Datas dataRowsDefinition)
        {
            if (childrenRowGroupDictionary.ContainsKey(dataType))
            {
                return childrenRowGroupDictionary[dataType];
            }
            else
            {
                var rowDefinition = dataRowsDefinition.Rows.FirstOrDefault(a => a.DataType == dataType);
                if (rowDefinition == null)
                {
                    throw new InvalidOperationException($"Row Definition '{dataType}' not found");
                }

                var childrenRowGroup = new ChildrenRowGroup { DataType = dataType, RowDefinition = rowDefinition };
                childrenRowGroupDictionary[dataType] = childrenRowGroup;
                return childrenRowGroup;
            }
        }

        private static RowDefinition FindMasterRowDefinition(Datas dataRowsDefinition)
        {
            var masterRowDefinitions = dataRowsDefinition.Rows.Where(a => a.DataType == dataRowsDefinition.MasterDataType);

            if (masterRowDefinitions.Count() != 1)
            {
                if (masterRowDefinitions.Any())
                {
                    throw new InvalidOperationException($"Master Row Definition '{dataRowsDefinition.MasterDataType}' found multiple times");
                }
                else
                {
                    throw new InvalidOperationException($"Master Row Definition '{dataRowsDefinition.MasterDataType}' not found");
                }
            }

            return masterRowDefinitions.First();
        }

        private static (Dictionary<string, IndexedRowDefinition> rowDefinitionDictionary, List<RowDefinition> rowDefinitions) BuildIndexedColumnInfos(Datas dataRowsDefinition)
        {
            var dictionary = new Dictionary<string, IndexedRowDefinition>();
            var list = new List<RowDefinition>();

            foreach (var rowDefinition in dataRowsDefinition.Rows)
            {
                list.Add(rowDefinition);
                dictionary[rowDefinition.DataType] = new IndexedRowDefinition
                {
                    Index = list.Count - 1,
                    RowDefinition = rowDefinition
                };
            }

            return (dictionary, list);
        }

        public class IndexedRowDefinition
        {
            public int Index { get; set; }
            public RowDefinition RowDefinition { get; set; }
        }

        public class ChildrenRowGroup
        {
            public string DataType { get; set; }
            public List<DataRow20> Rows { get; } = new List<DataRow20>();
            public RowDefinition RowDefinition { get; set; }
        }
    }
}
