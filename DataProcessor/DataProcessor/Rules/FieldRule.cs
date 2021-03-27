using DataProcessor.Contracts;
using DataProcessor.Models;
using DataProcessor.Utils;
using System;
using System.Collections.Generic;

namespace DataProcessor.Rules
{
    public abstract class FieldRule : IFieldRule
    {
        public virtual void Validate(Field field) { }

        public KeyValuePair<string, string>[] Args { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public ValidationResultType FailValidationResult { get; set; }

        private void EnsureThatPropertiesAreInitialized()
        {
            EnsureThatPropertyIsInitialized(nameof(FailValidationResult), FailValidationResult);
        }

        private void EnsureThatPropertyIsInitialized(string name, ValidationResultType value)
        {
            if (value == 0)
            {
                throw new InvalidOperationException($"Property {name} must be set");
            }
        }

        public virtual void Initialize(FieldRuleConfiguration config)
        {
            DataProcessorGlobal.Debug($"Rule: {Name}. Initializing.");
            EnsureThatPropertiesAreInitialized();
        }
    }
}
