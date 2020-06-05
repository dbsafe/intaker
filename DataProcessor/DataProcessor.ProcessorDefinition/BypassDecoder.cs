using DataProcessor.Domain.Contracts;
using DataProcessor.Domain.Models;

namespace DataProcessor.ProcessorDefinition
{
    public class BypassDecoder : IFieldDecoder
    {
        public string Pattern { get; set; }
        public ValidationResultType? FailValidationResult { get; set; }

        public void Decode(Field field)
        {
            field.ValidationResult = ValidationResultType.Valid;
        }
    }
}
