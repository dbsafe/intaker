using DataProcessor.Models;

namespace DataProcessor.Contracts
{
    public interface IFieldAggregator
    {
        void AggregateField(Field field);
        string Name { get; set; }
        string Description { get; set; }
        Aggregate Aggregate { get; set; }
    }
}
