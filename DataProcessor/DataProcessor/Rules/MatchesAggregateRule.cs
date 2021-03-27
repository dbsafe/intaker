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
        private const string ARG_AGGREGATOR = "aggregate";
        private Aggregate _aggregate;

        private void SetAggregate(IEnumerable<Aggregate> aggregates)
        {
            var aggregateName = Args?.FirstOrDefault(a => a.Key == ARG_AGGREGATOR).Value;
            if (string.IsNullOrEmpty(aggregateName))
            {
                throw new InvalidOperationException($"Rule: '{Name}'. Argument '{ARG_AGGREGATOR}' not found");
            }

            DataProcessorGlobal.Debug($"Rule: {Name}. Argument {ARG_AGGREGATOR}: '{aggregateName}'.");

            _aggregate = aggregates.FirstOrDefault(a => a.Name == aggregateName);
            if (_aggregate == null)
            {
                throw new InvalidOperationException($"Rule: '{Name}'. Aggregate '{aggregateName}' not found");
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
