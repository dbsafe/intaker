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
        private const string ARG_AGGREGATE = "AggregateName";
        private Aggregate _aggregateArg;

        private void SetArg(IEnumerable<Aggregate> aggregates)
        {
            var textArg = Args?.FirstOrDefault(a => a.Key == ARG_AGGREGATE).Value;
            if (string.IsNullOrEmpty(textArg))
            {
                throw new InvalidOperationException($"Rule: '{Name}'. Argument '{ARG_AGGREGATE}' not found");
            }

            DataProcessorGlobal.Debug($"Rule: {Name}. Argument {ARG_AGGREGATE}: '{textArg}'.");

            _aggregateArg = aggregates.FirstOrDefault(a => a.Name == textArg);
            if (_aggregateArg == null)
            {
                throw new InvalidOperationException($"Rule: '{Name}'. Aggregate '{textArg}' not found");
            }
        }

        public override void Validate(Field field)
        {
            base.Validate(field);
            if (field.ValidationResult != ValidationResultType.Valid)
            {
                return;
            }

            if (field.AsDecimal() != _aggregateArg.AsDecimal())
            {
                field.ValidationResult = FailValidationResult;
            }
        }

        public override void Initialize(FieldRuleConfiguration config)
        {
            base.Initialize(config);
            SetArg(config.Aggregates);
        }
    }
}
