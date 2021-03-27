using DataProcessor.Models;
using System.Collections.Generic;

namespace DataProcessor.Contracts
{
    public class FieldRuleConfiguration
    {
        public IEnumerable<Aggregate> Aggregates { get; set; }
    }

    public interface IFieldRule
    {
        void Validate(Field field);
        string Name { get; set; }
        string Description { get; set; }
        KeyValuePair<string, string>[] Args { get; set; }
        ValidationResultType FailValidationResult { get; set; }
        void Initialize(FieldRuleConfiguration config);
    }
}
