using DataProcessor.InputDefinitionFile;
using DataProcessor.Models;
using DataProcessor.Transformations;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
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

        private IEnumerable<DataRow20Group> BuildRowGroups(IEnumerable<DataRow20> rows, string masterDataType)
        {
            var config = new RowGroupsCreatorConfig { MasterDataType = masterDataType };
            var rowGroupsCreator = new RowGroupsCreator(config);
            return rowGroupsCreator.BuildRowGroups(rows);
        }

        private void Init(IEnumerable<DataRow20> rows)
        {
            var masterRowDefinition = _dataRowsDefinition.Rows.FirstOrDefault(a => a.DataType == _dataRowsDefinition.MasterDataType);
            if (masterRowDefinition == null)
            {
                throw new InvalidOperationException($"Master Row Definition '{_dataRowsDefinition.MasterDataType}' not found");
            }

            //var fieldNames = masterRowDefinition.Fields.Select(a => a.Name).ToArray();
            //var dataTypeFieldIndex = Array.FindIndex(fieldNames, a => a == _dataRowsDefinition.DataTypeField);
            //if (dataTypeFieldIndex == -1)
            //{
            //    throw new InvalidOperationException($"Master Row Definition '{_dataRowsDefinition.MasterDataType}' not found");
            //}

            var transformedData = BuildRowGroups(rows, _dataRowsDefinition.MasterDataType);

            var masterRows = new List<Row>();
            foreach (var row in rows)
            {

            }


            //var columnInfo = BuildColumnInfo();
            //var subtableColumnInfo = BuildSubtableColumnInfo();
            //var tableData = BuildTableData(rows);

            //_js.InvokeVoid("tabulator.init20", _id, tableData, columnInfo, subtableColumnInfo);
        }

        private IEnumerable<RowAndKey> BuildRowWithKey(IEnumerable<Row> rows)
        {
            var result = new List<RowAndKey>();

            foreach (var row in rows)
            {
                var rowAndKey = new RowAndKey { Row = row };
                //result.Add()
            }

            return result;
        }

        private class RowAndKey
        {
            public object Key { get; set; }
            public Row Row { get; set; }
        }
    }
}
