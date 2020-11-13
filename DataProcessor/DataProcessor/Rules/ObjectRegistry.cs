using DataProcessor.Contracts;
using System;
using System.Collections.Generic;

namespace DataProcessor.Rules
{
    public class ObjectRegistry : BaseObjectRegistry
    {
        private static readonly IEnumerable<KeyValuePair<string, Type>> _registeredFieldRules = new KeyValuePair<string, Type>[]
        {
            new KeyValuePair<string, Type>("MinDateFieldRule", typeof(MinDateFieldRule)),
            new KeyValuePair<string, Type>("MaxDateFieldRule", typeof(MaxDateFieldRule)),
            new KeyValuePair<string, Type>("MinNumberFieldRule", typeof(MinNumberFieldRule)),
            new KeyValuePair<string, Type>("MaxNumberFieldRule", typeof(MaxNumberFieldRule)),
            new KeyValuePair<string, Type>("MatchesAggregateRule", typeof(MatchesAggregateRule))
        };

        public override IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldRules() => _registeredFieldRules;
    }
}
