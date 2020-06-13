﻿using DataProcessor.Domain.Contracts;
using System;
using System.Collections.Generic;

namespace DataProcessor.Decoders
{
    public class ObjectRegistry : BaseObjectRegistry
    {
        private static IEnumerable<KeyValuePair<string, Type>> _registeredFieldDecoders = new KeyValuePair<string, Type>[]
        {
            new KeyValuePair<string, Type>("DateDecoder", typeof(DateDecoder)),
            new KeyValuePair<string, Type>("NumberDecoder", typeof(NumberDecoder)),
            new KeyValuePair<string, Type>("TextDecoder", typeof(TextDecoder))
        };

        public override IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldDecoders() => _registeredFieldDecoders;
    }
}