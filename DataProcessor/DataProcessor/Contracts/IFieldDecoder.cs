using DataProcessor.Models;

namespace DataProcessor.Contracts
{
    public interface IFieldDecoder
    {
        void Decode(Field field);
        string Pattern { get; set; }
        ValidationResultType FailValidationResult { get; set; }
    }
}
