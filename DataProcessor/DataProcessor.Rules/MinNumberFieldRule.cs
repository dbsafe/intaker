using DataProcessor.Domain.Models;

namespace DataProcessor.Rules
{
    public class MinNumberFieldRule : FieldRule<NumberFieldRuleArgs>
    {
        public MinNumberFieldRule()
            : base()
        {
        }

        public MinNumberFieldRule(string ruleName, string ruleDescription, string args, ValidationResultType? failValidationResult)
            : base(ruleName, ruleDescription, args, failValidationResult)
        {
            ArgsHelper.EnsureDecodedArgs(Name, Description, args, DecodedArgs.RuleValue);
        }

        public override void Validate(Field field)
        {
            if (field.ValidationResult != ValidationResultType.Valid)
            {
                return;
            }

            if (field.AsDecimal() < DecodedArgs.RuleValue)
            {
                field.ValidationResult = FailValidationResult;
            }
        }
    }
}
