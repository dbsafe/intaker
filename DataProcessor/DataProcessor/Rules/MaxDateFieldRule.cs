using DataProcessor.Models;
using DataProcessor.Utils;
using System;

namespace DataProcessor.Rules
{
    public class MaxDateFieldRule : FieldRule
    {
        private DateTime _dateTimeArg;

        protected override void SingleArgChanged() 
        {
            DataProcessorGlobal.Debug($"Rule: {Name}. Arg: '{_singleArg}'.");
            var isValidDateTime = DateTime.TryParse(_singleArg, out _dateTimeArg);
            if (!isValidDateTime)
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

            if (field.AsDateTime() > _dateTimeArg)
            {
                field.ValidationResult = FailValidationResult;
            }
        }
    }
}
