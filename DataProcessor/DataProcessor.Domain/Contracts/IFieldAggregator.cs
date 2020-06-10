using DataProcessor.Domain.Models;

namespace DataProcessor.Domain.Contracts
{
    public interface IFieldAggregator
    {
        void AggregateField(Field field);
        string Name { get; set; }
        string Description { get; set; }
        Aggregate Aggregate { get; set; }
    }
}
