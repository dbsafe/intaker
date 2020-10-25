using DataProcessor.Domain.Contracts;
using DataProcessor.Domain.Models;
using DataProcessor.Domain.Utils;
using Newtonsoft.Json;
using System;

namespace DataProcessor.Rules
{
    public abstract class FieldRule<TArgs> : IFieldRule
    {
        private string _args;

        public virtual void Validate(Field field)
        {
        }

        public string Args
        {
            get => _args;
            set
            {
                _args = value;
                DecodedArgs = GetArgs();
            }
        }
        protected TArgs DecodedArgs { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ValidationResultType FailValidationResult { get; set; }

        protected TArgs GetArgs()
        {
            if (string.IsNullOrEmpty(_args))
            {
                throw new InvalidOperationException($"RuleName: {Name}, RuleDescription: {Description} - Args is empty");
            }

            try
            {
                return JsonConvert.DeserializeObject<TArgs>(Args);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"RuleName: {Name}, RuleDescription: {Description} - Error reading Args [{Args}]", ex);
            }
        }

        protected void EnsureThatPropertiesAreInitialized()
        {
            EnsureThatPropertyIsInitialized(nameof(TArgs), _args);
            EnsureThatPropertyIsInitialized(nameof(FailValidationResult), FailValidationResult);
        }

        protected void EnsureThatPropertyIsInitialized(string name, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"Property {name} cannot be empty or null");
            }
        }

        protected void EnsureThatPropertyIsInitialized(string name, ValidationResultType value)
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
