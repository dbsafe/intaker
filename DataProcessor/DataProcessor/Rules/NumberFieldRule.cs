using DataProcessor.Contracts;
using DataProcessor.Utils;
using System;
using System.Linq;

namespace DataProcessor.Rules
{
    public abstract class NumberFieldRule : FieldRule
    {
        private const string ARG_NUMERIC_VALUE = "NumericValue";
        protected decimal _numericArg;

        private void SetArg()
        {
            var textArg = Args?.FirstOrDefault(a => a.Key == ARG_NUMERIC_VALUE).Value;
            if (string.IsNullOrEmpty(textArg))
            {
                throw new InvalidOperationException($"Rule: '{Name}'. Argument '{ARG_NUMERIC_VALUE}' not found");
            }

            DataProcessorGlobal.Debug($"Rule: {Name}. Argument {ARG_NUMERIC_VALUE}: '{textArg}'.");

            var isValidDecimal = decimal.TryParse(textArg, out _numericArg);
            if (!isValidDecimal)
            {
                throw new InvalidOperationException($"Rule: '{Name}'. Argument: '{ARG_NUMERIC_VALUE}'. Invalid value '{textArg}'");
            }
        }

        public override void Initialize(FieldRuleConfiguration config)
        {
            base.Initialize(config);
            SetArg();
        }
    }
}
