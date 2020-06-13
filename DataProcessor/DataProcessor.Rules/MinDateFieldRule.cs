using DataProcessor.Domain.Contracts;
using DataProcessor.Domain.Models;

namespace DataProcessor.Rules
{
    public class MinDateFieldRule : FieldRule<DateFieldRuleArgs>
    {
        public override void Validate(Field field)
        {
            base.Validate(field);
            if (field.ValidationResult != ValidationResultType.Valid)
            {
                return;
            }

            if (field.AsDateTime() < DecodedArgs.RuleValue)
            {
                field.ValidationResult = FailValidationResult;
            }
        }

        public override void Initialize(FieldRuleConfiguration config)
        {
            base.Initialize(config);
            ArgsHelper.EnsureDecodedArgs(Name, Description, Args, DecodedArgs.RuleValue);
        }
    }
}
