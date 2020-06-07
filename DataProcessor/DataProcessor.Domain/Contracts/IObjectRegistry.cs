using System;
using System.Collections.Generic;

namespace DataProcessor.Domain.Contracts
{
    public interface IObjectRegistry
    {
        IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldDecoders();
        IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldRules();
    }
}
