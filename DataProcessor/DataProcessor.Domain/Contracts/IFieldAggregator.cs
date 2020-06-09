using DataProcessor.Domain.Models;

namespace DataProcessor.Domain.Contracts
{
    public interface IFieldAggregator
    {
        void Aggregate(Field field, Aggregate aggregate);
        string Name { get; set; }
        string Description { get; set; }
        string Args { get; set; }
        ValidationResultType? FailValidationResult { get; set; }
    }
}
