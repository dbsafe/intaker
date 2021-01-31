using DataProcessor.InputDefinitionFile.Models;
using System.Xml.Serialization;

namespace DataProcessor.InputDefinitionFile
{
    [XmlRoot("inputDataDefinition")]
    public class InputDefinitionFile_10 : Models.InputDefinitionFile
    {
        [XmlElement("data")]
        public RowDefinition Data { get; set; }

        protected override string ExpectedVersion { get; } = "1.0";
    }
}
