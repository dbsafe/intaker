using DataProcessor.Domain.Models;
using System.ComponentModel;
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

        [XmlAttribute("args")]
        public string Args { get; set; }

        [XmlAttribute("isFixable")]
        public bool IsFixable { get; set; }

        public RuleDefinition()
        {
        }

        public RuleDefinition(string name, string description, string rule, string args)
        {
            Name = name;
            Description = description;
            Rule = rule;
            Args = args;
        }
    }
}
