namespace DataProcessor.ProcessorDefinition.Models
{
    public class ProcessorDefinition
    {
        public RowProcessorDefinition HeaderRowProcessorDefinition { get; set; }
        public RowProcessorDefinition TrailerRowProcessorDefinition { get; set; }
        public RowProcessorDefinition DataRowProcessorDefinition { get; set; }
    }
}
