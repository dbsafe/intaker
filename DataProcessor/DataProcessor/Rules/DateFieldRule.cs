using DataProcessor.Contracts;
using DataProcessor.Utils;
using System;
using System.Linq;

namespace DataProcessor.Rules
{
    public abstract class DateFieldRule : FieldRule
    {
        private const string ARG_DATETIME = "DateTimeValue";
        protected DateTime _dateTimeArg;

        private void SetArg()
        {
            var textArg = Args?.FirstOrDefault(a => a.Key == ARG_DATETIME).Value;
            if (string.IsNullOrEmpty(textArg))
            {
                throw new InvalidOperationException($"Rule: '{Name}'. Argument '{ARG_DATETIME}' not found");
            }

            DataProcessorGlobal.Debug($"Rule: {Name}. Argument {ARG_DATETIME}: '{textArg}'.");

            var isValidDateTime = DateTime.TryParse(textArg, out _dateTimeArg);
            if (!isValidDateTime)
            {
                throw new InvalidOperationException($"Rule: '{Name}'. Argument: '{ARG_DATETIME}'. Invalid value '{textArg}'");
            }
        }

        public override void Initialize(FieldRuleConfiguration config)
        {
            base.Initialize(config);
            SetArg();
        }
    }
}
