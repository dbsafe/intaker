using DataProcessor.Domain.Models;

namespace DataProcessor.Domain.Contracts
{
    public interface IFieldRule
    {
        void Validate(Field field);
        string Name { get; set; }
        string Description { get; set; }
        string Args { get; set; }
        ValidationResultType? FailValidationResult { get; set; }
    }
}
