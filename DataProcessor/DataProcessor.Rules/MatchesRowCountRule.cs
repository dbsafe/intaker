using DataProcessor.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataProcessor.Rules
{
    public class MatchesRowCountRule : FieldRule<MatchesRowCountRuleArgs>
    {
        private Aggregate _aggregate;

        public MatchesRowCountRule()
            : base()
        {
        }

        public MatchesRowCountRule(string ruleName, string ruleDescription, string args, ValidationResultType? failValidationResult)
            : base(ruleName, ruleDescription, args, failValidationResult)
        {
            ArgsHelper.EnsureDecodedArgs(Name, Description, args, DecodedArgs.RuleValue);
        }

        public override void SetAggregates(IEnumerable<Aggregate> aggregates)
        {
            _aggregate = aggregates.FirstOrDefault(a => a.Name == DecodedArgs.RuleValue);
            if (_aggregate == null)
            {
                throw new InvalidOperationException($"{typeof(MatchesRowCountRule)} - Aggregate '{DecodedArgs.RuleValue}' not found");
            }
        }

        public override void Validate(Field field)
        {
            ArgsHelper.EnsureDecodedArgs(Name, Description, Args, DecodedArgs.RuleValue);
            base.Validate(field);
            if (field.ValidationResult != ValidationResultType.Valid)
            {
                return;
            }

            if (field.AsInt() != _aggregate.AsInt())
            {
                field.ValidationResult = FailValidationResult;
            }
        }
    }
}
