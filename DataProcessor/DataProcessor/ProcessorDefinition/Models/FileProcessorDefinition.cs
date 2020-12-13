using System.Collections.Generic;

namespace DataProcessor.ProcessorDefinition.Models
{
    public abstract class FileProcessorDefinition
    {
        public bool CreateRowJsonEnabled { get; set; }
        public RowProcessorDefinition HeaderRowProcessorDefinition { get; set; }
        public RowProcessorDefinition TrailerRowProcessorDefinition { get; set; }
    }

    public class FileProcessorDefinition10 : FileProcessorDefinition
    {
        public RowProcessorDefinition DataRowProcessorDefinition { get; set; }
    }

    public class FileProcessorDefinition20 : FileProcessorDefinition
    {
        public Dictionary<string, RowProcessorDefinition> DataRowProcessorDefinitions { get; set; }
    }
}
