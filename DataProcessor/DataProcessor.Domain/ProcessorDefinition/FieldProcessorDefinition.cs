using DataProcessor.Domain.Contracts;

namespace DataProcessor.Domain
{
    public class FieldProcessorDefinition
    {
        public IFieldDecoder Decoder { get; set; }
        public string FieldName { get; set; }
    }
}
