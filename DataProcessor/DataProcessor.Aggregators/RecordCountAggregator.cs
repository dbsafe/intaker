using DataProcessor.Domain.Models;

namespace DataProcessor.Aggregators
{
    public class RecordCountAggregator : FieldAggregator
    {
        public override void AggregateField(Field field)
        {
            Aggregate.Value = Aggregate.AsInt() + 1;
        }
    }
}
