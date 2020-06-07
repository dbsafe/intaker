using DataProcessor.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataProcessor.Decoders
{
    public class ObjectRegistry : IObjectRegistry
    {
        private static IEnumerable<KeyValuePair<string, Type>> _registeredFieldDecoders = new KeyValuePair<string, Type>[]
        {
            new KeyValuePair<string, Type>("DateDecoder", typeof(DateDecoder)),
            new KeyValuePair<string, Type>("NumberDecoder", typeof(NumberDecoder)),
            new KeyValuePair<string, Type>("TextDecoder", typeof(TextDecoder))
        };

        public IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldDecoders() => _registeredFieldDecoders;

        public IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldRules()
        {
            return Enumerable.Empty<KeyValuePair<string, Type>>();
        }
    }
}
