using DataProcessor.Domain.Contracts;
using System;
using System.Collections.Generic;

namespace DataProcessor.Aggregators
{
    public class ObjectRegistry : BaseObjectRegistry
    {
        private static IEnumerable<KeyValuePair<string, Type>> _registeredFieldAggregators = new KeyValuePair<string, Type>[]
        {
            new KeyValuePair<string, Type>("SumAggregator", typeof(SumAggregator)),
            new KeyValuePair<string, Type>("RecordCountAggregator", typeof(RecordCountAggregator))
        };

        public override IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldDecoders() => _registeredFieldAggregators;
    }
}
