using System;
using System.Xml.Serialization;

namespace DataProcessor.InputDefinitionFile
{
    [XmlRoot("inputDataDefinition")]
    public class InputDefinitionFile_10 : Models.InputDefinitionFile
    {
        protected override void OnFrameworkVersionSet(string frameworkVersion)
        {
            if (frameworkVersion != "1.0")
            {
                throw new InvalidOperationException($"Invalid Framework Version '{frameworkVersion}'. Expected 1.0");
            }
        }
    }
}
