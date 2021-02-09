using DataProcessor.InputDefinitionFile.Models;
using System.Xml.Serialization;

namespace DataProcessor.InputDefinitionFile
{
    [XmlRoot("inputDataDefinition")]
    public class InputDefinitionFile10 : Models.InputDefinitionFile
    {
        public const string VERSION = "1.0";

        [XmlElement("data")]
        public RowDefinition Data { get; set; }

        protected override string ExpectedVersion { get; } = VERSION;
    }
}
