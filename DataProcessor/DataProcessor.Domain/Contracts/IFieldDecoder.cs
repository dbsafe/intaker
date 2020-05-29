using DataProcessor.Domain.Models;

namespace DataProcessor.Domain.Contracts
{
    public interface IFieldDecoder
    {
        void Decode(Field field);
        string Pattern { get; set; }
        ValidationResultType? FailValidationResult { get; set; }
    }
}
