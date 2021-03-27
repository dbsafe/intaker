using DataProcessor.Contracts;
using DataProcessor.Models;
using DataProcessor.Utils;
using System;
using System.Linq;

namespace DataProcessor.Rules
{
    public class RangeNumberFieldRule : FieldRule
    {
        private const string ARG_NUMERIC_VALUE_MIN = "NumericValueMin";
        private const string ARG_NUMERIC_VALUE_MAX = "NumericValueMax";

        private decimal _numericMinArg;
        private decimal _numericMaxArg;

        private void SetMinArg()
        {
            var textArg = Args?.FirstOrDefault(a => a.Key == ARG_NUMERIC_VALUE_MIN).Value;
            if (string.IsNullOrEmpty(textArg))
            {
                throw new InvalidOperationException($"Rule: '{Name}'. Argument '{ARG_NUMERIC_VALUE_MIN}' not found");
            }

            DataProcessorGlobal.Debug($"Rule: {Name}. Argument {ARG_NUMERIC_VALUE_MIN}: '{textArg}'.");

            var isValidDecimal = decimal.TryParse(textArg, out _numericMinArg);
            if (!isValidDecimal)
            {
                throw new InvalidOperationException($"Rule: '{Name}'. Argument: '{ARG_NUMERIC_VALUE_MIN}'. Invalid value '{textArg}'");
            }
        }

        private void SetMaxArg()
        {
            var textArg = Args?.FirstOrDefault(a => a.Key == ARG_NUMERIC_VALUE_MAX).Value;
            if (string.IsNullOrEmpty(textArg))
            {
                throw new InvalidOperationException($"Rule: '{Name}'. Argument '{ARG_NUMERIC_VALUE_MAX}' not found");
            }

            DataProcessorGlobal.Debug($"Rule: {Name}. Argument {ARG_NUMERIC_VALUE_MAX}: '{textArg}'.");

            var isValidDecimal = decimal.TryParse(textArg, out _numericMaxArg);
            if (!isValidDecimal)
            {
                throw new InvalidOperationException($"Rule: '{Name}'. Argument: '{ARG_NUMERIC_VALUE_MAX}'. Invalid value '{textArg}'");
            }
        }

        public override void Validate(Field field)
        {
            base.Validate(field);
            if (field.ValidationResult != ValidationResultType.Valid)
            {
                return;
            }

            var fieldAsDecimal = field.AsDecimal();

            if (fieldAsDecimal < _numericMinArg || fieldAsDecimal > _numericMaxArg)
            {
                field.ValidationResult = FailValidationResult;
            }
        }


        public override void Initialize(FieldRuleConfiguration config)
        {
            base.Initialize(config);
            SetMinArg();
            SetMaxArg();
        }
    }
}
