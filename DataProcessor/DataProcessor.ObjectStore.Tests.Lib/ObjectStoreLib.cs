using DataProcessor.Contracts;
using DataProcessor.Models;
using System;
using System.Collections.Generic;

namespace DataProcessor.ObjectStore.Tests.Lib
{
    public class ObjectStoreLib : BaseObjectRegistry
    {
        public static IEnumerable<KeyValuePair<string, Type>> RegisteredFieldDecoders { get; } = new KeyValuePair<string, Type>[]
             {
                new KeyValuePair<string, Type>("lib-decoder", typeof(LibDecoder))
             };

        public static IEnumerable<KeyValuePair<string, Type>> RegisteredFieldRules { get; } = new KeyValuePair<string, Type>[]
             {
                new KeyValuePair<string, Type>("lib-rule", typeof(LibRule))
             };

        public static IEnumerable<KeyValuePair<string, Type>> RegisteredFieldAggregators { get; } = new KeyValuePair<string, Type>[]
             {
                new KeyValuePair<string, Type>("lib-aggregator", typeof(LibAggregator))
             };

        public override IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldDecoders() => RegisteredFieldDecoders;
        public override IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldRules() => RegisteredFieldRules;
        public override IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldAggregators() => RegisteredFieldAggregators;
    }

    public class LibDecoder : Decoders.FieldDecoder
    {
        public override void Decode(Field field)
        {
            field.ValidationResult = ValidationResultType.Valid;
            field.Value = field.Raw;
        }
    }

    public class LibRule : Rules.FieldRule { }

    public class LibAggregator : Aggregators.FieldAggregator
    {
        public override void AggregateField(Field field) { }
    }
}
