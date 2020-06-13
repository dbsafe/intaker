using DataProcessor.Domain.Models;
using System.Collections.Generic;

namespace DataProcessor.Domain.Contracts
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
        string Args { get; set; }
        ValidationResultType? FailValidationResult { get; set; }
        void SetAggregates(IEnumerable<Aggregate> aggregates);
        void Initialize(FieldRuleConfiguration config);
    }
}
