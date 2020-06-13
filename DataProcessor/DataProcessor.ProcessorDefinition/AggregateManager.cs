using DataProcessor.Domain.Models;
using DataProcessor.Domain.Utils;
using System.Collections.Generic;
using System.Linq;

namespace DataProcessor.ProcessorDefinition
{
    public class AggregateManager
    {
        private readonly List<Aggregate> _aggregates = new List<Aggregate>();

        public Aggregate GetAggregateByName(string name)
        {
            var aggregate = _aggregates.FirstOrDefault(a => a.Name == name);
            if (aggregate == null)
            {
                aggregate = new Aggregate { Name = name };
                _aggregates.Add(aggregate);
                DataProcessorGlobal.Debug($"Created Aggregate: {name}");
            }

            return aggregate;
        }

        public IEnumerable<Aggregate> GetAggregates()
        {
            return _aggregates;
        }
    }
}
