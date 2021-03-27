using DataProcessor.Models;
using System.Xml.Serialization;

namespace DataProcessor.InputDefinitionFile.Models
{
    public class RuleDefinition
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlAttribute("rule")]
        public string Rule { get; set; }

        [XmlArray(ElementName = "args")]
        [XmlArrayItem(ElementName = "arg")]
        public ArgDefinition[] Args { get; set; }

        [XmlAttribute("failValidationResult")]
        public ValidationResultType FailValidationResult { get; set; } = ValidationResultType.Error;
    }
}
