using DataProcessor.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataProcessor.Rules
{
    public class ObjectRegistry : IObjectRegistry
    {
        private static IEnumerable<KeyValuePair<string, Type>> _registeredFieldRules = new KeyValuePair<string, Type>[]
        {
            new KeyValuePair<string, Type>("MinDateFieldRule", typeof(MinDateFieldRule)),
            new KeyValuePair<string, Type>("MaxDateFieldRule", typeof(MaxDateFieldRule)),
            new KeyValuePair<string, Type>("MinNumberFieldRule", typeof(MinNumberFieldRule)),
            new KeyValuePair<string, Type>("MaxNumberFieldRule", typeof(MaxNumberFieldRule))
        };

        public IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldDecoders()
        {
            return Enumerable.Empty<KeyValuePair<string, Type>>();
        }

        public IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldRules()
        {
            return _registeredFieldRules;
        }
    }
}
