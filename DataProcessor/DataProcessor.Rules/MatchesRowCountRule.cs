using DataProcessor.Domain.Contracts;
using DataProcessor.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataProcessor.Rules
{
    public class MatchesRowCountRule : FieldRule<MatchesRowCountRuleArgs>
    {
        private Aggregate _aggregate;

        private void SetAggregates(IEnumerable<Aggregate> aggregates)
        {
            _aggregate = aggregates.FirstOrDefault(a => a.Name == DecodedArgs.RuleValue);
            if (_aggregate == null)
            {
                throw new InvalidOperationException($"{typeof(MatchesRowCountRule)} - Aggregate '{DecodedArgs.RuleValue}' not found");
            }
        }

        public override void Validate(Field field)
        {
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

        public override void Initialize(FieldRuleConfiguration config)
        {
            base.Initialize(config);
            ArgsHelper.EnsureDecodedArgs(Name, Description, Args, DecodedArgs.RuleValue);
            SetAggregates(config.Aggregates);
        }
    }
}
