using DataProcessor.InputDefinitionFile;
using DataProcessor.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileValidator.Blazor
{
    public static class GroupHelper
    {
        public static IEnumerable<DataRow20Group> BuildRowGroups(IEnumerable<DataRow20> rows, Datas dataRowsDefinition)
        {
            var masterRowDefinition = dataRowsDefinition.Rows.FirstOrDefault(a => a.DataType == dataRowsDefinition.MasterDataType);
            if (masterRowDefinition == null)
            {
                throw new InvalidOperationException($"Master Row Definition '{dataRowsDefinition.MasterDataType}' not found");
            }

            var config = new RowGroupsCreatorConfig { MasterDataType = dataRowsDefinition.MasterDataType };
            var rowGroupsCreator = new RowGroupsCreator(config);
            return rowGroupsCreator.BuildRowGroups(rows);
        }
    }
}
