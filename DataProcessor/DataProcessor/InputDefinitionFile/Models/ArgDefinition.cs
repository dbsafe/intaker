using System.Xml.Serialization;

namespace DataProcessor.InputDefinitionFile.Models
{
    public class ArgDefinition
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }
    }
}
