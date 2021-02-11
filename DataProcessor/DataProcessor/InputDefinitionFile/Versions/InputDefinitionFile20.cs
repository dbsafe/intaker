using DataProcessor.InputDefinitionFile.Models;
using System.Xml.Serialization;

namespace DataProcessor.InputDefinitionFile
{
    [XmlRoot("inputDataDefinition")]
    public class InputDefinitionFile20 : Models.InputDefinitionFile
    {
        public const string VERSION = "2.0";
        protected override string ExpectedVersion { get; } = VERSION;

        [XmlElement(ElementName = "datas")]
        public Datas Datas { get; set; }
    }

    public class Datas
    {
        [XmlAttribute("keyField")]
        public string KeyField { get; set; }

        [XmlAttribute("dataTypeField")]
        public string DataTypeField { get; set; }

        [XmlAttribute("masterDataType")]
        public string MasterDataType { get; set; }

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
