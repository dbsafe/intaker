using DataProcessor.Contracts;
using DataProcessor.Models;
using DataProcessor.Utils;
using System;
using System.Collections.Generic;

namespace DataProcessor.Rules
{
    public abstract class FieldRule : IFieldRule
    {
        protected string _singleArg;
        protected KeyValuePair<string, string>[] _args;

        public virtual void Validate(Field field) { }
        protected virtual void SingleArgChanged() { }
        protected virtual void ArgsChanged() { }

        public string SingleArg
        {
            get => _singleArg;
            set
            {
                _singleArg = value;
                SingleArgChanged();
            }
        }

        public KeyValuePair<string, string>[] Args
        {
            get => _args;
            set
            {
                _args = value;
                ArgsChanged();
            }
        }

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
