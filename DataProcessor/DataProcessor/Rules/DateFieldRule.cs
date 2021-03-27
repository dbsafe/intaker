using DataProcessor.Contracts;
using DataProcessor.Utils;
using System;
using System.Linq;

namespace DataProcessor.Rules
{
    public abstract class DateFieldRule : FieldRule
    {
        private const string ARG_DATETIME = "DateTime";
        protected DateTime _dateTimeArg;

        private void SetDateTimeArg()
        {
            var dateTimeTextArg = Args?.FirstOrDefault(a => a.Key == ARG_DATETIME).Value;
            if (string.IsNullOrEmpty(dateTimeTextArg))
            {
                throw new InvalidOperationException($"Rule: '{Name}'. Argument '{ARG_DATETIME}' not found");
            }

            DataProcessorGlobal.Debug($"Rule: {Name}. Argument {ARG_DATETIME}: '{dateTimeTextArg}'.");

            var isValidDateTime = DateTime.TryParse(dateTimeTextArg, out _dateTimeArg);
            if (!isValidDateTime)
            {
                throw new InvalidOperationException($"Rule: '{Name}'. Argument: '{ARG_DATETIME}'. Invalid value '{dateTimeTextArg}'");
            }
        }

        public override void Initialize(FieldRuleConfiguration config)
        {
            base.Initialize(config);
            SetDateTimeArg();
        }
    }
}
