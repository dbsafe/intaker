using DataProcessor.Contracts;
using DataProcessor.Models;
using DataProcessor.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataProcessor.Rules
{
    public class MatchesAggregateRule : FieldRule
    {
        private Aggregate _aggregate;

        protected override void SingleArgChanged()
        {
            DataProcessorGlobal.Debug($"Rule: {Name}. Arg: '{_singleArg}'.");
            var isValidArg = !string.IsNullOrWhiteSpace(_singleArg);
            if (!isValidArg)
            {
                throw new InvalidOperationException($"RuleName: {Name}, RuleDescription: {Description} - Arg cannot be null or empty");
            }
        }

        private void SetAggregate(IEnumerable<Aggregate> aggregates)
        {
            _aggregate = aggregates.FirstOrDefault(a => a.Name == _singleArg);
            if (_aggregate == null)
            {
                throw new InvalidOperationException($"{typeof(MatchesAggregateRule)} - Aggregate '{_singleArg}' not found");
            }
        }

        public override void Validate(Field field)
        {
            base.Validate(field);
            if (field.ValidationResult != ValidationResultType.Valid)
            {
                return;
            }

            if (field.AsDecimal() != _aggregate.AsDecimal())
            {
                field.ValidationResult = FailValidationResult;
            }
        }

        public override void Initialize(FieldRuleConfiguration config)
        {
            base.Initialize(config);
            SetAggregate(config.Aggregates);
        }
    }
}
