using DataProcessor.Models;

namespace DataProcessor.Aggregators
{
    public class SumAggregator : FieldAggregator
    {
        public override void AggregateField(Field field)
        {
            if (field.ValidationResult != ValidationResultType.Valid)
            {
                return;
            }

            Aggregate.Value = Aggregate.AsDecimal() + field.AsDecimal();
        }
    }
}
