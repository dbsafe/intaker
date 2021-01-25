using System.Collections.Generic;

namespace DataProcessor.ProcessorDefinition.Models
{
    public class FileProcessorDefinition20 : FileProcessorDefinition
    {
        public string KeyField { get; set; }

        public string DataTypeField { get; set; }

        public Dictionary<string, DataRowProcessorDefinition> DataRowProcessorDefinitions { get; set; }
    }
}
