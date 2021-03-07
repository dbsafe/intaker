using DataProcessor.Models;
using DataProcessor.Utils;
using System;

namespace DataProcessor.Rules
{
    public class MinNumberFieldRule : FieldRule
    {
        private decimal _decimalArg;

        protected override void SingleArgChanged()
        {
            DataProcessorGlobal.Debug($"Rule: {Name}. Arg: '{_singleArg}'.");
            var isValidDecimal = decimal.TryParse(_singleArg, out _decimalArg);
            if (!isValidDecimal)
            {
                throw new InvalidOperationException($"RuleName: {Name}, RuleDescription: {Description} - Invalid arg '{_singleArg}'");
            }
        }

        public override void Validate(Field field)
        {
            base.Validate(field);
            if (field.ValidationResult != ValidationResultType.Valid)
            {
                return;
            }

            if (field.AsDecimal() < _decimalArg)
            {
                field.ValidationResult = FailValidationResult;
            }
        }
    }
}
