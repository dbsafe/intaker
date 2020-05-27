using System.Xml.Serialization;

namespace DataProcessor.ProcessDefinition.Models
{
    public class FieldDefinition
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlAttribute("decoder")]
        public string Decoder { get; set; }

        [XmlAttribute("pattern")]
        public string Pattern { get; set; }

        [XmlArray(ElementName = "rules")]
        [XmlArrayItem(ElementName = "rule")]
        public RuleDefinition[] Rules { get; set; }

        [XmlArray(ElementName = "aggregators")]
        [XmlArrayItem(ElementName = "aggregator")]
        public AggregatorDefinition[] Aggregators { get; set; }

        public FieldDefinition()
        {
        }

        public FieldDefinition(string name, string description, string decoder, string pattern)
        {
            Name = name;
            Description = description;
            Decoder = decoder;
            Pattern = pattern;
        }
    }
}
