namespace DataProcessor.ProcessorDefinition.Models
{
    public abstract class FileProcessorDefinition
    {
        public bool CreateRowJsonEnabled { get; set; }
        public RowProcessorDefinition HeaderRowProcessorDefinition { get; set; }
        public RowProcessorDefinition TrailerRowProcessorDefinition { get; set; }
    }
}
