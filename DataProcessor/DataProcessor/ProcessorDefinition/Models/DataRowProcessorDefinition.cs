namespace DataProcessor.ProcessorDefinition.Models
{
    public class DataRowProcessorDefinition
    {
        public RowProcessorDefinition RowProcessorDefinition { get; set; }
        public int DataTypeFieldIndex { get; set; } = -1;
        public int DataKeyFieldIndex { get; set; } = -1;
    }
}
