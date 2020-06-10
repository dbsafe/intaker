using DataProcessor.Domain.Contracts;
using DataProcessor.Domain.Models;

namespace DataProcessor.Aggregators
{
    public abstract class FieldAggregator : IFieldAggregator
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Aggregate Aggregate { get; set; }

        public abstract void AggregateField(Field field);
    }
}
