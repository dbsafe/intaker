using DataProcessor.InputDefinitionFile;
using DataProcessor.Transformations;
using System.Collections.Generic;

namespace FileValidator.Blazor
{
    public static class GroupHelper
    {
        public static IEnumerable<DataRow20Group> BuildRowGroups(IEnumerable<DataRow20> rows, Datas dataRowsDefinition)
        {
            var config = new RowGroupsCreatorConfig { MasterDataType = dataRowsDefinition.MasterDataType };
            var rowGroupsCreator = new RowGroupsCreator(config);
            return rowGroupsCreator.BuildRowGroups(rows);
        }
    }
}
