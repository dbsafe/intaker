using DataProcessor.Models;

namespace DataProcessor.Rules
{
    public class MaxNumberFieldRule : NumberFieldRule
    {
        public override void Validate(Field field)
        {
            base.Validate(field);
            if (field.ValidationResult != ValidationResultType.Valid)
            {
                return;
            }

            if (field.AsDecimal() > _numericArg)
            {
                field.ValidationResult = FailValidationResult;
            }
        }
    }
}
