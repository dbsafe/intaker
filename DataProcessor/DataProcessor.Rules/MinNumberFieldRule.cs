using DataProcessor.Domain.Contracts;
using DataProcessor.Domain.Models;

namespace DataProcessor.Rules
{
    public class MinNumberFieldRule : FieldRule<NumberFieldRuleArgs>
    {
        public override void Validate(Field field)
        {
            base.Validate(field);
            if (field.ValidationResult != ValidationResultType.Valid)
            {
                return;
            }

            if (field.AsDecimal() < DecodedArgs.RuleValue)
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
