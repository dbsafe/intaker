using DataProcessor.Domain.Contracts;
using DataProcessor.Domain.Models;
using Newtonsoft.Json;
using System;

namespace DataProcessor.Rules
{
    public abstract class FieldRule<TArgs> : IFieldRule
    {
        private string _args;

        public FieldRule()
        {
        }

        public FieldRule(string ruleName, string ruleDescription, string args, ValidationResultType? failValidationResult)
        {
            Name = ruleName;
            Description = ruleDescription;
            Args = args;
            FailValidationResult = failValidationResult;
        }

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
        public ValidationResultType? FailValidationResult { get; set; }

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
    }
}
