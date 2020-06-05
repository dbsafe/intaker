using DataProcessor.Domain.Contracts;
using System;
using System.Collections.Generic;

namespace DataProcessor.Decoders
{
    public class DecoderRegistry : IDecoderRegistry
    {
        public static IEnumerable<KeyValuePair<string, Type>> RegisteredObjects { get; } = new KeyValuePair<string, Type>[]
        {
            new KeyValuePair<string, Type>("DateDecoder", typeof(DateDecoder)),
            new KeyValuePair<string, Type>("NumberDecoder", typeof(NumberDecoder)),
            new KeyValuePair<string, Type>("TextDecoder", typeof(TextDecoder))
        };

        public IEnumerable<KeyValuePair<string, Type>> GetRegisteredObjects() => RegisteredObjects;
    }
}
