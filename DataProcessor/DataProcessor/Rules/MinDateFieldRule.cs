using DataProcessor.Models;

namespace DataProcessor.Rules
{
    public class MinDateFieldRule : DateFieldRule
    {

        public override void Validate(Field field)
        {
            base.Validate(field);
            if (field.ValidationResult != ValidationResultType.Valid)
            {
                return;
            }

            if (field.AsDateTime() < _dateTimeArg)
            {
                field.ValidationResult = FailValidationResult;
            }
        }
    }
}
