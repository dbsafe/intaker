using DataProcessor.InputDefinitionFile.Models;
using System.Xml.Serialization;

namespace DataProcessor.InputDefinitionFile
{
    [XmlRoot("inputDataDefinition")]
    public class InputDefinitionFile_20 : Models.InputDefinitionFile
    {
        protected override string ExpectedVersion { get; } = "2.0";

        [XmlElement(ElementName = "datas")]
        public Datas Datas { get; set; }
    }

    public class Datas
    {
        [XmlAttribute("keyField")]
        public string KeyField { get; set; }

        [XmlElement("data")]
        public RowDefinition[] Rows { get; set; }
    }

    public class Data
    {
        [XmlAttribute("dataType")]
        public string DataType { get; set; }

        [XmlArrayItem(ElementName = "fields")]
        public RowDefinition[] Rows { get; set; }
    }
}
