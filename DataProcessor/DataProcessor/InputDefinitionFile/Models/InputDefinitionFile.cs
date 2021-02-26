using System;
using System.Xml.Serialization;

namespace DataProcessor.InputDefinitionFile.Models
{
    [XmlRoot("inputDataDefinition")]
    public class InputDefinitionFrameworkVersion
    {
        [XmlAttribute("frameworkVersion")]
        public string FrameworkVersion { get; set; }
    }

    public abstract class InputDefinitionFile
    {
        private string frameworkVersion;

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlAttribute("delimiter")]
        public string Delimiter { get; set; }

        [XmlAttribute("hasFieldsEnclosedInQuotes")]
        public bool HasFieldsEnclosedInQuotes { get; set; }

        [XmlAttribute("commentedOutIndicator")]
        public string CommentedOutIndicator { get; set; }

        [XmlElement("header")]
        public RowDefinition Header { get; set; }

        [XmlElement("trailer")]
        public RowDefinition Trailer { get; set; }

        [XmlAttribute("frameworkVersion")]
        public string FrameworkVersion
        {
            get => frameworkVersion;
            set
            {
                frameworkVersion = value;
                OnFrameworkVersionSet(frameworkVersion);
            }
        }

        [XmlAttribute("createRowJsonEnabled")]
        public bool CreateRowJsonEnabled { get; set; }

        private void OnFrameworkVersionSet(string frameworkVersion)
        {
            if (frameworkVersion != ExpectedVersion)
            {
                throw new InvalidOperationException($"Invalid Framework Version '{frameworkVersion}'. Expected {ExpectedVersion}");
            }
        }

        protected abstract string ExpectedVersion { get; }
    }
}
