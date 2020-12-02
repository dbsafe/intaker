using System.Xml.Serialization;

namespace DataProcessor.InputDefinitionFile
{
    [XmlRoot("inputDataDefinition")]
    public class InputDefinitionFile_10 : Models.InputDefinitionFile
    {
        protected override string ExpectedVersion { get; } = "1.0";
    }
}
