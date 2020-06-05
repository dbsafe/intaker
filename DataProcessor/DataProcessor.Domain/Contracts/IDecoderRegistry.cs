using System;
using System.Collections.Generic;

namespace DataProcessor.Domain.Contracts
{
    public interface IDecoderRegistry
    {
        IEnumerable<KeyValuePair<string, Type>> GetRegisteredObjects();
    }
}
