using DataProcessor.Domain.Models;

namespace DataProcessor
{
    public static class ParsedDataProcessorHelper
    {
        public static ValidationResultType GetMaxValidationResult(ValidationResultType value1, ValidationResultType value2)
        {
            return value1.CompareTo(value2) > 0 ? value1 : value2;
        }
    }
}
