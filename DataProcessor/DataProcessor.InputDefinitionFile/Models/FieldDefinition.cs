﻿using DataProcessor.Domain.Models;
using System.Xml.Serialization;

namespace DataProcessor.InputDefinitionFile.Models
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

        [XmlAttribute("failValidationResult")]
        public ValidationResultType FailValidationResult { get; set; } = ValidationResultType.Error;

        [XmlArray(ElementName = "rules")]
        [XmlArrayItem(ElementName = "rule")]
        public RuleDefinition[] Rules { get; set; }

        [XmlArray(ElementName = "aggregators")]
        [XmlArrayItem(ElementName = "aggregator")]
        public AggregatorDefinition[] Aggregators { get; set; }
    }
}
