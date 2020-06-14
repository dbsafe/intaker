namespace DataProcessor.ProcessorDefinition.Models
{
    public class FileProcessorDefinition
    {
        public bool CreateRowJsonEnabled { get; set; }
        public RowProcessorDefinition HeaderRowProcessorDefinition { get; set; }
        public RowProcessorDefinition TrailerRowProcessorDefinition { get; set; }
        public RowProcessorDefinition DataRowProcessorDefinition { get; set; }
    }
}
