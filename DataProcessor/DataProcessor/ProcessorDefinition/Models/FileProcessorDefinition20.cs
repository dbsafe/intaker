using System.Collections.Generic;

namespace DataProcessor.ProcessorDefinition.Models
{
    public class FileProcessorDefinition20 : FileProcessorDefinition
    {
        public string KeyField { get; set; }
        public Dictionary<string, RowProcessorDefinition> DataRowProcessorDefinitions { get; set; }
    }
}
