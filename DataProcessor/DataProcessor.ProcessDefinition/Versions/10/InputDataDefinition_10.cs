using DataProcessor.ProcessDefinition.Models;
using System;
using System.Xml.Serialization;

namespace DataProcessor.ProcessDefinition.Versions_10
{
    [XmlRoot("inputDataDefinition")]
    public class InputDataDefinition_10 : InputDataDefinition
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
