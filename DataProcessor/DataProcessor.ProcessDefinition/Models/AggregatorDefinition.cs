using System.Xml.Serialization;

namespace DataProcessor.ProcessDefinition.Models
{
    public class AggregatorDefinition
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlAttribute("aggregator")]
        public string Aggregator { get; set; }

        public AggregatorDefinition()
        {
        }

        public AggregatorDefinition(string name, string description, string aggregator)
        {
            Name = name;
            Description = description;
            Aggregator = aggregator;
        }
    }
}
