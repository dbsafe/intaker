using System.Xml.Serialization;

namespace DataProcessor.InputDefinitionFile.Models
{
    public class RowDefinition
    {
        [XmlAttribute("dataType")]
        public string DataType { get; set; }

        [XmlArray(ElementName = "fields")]
        [XmlArrayItem(ElementName = "field")]
        public FieldDefinition[] Fields { get; set; }
    }
}
