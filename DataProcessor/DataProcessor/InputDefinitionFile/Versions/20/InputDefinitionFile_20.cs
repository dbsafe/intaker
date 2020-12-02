using DataProcessor.InputDefinitionFile.Models;
using System.Xml.Serialization;

namespace DataProcessor.InputDefinitionFile
{
    [XmlRoot("inputDataDefinition")]
    public class InputDefinitionFile_20 : Models.InputDefinitionFile
    {
        protected override string ExpectedVersion { get; } = "2.0";

        [XmlArray(ElementName = "datas")]
        [XmlArrayItem(ElementName = "data")]
        public RowDefinition[] Datas { get; set; }
    }
}
