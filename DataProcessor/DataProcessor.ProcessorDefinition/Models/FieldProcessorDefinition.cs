using DataProcessor.Domain.Contracts;

namespace DataProcessor.ProcessorDefinition.Models
{
    public class FieldProcessorDefinition
    {
        public IFieldDecoder Decoder { get; set; }
        public string FieldName { get; set; }
    }
}
