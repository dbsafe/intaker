using DataProcessor.Models;

namespace DataProcessor.Aggregators
{
    public class RowCountAggregator : FieldAggregator
    {
        public override void AggregateField(Field field)
        {
            Aggregate.Value = Aggregate.AsInt() + 1;
        }
    }
}
