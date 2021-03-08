using DataProcessor.Models;
using DataProcessor.Utils;
using System;
using System.Linq;

namespace DataProcessor.Rules
{
    public class RangeNumberFieldRule : FieldRule
    {
        private const string ARG_MIN = "Min";
        private const string ARG_MAX = "Max";

        private decimal _decimalMinArg;
        private decimal _decimalMaxArg;

        protected override void ArgsChanged()
        {
            if (_args == null || _args.Length == 0)
            {
                throw new InvalidOperationException($"RuleName: {Name}, RuleDescription: {Description} - Args cannot be null or empty");
            }

            _decimalMinArg = GetArg(ARG_MIN);
            _decimalMaxArg = GetArg(ARG_MAX);
            
            DataProcessorGlobal.Debug($"Rule: {Name}. Min: {_decimalMinArg}, Max: {_decimalMaxArg}.");
        }

        private decimal GetArg(string name)
        {
            var kvp = _args.FirstOrDefault(a => a.Key == name);
            if (kvp.Key == null)
            {
                throw new InvalidOperationException($"RuleName: {Name}, RuleDescription: {Description} - Arg '{name}' not found");
            }

            var isValidDecimal = decimal.TryParse(kvp.Value, out decimal decimalArg);
            if (!isValidDecimal)
            {
                throw new InvalidOperationException($"RuleName: {Name}, RuleDescription: {Description} - Invalid arg '{name}', found '{kvp.Value}'");
            }

            return decimalArg;
        }

        public override void Validate(Field field)
        {
            base.Validate(field);
            if (field.ValidationResult != ValidationResultType.Valid)
            {
                return;
            }

            var fieldAsDecimal = field.AsDecimal();

            if (fieldAsDecimal < _decimalMinArg || fieldAsDecimal > _decimalMaxArg)
            {
                field.ValidationResult = FailValidationResult;
            }
        }
    }
}
