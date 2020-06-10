using System;
using System.Collections.Generic;
using System.Linq;

namespace DataProcessor.Domain.Contracts
{
    public interface IObjectRegistry
    {
        IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldDecoders();
        IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldRules();
        IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldAggregators();
    }

    public abstract class BaseObjectRegistry : IObjectRegistry
    {
        public virtual IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldAggregators()
        {
            return Enumerable.Empty<KeyValuePair<string, Type>>();
        }

        public virtual IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldDecoders()
        {
            return Enumerable.Empty<KeyValuePair<string, Type>>();
        }

        public virtual IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldRules()
        {
            return Enumerable.Empty<KeyValuePair<string, Type>>();
        }
    }
}
