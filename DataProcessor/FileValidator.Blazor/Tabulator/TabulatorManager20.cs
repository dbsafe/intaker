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

        private static List<object> RawColumnInfo { get; } = new List<object>()
        {
            new { title = "Line Number", field = "lineNumber", headerSort = false },
            new { title = "Raw", field = "raw", headerSort = false }
        };

        public static TabulatorManager20 Init(IJSRuntime js, string id, IEnumerable<DataRow20> rows, Datas dataRowsDefinition)
        {
            var table = new TabulatorManager20(js, id);
            table.InitWithRowDefinition(rows, dataRowsDefinition);
            return table;
        }

        public static TabulatorManager20 InitWithRawValues(IJSRuntime js, string id, IEnumerable<DataRow20> rows)
        {
            var table = new TabulatorManager20(js, id);
            table.InitWithRawValues(rows);
            return table;
        }

        private TabulatorManager20(IJSRuntime js, string id)
        {
            _js = js as IJSInProcessRuntime;
            _id = id;
        }

        private void InitWithRawValues(IEnumerable<DataRow20> dataRows)
        {
            var tableData = dataRows.Select(a => new { lineNumber = a.Row.Index + 1, raw = a.Row.Raw });
            var tableModel = new { tableData, columnInfos = RawColumnInfo };
            _js.InvokeVoid("tabulator.init20Raw", _id, tableModel);
        }

        private void InitWithRowDefinition(IEnumerable<DataRow20> dataRows, Datas dataRowsDefinition)
        {
            var groups = GroupHelper.BuildRowGroups(dataRows, dataRowsDefinition);
            var tableModel = BuildTableDataModel(groups, dataRowsDefinition);

            _js.InvokeVoid("tabulator.init20", _id, tableModel, ErrorsAndWarningsColumnInfo);
        }

        private static int FindRowsDefinitionIndex(Dictionary<string, IndexedRowDefinition> rowDefinitionDictionary, string dataType)
        {
            var indexedRowDefinition = FindIndexedRowDefinition(rowDefinitionDictionary, dataType);
            return indexedRowDefinition.Index;
        }

        private static IndexedRowDefinition FindIndexedRowDefinition(Dictionary<string, IndexedRowDefinition> rowDefinitionDictionary, string dataType)
        {
            if (!rowDefinitionDictionary.ContainsKey(dataType))
            {
                throw new InvalidOperationException($"DataType '{dataType}' not found in dictionary");
            }

            return rowDefinitionDictionary[dataType];
        }

        private static ExpandoObject BuildDisplayData(DataRow20 row, RowDefinition rowDefinition, Dictionary<string, IndexedRowDefinition> rowDefinitionDictionary)
        {
            dynamic displayData = GetExpandoObjectFromRow(row.Row, rowDefinition);
            displayData.columnInfoIndex = FindRowsDefinitionIndex(rowDefinitionDictionary, rowDefinition.DataType);

            return displayData;
        }

        private static ExpandoObject BuildDisplayData(DataRow20 row, Dictionary<string, IndexedRowDefinition> rowDefinitionDictionary)
        {
            var indexedRowDefinition = FindIndexedRowDefinition(rowDefinitionDictionary, row.DataType);

            dynamic displayData = GetExpandoObjectFromRow(row.Row, indexedRowDefinition.RowDefinition);
            displayData.columnInfoIndex = indexedRowDefinition.Index;

            return displayData;
        }

        private static IEnumerable<IEnumerable<ExpandoObject>> BuildChildrenRowGroups(
            IEnumerable<KeyValuePair<string, List<DataRow20>>> rowsGroupedByType, 
            Dictionary<string, IndexedRowDefinition> rowDefinitionDictionary)
        {
            var childrenRowGroups = new List<IEnumerable<ExpandoObject>>();
            foreach (var rowsGroup in rowsGroupedByType)
            {
                var rows = BuildChildrenRows(rowsGroup.Value, rowDefinitionDictionary);
                childrenRowGroups.Add(rows);
            }

            return childrenRowGroups;
        }

        private static List<ExpandoObject> BuildChildrenRows(IEnumerable<DataRow20> dataRows, Dictionary<string, IndexedRowDefinition> rowDefinitionDictionary)
        {
            var childrenRows = new List<ExpandoObject>();
            foreach (var dataRow in dataRows)
            {
                var displayData = BuildDisplayData(dataRow, rowDefinitionDictionary);
                childrenRows.Add(displayData);
            }


            return childrenRows;
        }

        private static object BuildTableDataModel(IEnumerable<DataRow20Group> rowGroups, Datas dataRowsDefinition)
        {
            var (rowDefinitionDictionary, rowDefinitions) = BuildIndexedColumnInfos(dataRowsDefinition);
            var columnInfos = rowDefinitions.Select(BuildColumnInfo).ToArray();

            var masterRowDefinition = FindMasterRowDefinition(dataRowsDefinition);

            var tableData = new List<ExpandoObject>();
            foreach (var rowGroup in rowGroups)
            {
                dynamic tableDataItem = BuildDisplayData(rowGroup.MasterRow, masterRowDefinition, rowDefinitionDictionary);
                tableDataItem.childrenRowGroups = BuildChildrenRowGroups(rowGroup.RowsGroupedByType, rowDefinitionDictionary); ;

                tableData.Add(tableDataItem);
            }

            return new { tableData, columnInfos };
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
