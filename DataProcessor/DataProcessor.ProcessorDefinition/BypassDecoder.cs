using DataProcessor.Domain.Contracts;
using DataProcessor.Domain.Models;

namespace DataProcessor.ProcessorDefinition
{
    public class BypassDecoder : IFieldDecoder
    {
        public string Pattern { get; set; }
        public ValidationResultType FailValidationResult { get; set; } = ValidationResultType.Valid;

        public void Decode(Field field)
        {
            field.ValidationResult = ValidationResultType.Valid;
            field.Value = field.Raw;
        }
    }
}
