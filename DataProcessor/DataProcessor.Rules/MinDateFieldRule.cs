using DataProcessor.Domain.Models;

namespace DataProcessor.Rules
{
    public class MinDateFieldRule : FieldRule<DateFieldRuleArgs>
    {
        public MinDateFieldRule()
            : base()
        {
        }

        public MinDateFieldRule(string ruleName, string ruleDescription, string args, ValidationResultType? failValidationResult)
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

            if (field.AsDateTime() < DecodedArgs.RuleValue)
            {
                field.ValidationResult = FailValidationResult;
            }
        }
    }
}
