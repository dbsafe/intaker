using DataProcessor.Domain.Models;

namespace DataProcessor.Rules
{
    public class MaxNumberFieldRule : FieldRule<NumberFieldRuleArgs>
    {
        public MaxNumberFieldRule()
            : base()
        {
        }

        public MaxNumberFieldRule(string ruleName, string ruleDescription, string args, ValidationResultType? failValidationResult)
            : base(ruleName, ruleDescription, args, failValidationResult)
        {
            ArgsHelper.EnsureDecodedArgs(Name, Description, args, DecodedArgs.RuleValue);
        }

        public override void Validate(Field field)
        {
            ArgsHelper.EnsureDecodedArgs(Name, Description, Args, DecodedArgs.RuleValue);
            base.Validate(field);
            if (field.ValidationResult != ValidationResultType.Valid)
            {
                return;
            }
            
            if (field.AsDecimal() > DecodedArgs.RuleValue)
            {
                field.ValidationResult = FailValidationResult;
            }
        }
    }
}
