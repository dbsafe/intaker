using DataProcessor.Contracts;

namespace DataProcessor.ProcessorDefinition.Models
{
    public class FieldProcessorDefinition
    {
        public IFieldDecoder Decoder { get; set; }
        public IFieldRule[] Rules { get; set; }
        public IFieldAggregator[] Aggregators { get; set; }
        public string FieldName { get; set; }
        public string Description { get; set; }
    }
}
